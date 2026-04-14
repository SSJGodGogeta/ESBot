using System.Linq.Expressions;
using ESBot.API.Filter;
using ESBot.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ESBot.API.Controllers.v1;


public abstract partial class BaseController<TEntity>(EsBotDbContext context) : ControllerBase
    where TEntity : class, new()
{
    protected readonly EsBotDbContext Context = context;
    protected readonly DbSet<TEntity> DbSet = context.Set<TEntity>();

    /// <summary>
    /// This is a dummy function to implement auth and responses. Replace this with the official methods when using KeyCloak.
    /// </summary>
    /// <param name="bypassAuth">If the authorization should be skipped</param>
    /// <returns>true, if the user is authorized. Otherwise false</returns>
    protected bool IsAuthorized(bool bypassAuth) => bypassAuth || new Random().Next() % 2 == 0;
    
    /// <summary>
    /// This is a dummy function to implement auth and responses. Replace this with the official methods when using KeyCloak.
    /// </summary>
    /// <param name="bypassPerm">If the permission check should be skipped</param>
    /// <returns>true, if the user is authenticated. Otherwise false</returns>
    protected bool IsAllowed(bool bypassPerm) => bypassPerm || new Random().Next() % 2 == 0;
    
    
    ///////////////////////////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////           RETRIEVE           //////////////////////////////////   
    ///////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Retrieves all entities of the TEntity from the database.
    /// </summary>
    /// <param name="bypassAuth">If the authorization should be skipped</param>
    /// <param name="bypassPerm">If the authorization should be skipped</param>
    /// <returns>An IActionResult: if entities are found, it returns them wrapped in an `Ok` response; 
    /// otherwise, it returns a `NotFound` response with an appropriate message.</returns>
    protected virtual IActionResult GetAllEntitiesAndRespond(bool bypassAuth=true, bool bypassPerm=true)
    {
        try
        {
            if (!IsAuthorized(bypassAuth)) return StatusCode(401, "Unauthenticated"); 
            if (!IsAllowed(bypassPerm)) return StatusCode(403, "Forbidden"); 
            List<TEntity> entities = GetAllEntities();
            if(entities.Count == 0) return NotFound($"Could not find any entities of type {typeof(TEntity).Name}");
            return Ok(entities);
        }
        catch (Exception e)
        {
            return StatusCode(500, $"Could not get all entities of type {typeof(TEntity).Name}\nAn Exception occurred: Type - {e.GetType()}, Message - {e.Message}");
        }
    }

    /// <summary>
    /// Retrieves one entity by the given id.
    /// </summary>
    /// <param name="id">The PK as int of an entity</param>
    /// <param name="bypassAuth">If the authorization should be skipped</param>
    /// <param name="bypassPerm">If the authorization should be skipped</param>
    /// <returns>An IActionResult: if a matching entity is found, it returns the entity wrapped in an `Ok` response; 
    /// otherwise, it returns a `NotFound` response with an appropriate message.</returns>
    protected virtual IActionResult GetEntityByIdAndRespond(int id, bool bypassAuth=true, bool bypassPerm=true)
    {
        try
        {
            if (!IsAuthorized(bypassAuth)) return StatusCode(401, "Unauthenticated"); 
            if (!IsAllowed(bypassPerm)) return StatusCode(403, "Forbidden");   
            if (id <= 0) return BadRequest($"{typeof(TEntity).Name} ID must be greater than 0.");
            TEntity? entity = GetEntityById(id);
            if (entity is null) return NotFound($"Could not find {typeof(TEntity).Name} by ID {id}");
            return Ok(entity);
        }
        catch (Exception e)
        {
            return StatusCode(500, $"Could not get entity of type {typeof(TEntity).Name}\nAn Exception occurred: Type - {e.GetType()}, Message - {e.Message}");
        }
    }
    
    /// <summary>
    /// Retrieves the first entity that matches the given filter predicate.
    /// The predicate is a lambda expression used to define the condition for the query.
    /// This method uses Entity Framework's LINQ capabilities to translate the predicate into a SQL query.
    /// </summary>
    /// <param name="predicate">An expression representing the filter condition (a lambda expression) to be applied on the entity.</param>
    /// <param name="bypassAuth">If the authorization should be skipped</param>
    /// <param name="bypassPerm">If the authorization should be skipped</param>
    /// <returns>An IActionResult: if a matching entity is found, it returns the entity wrapped in an `Ok` response; 
    /// otherwise, it returns a `NotFound` response with an appropriate message.</returns>
    protected virtual IActionResult GetOneEntityByPredicateAndRespond(Expression<Func<TEntity, bool>> predicate, bool bypassAuth=true, bool bypassPerm=true)
    {
        try
        {
            if (!IsAuthorized(bypassAuth)) return StatusCode(401, "Unauthenticated"); 
            if (!IsAllowed(bypassPerm)) return StatusCode(403, "Forbidden");   
            TEntity? result = GetOneEntityByPredicate(predicate);
            if (result == null) return NotFound($"Could not find {typeof(TEntity).Name} by the defined arg");
            return Ok(result);
        }
        catch (Exception e)
        {
            return StatusCode(500, $"Could not get entity of type {typeof(TEntity).Name}\nAn Exception occurred: Type - {e.GetType()}, Message - {e.Message}");
        }
    }
    
    /// <summary>
    /// Retrieves all entities that match the given filter predicate.
    /// The predicate is a lambda expression used to define the condition for the query.
    /// This method uses Entity Framework's LINQ capabilities to translate the predicate into a SQL query.
    /// </summary>
    /// <param name="predicate">An expression representing the filter condition (a lambda expression) to be applied on the entity.</param>
    /// <param name="bypassAuth">If the authorization should be skipped</param>
    /// <param name="bypassPerm">If the authorization should be skipped</param>
    /// <returns>An IActionResult: if matching entities are found, it returns them wrapped in an `Ok` response; 
    /// otherwise, it returns a `NotFound` response with an appropriate message.</returns>
    protected virtual IActionResult GetAllEntitiesByPredicateAndRespond(Expression<Func<TEntity, bool>> predicate, bool bypassAuth=true, bool bypassPerm=true)
    {
        try
        {
            if (!IsAuthorized(bypassAuth)) return StatusCode(401, "Unauthenticated"); 
            if (!IsAllowed(bypassPerm)) return StatusCode(403, "Forbidden");   
            List<TEntity> result = GetAllEntitiesByPredicate(predicate);
            if (result.Count == 0) return NotFound($"Could not find any {typeof(TEntity).Name} by the defined arg");
            return Ok(result);
        }
        catch (Exception e)
        {
            return StatusCode(500, $"Could not get all entities of type {typeof(TEntity).Name}\nAn Exception occurred: Type - {e.GetType()}, Message - {e.Message}");
        }
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////     Create/Update/Delete     //////////////////////////////////   
    ///////////////////////////////////////////////////////////////////////////////////////////////////

    
    /// <summary>
    /// Creates a new entity in the database.
    /// </summary>
    /// <param name="entity">The Entity to be created, retrieved from the body</param>
    /// <param name="nameOfId">The name of the attribute that defines the ID (PK)</param>
    /// <param name="getActionNameFunc">A function that returns the action name for the created entity.</param>
    /// <param name="bypassAuth">If the authorization should be skipped</param>
    /// <param name="bypassPerm">If the authorization should be skipped</param>
    /// <returns>An IActionResult indicating the outcome of the creation request.</returns>
    protected virtual IActionResult CreateEntityAndRespond([FromBody] TEntity entity, bool bypassAuth=true, bool bypassPerm=true)
    {
        try
        {
            if (!IsAuthorized(bypassAuth)) return StatusCode(401, "Unauthenticated"); 
            if (!IsAllowed(bypassPerm)) return StatusCode(403, "Forbidden");   
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
    /// <param name="bypassAuth">If the authorization should be skipped</param>
    /// <param name="bypassPerm">If the authorization should be skipped</param>
    /// <returns>An IActionResult indicating the outcome of the creation request.</returns>
    protected virtual IActionResult DeleteEntityAndRespond(int id, bool bypassAuth=true, bool bypassPerm=true)
    {
        try
        {
            if (!IsAuthorized(bypassAuth)) return StatusCode(401, "Unauthenticated"); 
            if (!IsAllowed(bypassPerm)) return StatusCode(403, "Forbidden");   
            if (id <= 0) return BadRequest($"{typeof(TEntity).Name} ID must be greater than 0.");
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
    /// <param name="bypassAuth">If the authorization should be skipped</param>
    /// <param name="bypassPerm">If the authorization should be skipped</param>
    protected virtual IActionResult UpdateEntityAndRespond(int id, [FromBody] TEntity updatedEntity, bool bypassAuth=true, bool bypassPerm=true)
    {
        try
        {
            if (!IsAuthorized(bypassAuth)) return StatusCode(401, "Unauthenticated"); 
            if (!IsAllowed(bypassPerm)) return StatusCode(403, "Forbidden");   
            if (id <= 0) return BadRequest("Invalid ID.");
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            TEntity? existingEntityDto = GetEntityById(id);
            if (existingEntityDto == null) return NotFound($"{typeof(TEntity).Name} with ID {id} not found.");
        
            var idProp = typeof(TEntity).GetProperties().FirstOrDefault(p =>
                p.Name.Equals("Id", StringComparison.OrdinalIgnoreCase) ||
                p.Name.EndsWith("Id", StringComparison.OrdinalIgnoreCase)
            );
            if (idProp == null)
                return BadRequest("Could not identify ID property on entity. Please check the name and the naming conventions");
            var updatedIdValue = idProp.GetValue(updatedEntity);
            if (!id.Equals(updatedIdValue))
                return BadRequest($"The ID in the body does not match the ID in the URL. Body ID/URL ID: {updatedIdValue}/{id}");

            var result = UpdateEntityAndSave(existingEntityDto, updatedEntity);
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