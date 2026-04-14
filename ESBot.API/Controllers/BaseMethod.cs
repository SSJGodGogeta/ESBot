namespace ESBot.API.Controllers.v1;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

public abstract partial class BaseController<TEntity> where TEntity : class, new()
{
    protected virtual TEntity? GetEntityById(Guid id) => DbSet.Find(id);
    protected virtual (bool, Exception?) CreateAndSaveEntity(TEntity? entity)
    {
        try
        {
            if(entity == null) return (false, new ArgumentNullException(nameof(entity)));
            DbSet.Add(entity);
            Context.SaveChanges();
        }
        catch (Exception e)
        {
            return (false, e);
        }
        return (true, null);
    }
    protected virtual (bool, Exception?) DeleteEntityAndSave(TEntity entity)
    {
        try
        {
            DbSet.Remove(entity);
            Context.SaveChanges();
        }
        catch (Exception e)
        {
            return (false, e);
        }
        return (true, null);
    }
    protected virtual (bool, Exception?) UpdateEntityAndSave(TEntity? existingEntity, TEntity updatedEntity)
    {
        try
        {
            if(existingEntity is null) return (false, new ArgumentNullException(nameof(existingEntity)));
            Context.Entry(existingEntity).CurrentValues.SetValues(updatedEntity);
            Context.SaveChanges();
        }
        catch (Exception e)
        {
            return (false, e);
        }
        return (true, null);
    }
    
    protected virtual List<TEntity> GetAllEntities() => DbSet.ToList();
    
    protected virtual TEntity? GetOneEntityByPredicate(Expression<Func<TEntity, bool>> predicate)
    {
        TEntity? entity = DbSet.FirstOrDefault(predicate);
        return entity;
    }
    protected virtual List<TEntity> GetAllEntitiesByPredicate(Expression<Func<TEntity, bool>> predicate) => DbSet.Where(predicate).ToList();

}