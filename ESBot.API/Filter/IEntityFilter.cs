namespace ESBot.API.Filter;

/// <summary>
/// Used to apply a query filter for the GET routes 
/// </summary>
/// <typeparam name="TEntity">The Entity in the database</typeparam>
public interface IEntityFilter<TEntity>
{
    Task<List<TEntity>> Apply(IQueryable<TEntity> query);
}