using ESBot.API.Interfaces;
using ESBot.Domain.Contracts.SubmittedAnswer;
using ESBot.Domain.Entities;

namespace ESBot.API.Mapper;

public class SubmittedAnswerMapper
    : IMapper<
        CreateSubmittedAnswerDto,
        UpdateSubmittedAnswerDto,
        SubmittedAnswerDto,
        SubmittedAnswer>
{
    public SubmittedAnswer ToEntity(CreateSubmittedAnswerDto dto)
    {
        return new SubmittedAnswer
        {
            Id = Guid.NewGuid(),
            QuizItemId = dto.QuizItemId,
            Answer = dto.Answer
        };
    }

    public void ApplyUpdate(
        SubmittedAnswer entity,
        UpdateSubmittedAnswerDto dto)
    {
        if (dto.Answer is not null)
            entity.Answer = dto.Answer;
    }

    public SubmittedAnswerDto ToDto(SubmittedAnswer entity)
    {
        return new SubmittedAnswerDto
        {
            Id = entity.Id,
            QuizItemId = entity.QuizItemId,
            Answer = entity.Answer,
            SubmittedAt = entity.SubmittedAt,
            IsEvaluated = entity.EvaluationResult is not null
        };
    }
}