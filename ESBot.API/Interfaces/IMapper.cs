using ESBot.Domain.Interfaces;

namespace ESBot.API.Interfaces;

public interface IMapper<TCreateDto, TUpdateDto, TDto, TEntity> where TCreateDto : ICreateDto
    where TUpdateDto : IUpdateDto
    where TDto : IDto
{
    /// <summary>
    /// Maps a CreateDto to an Entity. The Id of the Entity should be generated in this method.
    /// </summary>
    /// <param name="dto"></param>
    /// <returns>A new Entity instance</returns>
    TEntity ToEntity(TCreateDto dto);

    /// <summary>
    /// Applies the values from the UpdateDto to the existing Entity. The Id of the Entity should not be modified.
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="dto"></param>
    void ApplyUpdate(TEntity entity, TUpdateDto dto);

    /// <summary>
    /// Maps an Entity to a Dto.
    /// </summary>
    /// <param name="entity"></param>
    /// <returns>A new Dto instance</returns>
    TDto ToDto(TEntity entity);
}