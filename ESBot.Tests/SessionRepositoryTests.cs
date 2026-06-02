using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using ESBot.Domain.Entities;
using ESBot.Domain.Enums;
using ESBot.Infrastructure.Data;
using ESBot.Infrastructure.Repositories;

namespace ESBot.Tests;

public class SessionRepositoryTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly EsBotDbContext _context;
    private readonly SessionRepository _repository;
    private readonly User _testUser;

    public SessionRepositoryTests()
    {
        _connection = new SqliteConnection("Filename=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<EsBotDbContext>()
            .UseSqlite(_connection)
            .Options;

        _context = new EsBotDbContext(options);
        _context.Database.EnsureCreated();
        _repository = new SessionRepository(_context);

        _testUser = new User { Username = "testuser", Email = "test@example.com", HashedPassword = "hashedpw1" };
        _context.Users.Add(_testUser);
        _context.SaveChanges();
    }

    // --- CreateSession ---

    [Fact]
    public void CreateSession_WithValidSession_PersistsToDatabase()
    {
        var session = new Session { UserId = _testUser.Id };

        _repository.CreateSession(session);

        var persisted = _context.UserSessions.SingleOrDefault(s => s.Id == session.Id);
        Assert.NotNull(persisted);
        Assert.Equal(_testUser.Id, persisted.UserId);
    }

    [Fact]
    public void CreateSession_WithValidSession_ReturnsPersistedSession()
    {
        var session = new Session { UserId = _testUser.Id };

        var result = _repository.CreateSession(session);

        Assert.NotNull(result);
        Assert.Equal(session.Id, result.Id);
    }

    [Fact]
    public void CreateSession_WithNullSession_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => _repository.CreateSession(null!));
    }

    // --- FindSessionById ---

    [Fact]
    public void FindSessionById_WithExistingSession_ReturnsSession()
    {
        var session = new Session { UserId = _testUser.Id };
        _context.UserSessions.Add(session);
        _context.SaveChanges();

        var result = _repository.FindSessionById(session.Id);

        Assert.NotNull(result);
        Assert.Equal(session.Id, result.Id);
    }

    [Fact]
    public void FindSessionById_WithNonExistentId_ReturnsNull()
    {
        var result = _repository.FindSessionById(Guid.NewGuid());

        Assert.Null(result);
    }

    // --- FindSessionsByUser ---

    [Fact]
    public void FindSessionsByUser_WithMultipleSessions_ReturnsAllForUser()
    {
        var sessionA = new Session { UserId = _testUser.Id };
        var sessionB = new Session { UserId = _testUser.Id };
        _context.UserSessions.AddRange(sessionA, sessionB);
        _context.SaveChanges();

        var result = _repository.FindSessionsByUser(_testUser.Id);

        Assert.Equal(2, result.Count);
        Assert.All(result, s => Assert.Equal(_testUser.Id, s.UserId));
    }

    [Fact]
    public void FindSessionsByUser_ReturnsSessionsOrderedByStartedAtDescending()
    {
        var sessionA = new Session { UserId = _testUser.Id };
        _context.UserSessions.Add(sessionA);
        _context.SaveChanges();

        var sessionB = new Session { UserId = _testUser.Id };
        _context.UserSessions.Add(sessionB);
        _context.SaveChanges();

        var result = _repository.FindSessionsByUser(_testUser.Id);

        Assert.Equal(2, result.Count);
        Assert.True(result[0].StartedAt >= result[1].StartedAt);
    }

    [Fact]
    public void FindSessionsByUser_WithNoSessions_ReturnsEmptyList()
    {
        var result = _repository.FindSessionsByUser(_testUser.Id);

        Assert.Empty(result);
    }

    [Fact]
    public void FindSessionsByUser_DoesNotReturnOtherUsersSessions()
    {
        var otherUser = new User { Username = "otheruser", Email = "other@example.com", HashedPassword = "hashedpw1" };
        _context.Users.Add(otherUser);
        _context.UserSessions.Add(new Session { UserId = otherUser.Id });
        _context.SaveChanges();

        var result = _repository.FindSessionsByUser(_testUser.Id);

        Assert.Empty(result);
    }

    // --- AppendMessage ---

    [Fact]
    public void AppendMessage_WithValidData_PersistsMessageLinkedToSession()
    {
        var session = new Session { UserId = _testUser.Id };
        _context.UserSessions.Add(session);
        _context.SaveChanges();
        var message = new Message { Content = "Hello", Role = EMessageRole.User };

        _repository.AppendMessage(session.Id, message);

        var persisted = _context.Messages.SingleOrDefault(m => m.Id == message.Id);
        Assert.NotNull(persisted);
        Assert.Equal(session.Id, persisted.SessionId);
        Assert.Equal("Hello", persisted.Content);
    }

    [Fact]
    public void AppendMessage_WithNonExistentSession_ThrowsKeyNotFoundException()
    {
        var message = new Message { Content = "Hello", Role = EMessageRole.User };

        Assert.Throws<KeyNotFoundException>(() => _repository.AppendMessage(Guid.NewGuid(), message));
    }

    [Fact]
    public void AppendMessage_WithNullMessage_ThrowsArgumentNullException()
    {
        var session = new Session { UserId = _testUser.Id };
        _context.UserSessions.Add(session);
        _context.SaveChanges();

        Assert.Throws<ArgumentNullException>(() => _repository.AppendMessage(session.Id, null!));
    }

    // --- GetMessageHistory ---

    [Fact]
    public void GetMessageHistory_WithMessages_ReturnsAllMessagesForSession()
    {
        var session = new Session { UserId = _testUser.Id };
        _context.UserSessions.Add(session);
        _context.SaveChanges();
        _context.Messages.AddRange(
            new Message { Content = "First", Role = EMessageRole.User, SessionId = session.Id },
            new Message { Content = "Second", Role = EMessageRole.Bot, SessionId = session.Id });
        _context.SaveChanges();

        var result = _repository.GetMessageHistory(session.Id);

        Assert.Equal(2, result.Count);
        Assert.All(result, m => Assert.Equal(session.Id, m.SessionId));
    }

    [Fact]
    public void GetMessageHistory_ReturnsMessagesOrderedByCreatedAtAscending()
    {
        var session = new Session { UserId = _testUser.Id };
        _context.UserSessions.Add(session);
        _context.SaveChanges();
        var first = new Message { Content = "First", Role = EMessageRole.User, SessionId = session.Id };
        _context.Messages.Add(first);
        _context.SaveChanges();
        var second = new Message { Content = "Second", Role = EMessageRole.Bot, SessionId = session.Id };
        _context.Messages.Add(second);
        _context.SaveChanges();

        var result = _repository.GetMessageHistory(session.Id);

        Assert.Equal(2, result.Count);
        Assert.True(result[0].CreatedAt <= result[1].CreatedAt);
    }

    [Fact]
    public void GetMessageHistory_WithNoMessages_ReturnsEmptyList()
    {
        var session = new Session { UserId = _testUser.Id };
        _context.UserSessions.Add(session);
        _context.SaveChanges();

        var result = _repository.GetMessageHistory(session.Id);

        Assert.Empty(result);
    }

    [Fact]
    public void GetMessageHistory_DoesNotReturnMessagesFromOtherSessions()
    {
        var sessionA = new Session { UserId = _testUser.Id };
        var sessionB = new Session { UserId = _testUser.Id };
        _context.UserSessions.AddRange(sessionA, sessionB);
        _context.SaveChanges();
        _context.Messages.Add(new Message { Content = "Belongs to B", Role = EMessageRole.User, SessionId = sessionB.Id });
        _context.SaveChanges();

        var result = _repository.GetMessageHistory(sessionA.Id);

        Assert.Empty(result);
    }

    // --- UpdateSessionEndTime ---

    [Fact]
    public void UpdateSessionEndTime_WithValidTime_PersistsEndedAt()
    {
        var session = new Session { UserId = _testUser.Id };
        _context.UserSessions.Add(session);
        _context.SaveChanges();
        var endTime = new DateTime(2026, 1, 1, 12, 0, 0, DateTimeKind.Utc);

        _repository.UpdateSessionEndTime(session.Id, endTime);

        var updated = _context.UserSessions.Single(s => s.Id == session.Id);
        Assert.Equal(endTime, updated.EndedAt);
    }

    [Fact]
    public void UpdateSessionEndTime_WithNull_ClearsEndedAt()
    {
        var session = new Session { UserId = _testUser.Id, EndedAt = DateTime.UtcNow };
        _context.UserSessions.Add(session);
        _context.SaveChanges();

        _repository.UpdateSessionEndTime(session.Id, null);

        var updated = _context.UserSessions.Single(s => s.Id == session.Id);
        Assert.Null(updated.EndedAt);
    }

    [Fact]
    public void UpdateSessionEndTime_ReturnsUpdatedSession()
    {
        var session = new Session { UserId = _testUser.Id };
        _context.UserSessions.Add(session);
        _context.SaveChanges();
        var endTime = DateTime.UtcNow;

        var result = _repository.UpdateSessionEndTime(session.Id, endTime);

        Assert.Equal(session.Id, result.Id);
        Assert.NotNull(result.EndedAt);
    }

    [Fact]
    public void UpdateSessionEndTime_WithNonExistentSession_ThrowsKeyNotFoundException()
    {
        Assert.Throws<KeyNotFoundException>(() =>
            _repository.UpdateSessionEndTime(Guid.NewGuid(), DateTime.UtcNow));
    }

    // --- DeleteSession ---

    [Fact]
    public void DeleteSession_WithExistingSession_RemovesRecordFromDatabase()
    {
        var session = new Session { UserId = _testUser.Id };
        _context.UserSessions.Add(session);
        _context.SaveChanges();

        _repository.DeleteSession(session.Id);

        Assert.Null(_context.UserSessions.SingleOrDefault(s => s.Id == session.Id));
    }

    [Fact]
    public void DeleteSession_WithExistingSession_ReturnsTrue()
    {
        var session = new Session { UserId = _testUser.Id };
        _context.UserSessions.Add(session);
        _context.SaveChanges();

        var result = _repository.DeleteSession(session.Id);

        Assert.True(result);
    }

    [Fact]
    public void DeleteSession_WithNonExistentSession_ReturnsFalse()
    {
        var result = _repository.DeleteSession(Guid.NewGuid());

        Assert.False(result);
    }

    // --- FindQuizItemById ---

    [Fact]
    public void FindQuizItemById_WithExistingItem_ReturnsQuizItemWithQuizRequestEagerLoaded()
    {
        var session = new Session { UserId = _testUser.Id };
        _context.UserSessions.Add(session);
        var quizRequest = new QuizRequest { SessionId = session.Id, Topic = "C#", Difficulty = EDifficulty.Easy };
        _context.QuizRequests.Add(quizRequest);
        var quizItem = new QuizItem { QuizRequestId = quizRequest.Id, Question = "What is C#?", CorrectAnswer = "A programming language." };
        _context.QuizItems.Add(quizItem);
        _context.SaveChanges();

        var result = _repository.FindQuizItemById(quizItem.Id);

        Assert.NotNull(result);
        Assert.Equal(quizItem.Id, result.Id);
        Assert.NotNull(result.QuizRequest);
        Assert.Equal("C#", result.QuizRequest.Topic);
    }

    [Fact]
    public void FindQuizItemById_WithNonExistentId_ReturnsNull()
    {
        var result = _repository.FindQuizItemById(Guid.NewGuid());

        Assert.Null(result);
    }

    // --- Deferred save pattern (Add* + SaveChanges) ---

    [Fact]
    public void AddSession_WithoutSaveChanges_IsTrackedAsAddedButNotYetPersisted()
    {
        var session = new Session { UserId = _testUser.Id };

        _repository.AddSession(session);

        Assert.Equal(EntityState.Added, _context.Entry(session).State);
    }

    [Fact]
    public void AddSession_ThenSaveChanges_PersistsRecordToDatabase()
    {
        var session = new Session { UserId = _testUser.Id };

        _repository.AddSession(session);
        _repository.SaveChanges();

        Assert.NotNull(_context.UserSessions.SingleOrDefault(s => s.Id == session.Id));
    }

    [Fact]
    public void AddMessage_ThenSaveChanges_PersistsRecordToDatabase()
    {
        var session = new Session { UserId = _testUser.Id };
        _context.UserSessions.Add(session);
        _context.SaveChanges();
        var message = new Message { Content = "Deferred message", Role = EMessageRole.User, SessionId = session.Id };

        _repository.AddMessage(message);
        _repository.SaveChanges();

        Assert.NotNull(_context.Messages.SingleOrDefault(m => m.Id == message.Id));
    }

    [Fact]
    public void AddQuizRequest_ThenSaveChanges_PersistsRecordToDatabase()
    {
        var session = new Session { UserId = _testUser.Id };
        _context.UserSessions.Add(session);
        _context.SaveChanges();
        var quizRequest = new QuizRequest { SessionId = session.Id, Topic = "Math", Difficulty = EDifficulty.Medium };

        _repository.AddQuizRequest(quizRequest);
        _repository.SaveChanges();

        var persisted = _context.QuizRequests.SingleOrDefault(qr => qr.Id == quizRequest.Id);
        Assert.NotNull(persisted);
        Assert.Equal("Math", persisted.Topic);
        Assert.Equal(EDifficulty.Medium, persisted.Difficulty);
    }

    [Fact]
    public void AddQuizItem_ThenSaveChanges_PersistsRecordToDatabase()
    {
        var session = new Session { UserId = _testUser.Id };
        _context.UserSessions.Add(session);
        var quizRequest = new QuizRequest { SessionId = session.Id, Topic = "Science", Difficulty = EDifficulty.Hard };
        _context.QuizRequests.Add(quizRequest);
        _context.SaveChanges();
        var quizItem = new QuizItem { QuizRequestId = quizRequest.Id, Question = "What is gravity?", CorrectAnswer = "A fundamental force." };

        _repository.AddQuizItem(quizItem);
        _repository.SaveChanges();

        Assert.NotNull(_context.QuizItems.SingleOrDefault(qi => qi.Id == quizItem.Id));
    }

    public void Dispose()
    {
        _context.Dispose();
        _connection.Close();
    }
}
