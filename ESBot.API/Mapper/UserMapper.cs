using ESBot.API.Interfaces;
using ESBot.Domain.Contracts.User;
using ESBot.Domain.Entities;

namespace ESBot.API.Mapper;

public class UserMapper : IMapper<CreateUserDto, UpdateUserDto, UserDto, User>
{
    public User ToEntity(CreateUserDto dto)
    {
        return new User
        {
            Id = Guid.NewGuid(),
            Username = dto.Username,
            Email = dto.Email,
            HashedPassword = dto.Password
        };
    }

    public void ApplyUpdate(User entity, UpdateUserDto dto)
    {
        if (dto.Username is not null)
            entity.Username = dto.Username;

        if (dto.Email is not null)
            entity.Email = dto.Email;

        if (dto.Password is not null)
            entity.HashedPassword = dto.Password;
    }

    public UserDto ToDto(User entity)
    {
        return new UserDto
        {
            Id = entity.Id,
            Username = entity.Username,
            Email = entity.Email,
            CreatedAt = entity.CreatedAt,
            SessionCount = entity.Sessions.Count
        };
    }
}