using ESBot.Application.Contracts;
using ESBot.Application.Models;
using ESBot.Application.Services;
using ESBot.Domain.Entities;
using ESBot.Domain.Enums;
using Moq;

namespace ESBot.Tests;

public class ChatServiceTests
{
    private readonly Mock<ISessionRepository> _repository = new();
    private readonly Mock<ILlmService> _llmService = new();

    [Fact]
    public void StartNewLearningSession_CreatesAndPersistsSessionForUser()
    {
        var userId = Guid.NewGuid();
        var service = CreateService();

        var session = service.StartNewLearningSession(userId);

        Assert.Equal(userId, session.UserId);
        Assert.Empty(session.Messages);
        _repository.Verify(r => r.AddSession(It.Is<Session>(s => s == session && s.UserId == userId)), Times.Once);
        _repository.Verify(r => r.SaveChanges(), Times.Once);
        _llmService.VerifyNoOtherCalls();
    }

    [Fact]
    public void SendMessage_StoresUserAndGeneratedBotMessages()
    {
        var session = NewSession();
        var storedMessages = new List<Message>();
        _repository.Setup(r => r.FindSessionById(session.Id)).Returns(session);
        _repository.Setup(r => r.AddMessage(It.IsAny<Message>()))
            .Callback<Message>(storedMessages.Add);
        _llmService.Setup(l => l.GenerateResponse("Explain polymorphism."))
            .Returns("A mocked explanation.");

        var response = CreateService().SendMessage(session.Id, "Explain polymorphism.");

        Assert.Equal(EMessageRole.Bot, response.Role);
        Assert.Equal("A mocked explanation.", response.Content);
        Assert.Collection(
            storedMessages,
            userMessage =>
            {
                Assert.Equal(EMessageRole.User, userMessage.Role);
                Assert.Equal("Explain polymorphism.", userMessage.Content);
                Assert.Equal(session.Id, userMessage.SessionId);
            },
            botMessage =>
            {
                Assert.Equal(EMessageRole.Bot, botMessage.Role);
                Assert.Equal("A mocked explanation.", botMessage.Content);
                Assert.Equal(session.Id, botMessage.SessionId);
            });
        Assert.Equal(2, session.Messages.Count);
        _llmService.Verify(l => l.GenerateResponse("Explain polymorphism."), Times.Once);
        _repository.Verify(r => r.SaveChanges(), Times.Once);
    }

    [Fact]
    public void GenerateQuiz_RequestsQuestionsAndStoresQuizInSession()
    {
        var session = NewSession();
        _repository.Setup(r => r.FindSessionById(session.Id)).Returns(session);
        _llmService.Setup(l => l.GenerateQuiz("Databases", EDifficulty.Medium, 2))
            .Returns(
            [
                new GeneratedQuizItem("What is a primary key?", "A unique row identifier."),
                new GeneratedQuizItem("What is normalization?", "Structuring data to reduce redundancy.")
            ]);

        var quiz = CreateService().GenerateQuiz(session.Id, "Databases", EDifficulty.Medium, 2);

        Assert.Equal(session.Id, quiz.SessionId);
        Assert.Equal("Databases", quiz.Topic);
        Assert.Collection(
            quiz.QuizItems,
            item => Assert.Equal("What is a primary key?", item.Question),
            item => Assert.Equal("What is normalization?", item.Question));
        Assert.Contains(quiz, session.QuizRequests);
        _llmService.Verify(l => l.GenerateQuiz("Databases", EDifficulty.Medium, 2), Times.Once);
        _repository.Verify(r => r.AddQuizRequest(quiz), Times.Once);
        _repository.Verify(r => r.AddQuizItem(It.IsAny<QuizItem>()), Times.Exactly(2));
        _repository.Verify(r => r.SaveChanges(), Times.Once);
    }

    [Fact]
    public void GenerateQuiz_WhenLlmFails_ReturnsEmptyQuizWithoutPersistingIt()
    {
        var session = NewSession();
        _repository.Setup(r => r.FindSessionById(session.Id)).Returns(session);
        _llmService.Setup(l => l.GenerateQuiz("Databases", EDifficulty.Medium, 2))
            .Throws(new InvalidOperationException("Inference engine unavailable."));

        var quiz = CreateService().GenerateQuiz(session.Id, "Databases", EDifficulty.Medium, 2);

        Assert.Equal("Databases", quiz.Topic);
        Assert.Empty(quiz.QuizItems);
        Assert.Empty(session.QuizRequests);
        _repository.Verify(r => r.AddQuizRequest(It.IsAny<QuizRequest>()), Times.Never);
        _repository.Verify(r => r.AddQuizItem(It.IsAny<QuizItem>()), Times.Never);
        _repository.Verify(r => r.SaveChanges(), Times.Never);
    }

