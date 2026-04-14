using System.ComponentModel.DataAnnotations;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using ESBot.Domain.Entities;
using ESBot.Infrastructure.Data;

namespace ESBot.Tests;

public class UserEntityTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly EsBotDbContext _context;

    public UserEntityTests()
    {
        // Setup SQLite In-Memory connection
        _connection = new SqliteConnection("Filename=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<EsBotDbContext>()
            .UseSqlite(_connection)
            .Options;

        _context = new EsBotDbContext(options);
        _context.Database.EnsureCreated();
    }

    [Fact]
    public void User_Creation_WithValidData_ShouldSucceed()
    {
        // Arrange
        var user = new User
        {
            Username = "TestUser",
            Email = "test@example.com",
            HashedPassword = "$2a$12$nW5uUChi/tFaVUfkorghl.oc9C/O5mqcRkOM/MRkDbnpDCoghXmH2",
        };

        // Act
        _context.Users.Add(user);
        _context.SaveChanges();

        // Assert
        var savedUser = _context.Users.FirstOrDefault(u => u.Username == "TestUser");
        Assert.NotNull(savedUser);
        Assert.Equal("test@example.com", savedUser.Email);
        Assert.True(savedUser.CreatedAt <= DateTime.UtcNow);
    }

    [Fact]
    public void User_Validation_MissingRequiredFields_ShouldFail()
    {
        // Arrange
        var user = new User { Username = null! }; // Missing Email, Username and Hashed Password

        // Act & Assert
        var validationContext = new ValidationContext(user);
        var validationResults = new List<ValidationResult>();
        
        bool isValid = Validator.TryValidateObject(user, validationContext, validationResults, true);

        Assert.False(isValid);
        Assert.Contains(validationResults, v => v.MemberNames.Contains("Username"));
        Assert.Contains(validationResults, v => v.MemberNames.Contains("Email"));
        Assert.Contains(validationResults, v => v.MemberNames.Contains("HashedPassword"));
    }

    [Fact]
    public void User_Relationships_SessionAssociation_ShouldBeConsistent()
    {
        // Arrange
        var user = new User
        {
            Username = "Kevin",
            Email = "kevin@test.com",
            HashedPassword = "$2a$12$nW5uUChi/tFaVUfkorghl.oc9C/O5mqcRkOM/MRkDbnpDCoghXmH2",
        };
        var session = new UserSession 
        { 
            User = user, 
            StartedAt = DateTime.UtcNow 
        };

        // Act
        user.Sessions.Add(session);
        _context.Users.Add(user);
        _context.SaveChanges();

        // Assert
        var retrievedUser = _context.Users
            .Include(u => u.Sessions)
            .First(u => u.Username == "Kevin");

        Assert.Single(retrievedUser.Sessions);
        Assert.Equal(retrievedUser.Id, retrievedUser.Sessions[0].UserId);
    }

    public void Dispose()
    {
        _context.Dispose();
        _connection.Close();
    }
}