using ESBot.API.Interfaces;
using ESBot.Domain.Contracts.EvaluationResult;
using ESBot.Domain.Entities;

namespace ESBot.API.Mapper;

public class EvaluationResultMapper
    : IMapper<
        CreateEvaluationResultDto,
        UpdateEvaluationResultDto,
        EvaluationResultDto,
        EvaluationResult>
{
    public EvaluationResult ToEntity(CreateEvaluationResultDto dto)
    {
        return new EvaluationResult
        {
            Id = Guid.NewGuid(),
            SubmittedAnswerId = dto.SubmittedAnswerId,
            IsCorrect = dto.IsCorrect,
            Score = dto.Score,
            Feedback = dto.Feedback
        };
    }

    public void ApplyUpdate(EvaluationResult entity, UpdateEvaluationResultDto dto)
    {
        if (dto.IsCorrect.HasValue)
            entity.IsCorrect = dto.IsCorrect.Value;

        if (dto.Score.HasValue)
            entity.Score = dto.Score.Value;

        if (dto.Feedback is not null)
            entity.Feedback = dto.Feedback;
    }

    public EvaluationResultDto ToDto(EvaluationResult entity)
    {
        return new EvaluationResultDto
        {
            Id = entity.Id,
            SubmittedAnswerId = entity.SubmittedAnswerId,
            IsCorrect = entity.IsCorrect,
            Score = entity.Score,
            Feedback = entity.Feedback
        };
    }
}