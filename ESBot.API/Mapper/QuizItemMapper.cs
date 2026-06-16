using ESBot.API.Interfaces;
using ESBot.Domain.Contracts.QuizItem;
using ESBot.Domain.Entities;

namespace ESBot.API.Mapper;

public class QuizItemMapper
    : IMapper<
        CreateQuizItemDto,
        UpdateQuizItemDto,
        QuizItemDto,
        QuizItem>
{
    public QuizItem ToEntity(CreateQuizItemDto dto)
    {
        return new QuizItem
        {
            Id = Guid.NewGuid(),
            QuizRequestId = dto.QuizRequestId,
            Question = dto.Question,
            CorrectAnswer = dto.CorrectAnswer
        };
    }

    public void ApplyUpdate(QuizItem entity, UpdateQuizItemDto dto)
    {
        if (dto.Question is not null)
            entity.Question = dto.Question;

        if (dto.CorrectAnswer is not null)
            entity.CorrectAnswer = dto.CorrectAnswer;
    }

    public QuizItemDto ToDto(QuizItem entity)
    {
        return new QuizItemDto
        {
            Id = entity.Id,
            QuizRequestId = entity.QuizRequestId,
            Question = entity.Question,
            CorrectAnswer = entity.CorrectAnswer,
            SubmittedAnswerCount = entity.SubmittedAnswers.Count
        };
    }
}