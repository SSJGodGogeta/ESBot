using ESBot.Application.Contracts;
using ESBot.Application.Models;
using ESBot.Domain.Entities;
using ESBot.Domain.Enums;

namespace ESBot.Application.Services;

public class ChatService(ISessionRepository repository, ILlmService llmService)
{
    public const string UnavailableResponse =
        "ESBot is currently unavailable. Please try again later.";

    public const string UnavailableFeedback =
        "Feedback is currently unavailable. Please try again later.";

    public Session StartNewLearningSession(Guid userId)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("A user ID is required.", nameof(userId));

        var session = new Session { UserId = userId };

        repository.AddSession(session);
        repository.SaveChanges();

        return session;
    }

    public Message SendMessage(Guid sessionId, string content)
    {
        var session = GetRequiredSession(sessionId);
        ValidateRequiredText(content, nameof(content), 4000);

        var userMessage = new Message
        {
            SessionId = session.Id,
            Session = session,
            Content = content,
            Role = EMessageRole.User
        };
        AddMessage(session, userMessage);

        string response;
        try
        {
            response = llmService.GenerateResponse(content);
            if (string.IsNullOrWhiteSpace(response))
                response = UnavailableResponse;
        }
        catch (Exception)
        {
            response = UnavailableResponse;
        }

        var botMessage = new Message
        {
            SessionId = session.Id,
            Session = session,
            Content = response,
            Role = EMessageRole.Bot
        };
        AddMessage(session, botMessage);
        repository.SaveChanges();

        return botMessage;
    }

    public QuizRequest GenerateQuiz(Guid sessionId, string topic, EDifficulty difficulty, int questionCount)
    {
        var session = GetRequiredSession(sessionId);
        ValidateRequiredText(topic, nameof(topic), 200);

        if (!Enum.IsDefined(difficulty))
            throw new ArgumentOutOfRangeException(nameof(difficulty));
        if (questionCount <= 0)
            throw new ArgumentOutOfRangeException(nameof(questionCount), "At least one question is required.");

        var quizRequest = new QuizRequest
        {
            SessionId = session.Id,
            Session = session,
            Topic = topic,
            Difficulty = difficulty
        };
        IReadOnlyList<GeneratedQuizItem> generatedItems;
        try
        {
            generatedItems = llmService.GenerateQuiz(topic, difficulty, questionCount);
        }
        catch (Exception)
        {
            return quizRequest;
        }

        session.QuizRequests.Add(quizRequest);
        repository.AddQuizRequest(quizRequest);
        foreach (var generatedItem in generatedItems)
        {
            ValidateRequiredText(generatedItem.Question, nameof(generatedItem.Question), 2000);
            ValidateRequiredText(generatedItem.CorrectAnswer, nameof(generatedItem.CorrectAnswer), 1000);

            var item = new QuizItem
            {
                QuizRequestId = quizRequest.Id,
                QuizRequest = quizRequest,
                Question = generatedItem.Question,
                CorrectAnswer = generatedItem.CorrectAnswer
            };
            quizRequest.QuizItems.Add(item);
            repository.AddQuizItem(item);
        }

        repository.SaveChanges();
        return quizRequest;
    }

    public EvaluationResult EvaluateAnswer(Guid sessionId, Guid questionId, string submittedAnswer)
    {
        var session = GetRequiredSession(sessionId);
        ValidateRequiredText(submittedAnswer, nameof(submittedAnswer), 2000);

        var question = repository.FindQuizItemById(questionId)
            ?? throw new KeyNotFoundException($"Quiz item '{questionId}' was not found.");

        if (question.QuizRequest.SessionId != session.Id)
            throw new InvalidOperationException("The quiz item does not belong to this session.");

        var answer = new SubmittedAnswer
        {
            QuizItemId = question.Id,
            QuizItem = question,
            Answer = submittedAnswer
        };
        question.SubmittedAnswers.Add(answer);
        repository.AddSubmittedAnswer(answer);

        AnswerEvaluation generatedEvaluation;
        try
        {
            generatedEvaluation = llmService.EvaluateAnswer(
                question.Question,
                question.CorrectAnswer,
                submittedAnswer);
        }
        catch (Exception)
        {
            generatedEvaluation = new AnswerEvaluation(false, 0, UnavailableFeedback);
        }

        var result = new EvaluationResult
        {
            SubmittedAnswerId = answer.Id,
            SubmittedAnswer = answer,
            IsCorrect = generatedEvaluation.IsCorrect,
            Score = generatedEvaluation.Score,
            Feedback = generatedEvaluation.Feedback
        };
        answer.EvaluationResult = result;
        repository.AddEvaluationResult(result);
        repository.SaveChanges();

        return result;
    }

    private Session GetRequiredSession(Guid sessionId)
    {
        if (sessionId == Guid.Empty)
            throw new ArgumentException("A session ID is required.", nameof(sessionId));

        return repository.FindSessionById(sessionId)
            ?? throw new KeyNotFoundException($"Session '{sessionId}' was not found.");
    }

    private void AddMessage(Session session, Message message)
    {
        session.AddMessage(message);
        repository.AddMessage(message);
    }

    private static void ValidateRequiredText(string value, string parameterName, int maximumLength)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("A non-empty value is required.", parameterName);
        if (value.Length > maximumLength)
            throw new ArgumentException($"The value may not exceed {maximumLength} characters.", parameterName);
    }
}
