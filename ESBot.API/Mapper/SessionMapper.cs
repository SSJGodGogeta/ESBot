using ESBot.API.Interfaces;
using ESBot.Domain.Contracts.Session;
using ESBot.Domain.Entities;

namespace ESBot.API.Mapper;

public class SessionMapper
    : IMapper<
        CreateSessionDto,
        UpdateSessionDto,
        SessionDto,
        Session>
{
    public Session ToEntity(CreateSessionDto dto)
    {
        return new Session
        {
            Id = Guid.NewGuid(),
            UserId = dto.UserId,
            Title = dto.Title
        };
    }

    public void ApplyUpdate(Session entity, UpdateSessionDto dto)
    {
        if (dto.EndedAt.HasValue)
            entity.EndedAt = dto.EndedAt.Value;
        entity.Title = dto.Title;
    }

    public SessionDto ToDto(Session entity)
    {
        return new SessionDto
        {
            Id = entity.Id,
            UserId = entity.UserId,
            StartedAt = entity.StartedAt,
            EndedAt = entity.EndedAt,
            MessageCount = entity.Messages.Count,
            QuizRequestCount = entity.QuizRequests.Count,
            Title = entity.Title
        };
    }
}