using ESBot.API.Interfaces;
using ESBot.Domain.Contracts.QuizRequest;
using ESBot.Domain.Entities;

namespace ESBot.API.Mapper;

public class QuizRequestMapper
    : IMapper<
        CreateQuizRequestDto,
        UpdateQuizRequestDto,
        QuizRequestDto,
        QuizRequest>
{
    public QuizRequest ToEntity(CreateQuizRequestDto dto)
    {
        return new QuizRequest
        {
            Id = Guid.NewGuid(),
            SessionId = dto.SessionId,
            Topic = dto.Topic,
            Difficulty = dto.Difficulty
        };
    }

    public void ApplyUpdate(QuizRequest entity, UpdateQuizRequestDto dto)
    {
        if (dto.Topic is not null)
            entity.Topic = dto.Topic;

        if (dto.Difficulty.HasValue)
            entity.Difficulty = dto.Difficulty.Value;
    }

    public QuizRequestDto ToDto(QuizRequest entity)
    {
        return new QuizRequestDto
        {
            Id = entity.Id,
            SessionId = entity.SessionId,
            Topic = entity.Topic,
            Difficulty = entity.Difficulty,
            CreatedAt = entity.CreatedAt,
            QuizItemCount = entity.QuizItems.Count
        };
    }
}