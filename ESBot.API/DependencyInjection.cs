using ESBot.API.Interfaces;
using ESBot.API.Mapper;
using ESBot.Domain.Contracts.EvaluationResult;
using ESBot.Domain.Contracts.Message;
using ESBot.Domain.Contracts.QuizItem;
using ESBot.Domain.Contracts.QuizRequest;
using ESBot.Domain.Contracts.Session;
using ESBot.Domain.Contracts.SubmittedAnswer;
using ESBot.Domain.Contracts.User;
using ESBot.Domain.Entities;

namespace ESBot.API;

public static class DependencyInjection
{
    public static IServiceCollection AddMappers(this IServiceCollection services)
    {
        services.AddScoped<IMapper<CreateEvaluationResultDto, UpdateEvaluationResultDto, EvaluationResultDto, EvaluationResult>, EvaluationResultMapper>();
        services.AddScoped<IMapper<CreateMessageDto, UpdateMessageDto, MessageDto, Message>, MessageMapper>();
        services.AddScoped<IMapper<CreateQuizItemDto, UpdateQuizItemDto, QuizItemDto, QuizItem>, QuizItemMapper>();
        services.AddScoped<IMapper<CreateQuizRequestDto, UpdateQuizRequestDto, QuizRequestDto, QuizRequest>, QuizRequestMapper>();
        services.AddScoped<IMapper<CreateSessionDto, UpdateSessionDto, SessionDto, Session>, SessionMapper>();
        services.AddScoped<IMapper<CreateSubmittedAnswerDto, UpdateSubmittedAnswerDto, SubmittedAnswerDto, SubmittedAnswer>, SubmittedAnswerMapper>();
        services.AddScoped<IMapper<CreateUserDto, UpdateUserDto, UserDto, User>, UserMapper>();

        return services;
    }
}