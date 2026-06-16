using System.Linq.Expressions;
using ESBot.API.Filter;
using ESBot.API.Interfaces;
using ESBot.API.Mapper;
using ESBot.Domain.Interfaces;
using ESBot.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ESBot.API.Controllers;


public abstract partial class BaseController<TEntity, TCreateDto, TUpdateDto, TDto>(EsBotDbContext context, IMapper<TCreateDto, TUpdateDto, TDto, TEntity> mapper) : ControllerBase
    where TEntity : class, new()
    where TCreateDto : ICreateDto
    where TUpdateDto : IUpdateDto
    where TDto : IDto
{
    protected readonly EsBotDbContext Context = context;
    protected readonly DbSet<TEntity> DbSet = context.Set<TEntity>();
    
    protected readonly IMapper<TCreateDto, TUpdateDto, TDto, TEntity> Mapper = mapper;

    ///////////////////////////////////////////////////////////////////////////////////////////////////
    //////////////////////////////     Create/Retrieve/Update/Delete     //////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////////////////////////

    
    /// <summary>
    /// Creates a new entity in the database.
    /// </summary>
    /// <param name="dto">The Entity DTO to be created, retrieved from the body</param>
    /// <returns>An IActionResult indicating the outcome of the creation request.</returns>
    protected IActionResult CreateEntityAndRespond([FromBody] TCreateDto dto)
    {
        try
        {
            if (!ModelState.IsValid) return UnprocessableEntity(ModelState);
            var entity = Mapper.ToEntity(dto);
            var result = CreateAndSaveEntity(entity);
            if(!result.Item1) return StatusCode(409, $"Could not create entity of type {typeof(TEntity).Name}\nAn Exception occurred: Type - {result.Item2!.GetType()}, Message - {result.Item2.Message}");
            var dtoResult = Mapper.ToDto(entity);
            return StatusCode(StatusCodes.Status201Created, dtoResult);
        }
        catch (Exception e)
        {
            return StatusCode(500, $"Could not create entity of type {typeof(TEntity).Name}\nAn Exception occurred: Type - {e.GetType()}, Message - {e.Message}");
        }
    }

    /// <summary>
    /// Deletes the entity from the database by its id.
    /// </summary>
    /// <param name="id">The ID of the entity to delete</param>
    /// <returns>An IActionResult indicating the outcome of the creation request.</returns>
    protected IActionResult DeleteEntityAndRespond(Guid id)
    {
        try
        {
            if (id == Guid.Empty) return BadRequest($"{typeof(TEntity).Name} ID must not be empty.");
            TEntity? entity = GetEntityById(id);
            if (entity is null) return NotFound($"Could not find {typeof(TEntity).Name} by ID {id}");
            var result = DeleteEntityAndSave(entity);
            if(result.Item1) return Ok($"Deleted entity with ID: {id}");
            return StatusCode(409, $"Could not delete entity of type {typeof(TEntity).Name}\nAn Exception occurred: Type - {result.Item2!.GetType()}, Message - {result.Item2.Message}");
        }
        catch (Exception e)
        {
            return StatusCode(500, $"Could not delete entity of type {typeof(TEntity).Name}\nAn Exception occurred: Type - {e.GetType()}, Message - {e.Message}");
        }
    }    
    
    /// <summary>
    /// Performs a full update of an entity's scalar (simple) properties by copying values from the provided updated entity.
    /// This method uses Entity Framework Core's change tracker to detect and apply changes to the existing entity instance.
    ///
    /// Only primitive and scalar properties (e.g., int, string, bool) are updated. 
    /// Navigation properties such as collections or related entities (e.g., foreign key relationships, child lists) 
    /// are not modified by this operation. This ensures existing relationships are preserved unless explicitly updated elsewhere.
    ///
    /// </summary>
    /// <param name="id">The id of the entity to update.</param>
    /// <param name="dto">The new entity data containing the updated scalar values.</param>
    protected IActionResult UpdateEntityAndRespond(Guid id, [FromBody] TUpdateDto dto)
    {
        try
        {
            if (id == Guid.Empty) return BadRequest($"{typeof(TEntity).Name} ID must not be empty.");
            if (!ModelState.IsValid) return UnprocessableEntity(ModelState);
            TEntity? existingEntity = GetEntityById(id);
            if (existingEntity == null) return NotFound($"{typeof(TEntity).Name} with ID {id} not found.");

            Mapper.ApplyUpdate(existingEntity, dto);
            
            var result = UpdateEntityAndSave(existingEntity);
            if(!result.Item1) return StatusCode(409, $"Could not update entity of type {typeof(TEntity).Name}\nAn Exception occurred: Type - {result.Item2!.GetType()}, Message - {result.Item2.Message}");

            var dtoResult = Mapper.ToDto(existingEntity);
            return Ok(dtoResult);
        }
        catch (Exception e)
        {
            return StatusCode(500, $"Could not update entity of type {typeof(TEntity).Name}\nAn Exception occurred: Type - {e.GetType()}, Message - {e.Message}");
        }
    }
    
    protected async Task<IActionResult> FilterEntities([FromQuery] IEntityFilter<TEntity> filter)
    {
        var query = DbSet.AsQueryable();
        var entities = await filter.Apply(query);
        return Ok(entities);
    }

}