    [Fact]
    public void EvaluateAnswer_StoresAnswerAndReturnsMockedEvaluation()
    {
        var session = NewSession();
        var question = NewQuestion(session);
        _repository.Setup(r => r.FindSessionById(session.Id)).Returns(session);
        _repository.Setup(r => r.FindQuizItemById(question.Id)).Returns(question);
        _llmService.Setup(l => l.EvaluateAnswer(question.Question, question.CorrectAnswer, "Paris"))
            .Returns(new AnswerEvaluation(true, 1.0, "Correct."));

        var result = CreateService().EvaluateAnswer(session.Id, question.Id, "Paris");

        Assert.True(result.IsCorrect);
        Assert.Equal(1.0, result.Score);
        Assert.Equal("Correct.", result.Feedback);
        Assert.NotNull(result.SubmittedAnswer);
        Assert.Equal("Paris", result.SubmittedAnswer.Answer);
        Assert.Same(result, result.SubmittedAnswer.EvaluationResult);
        _llmService.Verify(l => l.EvaluateAnswer(question.Question, question.CorrectAnswer, "Paris"), Times.Once);
        _repository.Verify(r => r.AddSubmittedAnswer(result.SubmittedAnswer), Times.Once);
        _repository.Verify(r => r.AddEvaluationResult(result), Times.Once);
        _repository.Verify(r => r.SaveChanges(), Times.Once);
    }

    [Fact]
    public void SendMessage_WhenLlmFails_ReturnsAndStoresFallbackBotResponse()
    {
        var session = NewSession();
        var storedMessages = new List<Message>();
        _repository.Setup(r => r.FindSessionById(session.Id)).Returns(session);
        _repository.Setup(r => r.AddMessage(It.IsAny<Message>()))
            .Callback<Message>(storedMessages.Add);
        _llmService.Setup(l => l.GenerateResponse(It.IsAny<string>()))
            .Throws(new InvalidOperationException("Inference engine unavailable."));

        var response = CreateService().SendMessage(session.Id, "Are you available?");

        Assert.Equal(ChatService.UnavailableResponse, response.Content);
        Assert.Equal(EMessageRole.Bot, response.Role);
        Assert.Contains(storedMessages, message => message.Content == ChatService.UnavailableResponse);
        _repository.Verify(r => r.AddMessage(It.IsAny<Message>()), Times.Exactly(2));
        _repository.Verify(r => r.SaveChanges(), Times.Once);
    }

    [Fact]
    public void EvaluateAnswer_WhenLlmFails_PersistsFallbackFeedback()
    {
        var session = NewSession();
        var question = NewQuestion(session);
        _repository.Setup(r => r.FindSessionById(session.Id)).Returns(session);
        _repository.Setup(r => r.FindQuizItemById(question.Id)).Returns(question);
        _llmService.Setup(l => l.EvaluateAnswer(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .Throws(new InvalidOperationException("Inference engine unavailable."));

        var result = CreateService().EvaluateAnswer(session.Id, question.Id, "London");

        Assert.False(result.IsCorrect);
        Assert.Equal(0, result.Score);
        Assert.Equal(ChatService.UnavailableFeedback, result.Feedback);
        _repository.Verify(r => r.AddSubmittedAnswer(It.IsAny<SubmittedAnswer>()), Times.Once);
        _repository.Verify(r => r.AddEvaluationResult(result), Times.Once);
        _repository.Verify(r => r.SaveChanges(), Times.Once);
    }

    private ChatService CreateService() => new(_repository.Object, _llmService.Object);

    private static Session NewSession() => new() { Id = Guid.NewGuid(), UserId = Guid.NewGuid() };

    private static QuizItem NewQuestion(Session session)
    {
        var quiz = new QuizRequest
        {
            Id = Guid.NewGuid(),
            SessionId = session.Id,
            Session = session,
            Topic = "Geography",
            Difficulty = EDifficulty.Easy
        };

        return new QuizItem
        {
            Id = Guid.NewGuid(),
            QuizRequestId = quiz.Id,
            QuizRequest = quiz,
            Question = "What is the capital of France?",
            CorrectAnswer = "Paris"
        };
    }
}
