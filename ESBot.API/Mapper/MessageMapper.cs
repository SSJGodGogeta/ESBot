using ESBot.API.Interfaces;
using ESBot.Domain.Contracts.Message;
using ESBot.Domain.Entities;

namespace ESBot.API.Mapper;

public class MessageMapper
    : IMapper<
        CreateMessageDto,
        UpdateMessageDto,
        MessageDto,
        Message>
{
    public Message ToEntity(CreateMessageDto dto)
    {
        return new Message
        {
            Id = Guid.NewGuid(),
            SessionId = dto.SessionId,
            Content = dto.Content,
            Role = dto.Role
        };
    }

    public void ApplyUpdate(Message entity, UpdateMessageDto dto)
    {
        if (dto.Content is not null)
            entity.Content = dto.Content;
    }

    public MessageDto ToDto(Message entity)
    {
        return new MessageDto
        {
            Id = entity.Id,
            SessionId = entity.SessionId,
            Content = entity.Content,
            Role = entity.Role,
            CreatedAt = entity.CreatedAt
        };
    }
}