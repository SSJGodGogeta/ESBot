using ESBot.Domain.Entities;
using ESBot.Domain.Enums;
using ESBot.Infrastructure.Data;

namespace ESBot.Infrastructure.Data;

public static class DbSeeder
{
    public static void Seed(EsBotDbContext db)
    {
        if (db.Users.Any())
            return;

        // =====================
        // USERS
        // =====================
        var user1 = new User
        {
            Username = "alice",
            Email = "alice@esbot.com",
            HashedPassword = "hashed1"
        };

        var user2 = new User
        {
            Username = "bob",
            Email = "bob@esbot.com",
            HashedPassword = "hashed2"
        };

        db.Users.AddRange(user1, user2);
        db.SaveChanges();

        // =====================
        // SESSIONS
        // =====================
        var session1 = new UserSession
        {
            UserId = user1.Id
        };

        var session2 = new UserSession
        {
            UserId = user2.Id,
        };

        db.UserSessions.AddRange(session1, session2);
        db.SaveChanges();

        // =====================
        // MESSAGES
        // =====================
        db.Messages.AddRange(
            new Message
            {
                SessionId = session1.Id,
                Content = "Hi ESBot, what is polymorphism?",
                Role = EMessageRole.User
            },
            new Message
            {
                SessionId = session1.Id,
                Content = "Polymorphism is ...",
                Role = EMessageRole.Bot
            }
        );

        // =====================
        // QUIZ REQUESTS
        // =====================
        var quizRequest = new QuizRequest
        {
            SessionId = session1.Id,
            Topic = "C# Basics",
            Difficulty = EDifficulty.Medium
        };

        db.QuizRequests.Add(quizRequest);
        db.SaveChanges();

        // =====================
        // QUIZ ITEMS
        // =====================
        var item1 = new QuizItem
        {
            QuizRequestId = quizRequest.Id,
            Question = "What is an interface?",
            CorrectAnswer = "A contract for classes"
        };

        var item2 = new QuizItem
        {
            QuizRequestId = quizRequest.Id,
            Question = "What is inheritance?",
            CorrectAnswer = "Reusing base class behavior"
        };

        db.QuizItems.AddRange(item1, item2);
        db.SaveChanges();

        // =====================
        // SUBMITTED ANSWERS
        // =====================
        var answer1 = new SubmittedAnswer
        {
            QuizItemId = item1.Id,
            Answer = "A blueprint"
        };

        var answer2 = new SubmittedAnswer
        {
            QuizItemId = item2.Id,
            Answer = "Code reuse"
        };

        db.SubmittedAnswers.AddRange(answer1, answer2);
        db.SaveChanges();

        // =====================
        // EVALUATION RESULTS
        // =====================
        db.EvaluationResults.AddRange(
            new EvaluationResult
            {
                SubmittedAnswerId = answer1.Id,
                IsCorrect = false,
                Score = 0.3,
                Feedback = "Too vague"
            },
            new EvaluationResult
            {
                SubmittedAnswerId = answer2.Id,
                IsCorrect = true,
                Score = 0.9,
                Feedback = "Correct"
            }
        );

        db.SaveChanges();
    }
}