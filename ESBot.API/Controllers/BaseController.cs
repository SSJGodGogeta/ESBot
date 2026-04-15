using System.Linq.Expressions;
using ESBot.API.Filter;
using ESBot.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ESBot.API.Controllers;


public abstract partial class BaseController<TEntity>(EsBotDbContext context) : ControllerBase
    where TEntity : class, new()
{
    protected readonly EsBotDbContext Context = context;
    protected readonly DbSet<TEntity> DbSet = context.Set<TEntity>();

    ///////////////////////////////////////////////////////////////////////////////////////////////////
    //////////////////////////////     Create/Retrieve/Update/Delete     //////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////////////////////////

    
    /// <summary>
    /// Creates a new entity in the database.
    /// </summary>
    /// <param name="entity">The Entity to be created, retrieved from the body</param>
    /// <returns>An IActionResult indicating the outcome of the creation request.</returns>
    protected IActionResult CreateEntityAndRespond([FromBody] TEntity entity)
    {
        try
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = CreateAndSaveEntity(entity);
            if(result.Item1) return StatusCode(StatusCodes.Status201Created, entity);
            return StatusCode(409, $"Could not create entity of type {typeof(TEntity).Name}\nAn Exception occurred: Type - {result.Item2!.GetType()}, Message - {result.Item2.Message}");
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
    /// <param name="updatedEntity">The new entity data containing the updated scalar values.</param>
    protected IActionResult UpdateEntityAndRespond(Guid id, [FromBody] TEntity updatedEntity)
    {
        try
        {
            if (id == Guid.Empty) return BadRequest($"{typeof(TEntity).Name} ID must not be empty.");
            if (!ModelState.IsValid) return BadRequest(ModelState);
            TEntity? existingEntity = GetEntityById(id);
            if (existingEntity == null) return NotFound($"{typeof(TEntity).Name} with ID {id} not found.");
        
            var idProp = typeof(TEntity).GetProperties().FirstOrDefault(p =>
                p.Name.Equals("Id", StringComparison.OrdinalIgnoreCase) ||
                p.Name.EndsWith("Id", StringComparison.OrdinalIgnoreCase)
            );
            if (idProp == null)
                return BadRequest("Could not identify ID property on entity. Please check the name and the naming conventions");
            var updatedIdValue = idProp.GetValue(updatedEntity);
            if (updatedIdValue is not Guid updatedGuid)
                return BadRequest("The entity ID must be a Guid.");
            if (id != updatedGuid)
                return BadRequest($"The ID in the body does not match the ID in the URL. Body ID/URL ID: {updatedIdValue}/{id}");

            var result = UpdateEntityAndSave(existingEntity, updatedEntity);
            updatedEntity = GetEntityById(id)!;
            if(result.Item1) return Ok(updatedEntity);
            return StatusCode(409, $"Could not update entity of type {typeof(TEntity).Name}\nAn Exception occurred: Type - {result.Item2!.GetType()}, Message - {result.Item2.Message}");

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
