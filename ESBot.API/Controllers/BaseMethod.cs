using System.Linq.Expressions;
using ESBot.Domain.Interfaces;

namespace ESBot.API.Controllers;

public abstract partial class BaseController<TEntity> where TEntity : class, new()
{
    protected TEntity? GetEntityById(Guid id) => DbSet.Find(id);

    protected (bool, Exception?) CreateAndSaveEntity(TEntity? entity)
    {
        try
        {
            if (entity == null) return (false, new ArgumentNullException(nameof(entity)));
            DbSet.Add(entity);
            Context.SaveChanges();
        }
        catch (Exception e)
        {
            return (false, e);
        }

        return (true, null);
    }

    protected (bool, Exception?) DeleteEntityAndSave(TEntity entity)
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

    protected (bool, Exception?) UpdateEntityAndSave(TEntity? existingEntity, TEntity updatedEntity)
    {
        try
        {
            if (existingEntity is null) return (false, new ArgumentNullException(nameof(existingEntity)));
            var entry = Context.Entry(existingEntity);
            entry.CurrentValues.SetValues(updatedEntity);
            if (existingEntity is IImmutableProperties immutable)
            {
                foreach (var prop in immutable.GetImmutableProperties())
                {
                    entry.Property(prop).IsModified = false;
                }
            }
            Context.SaveChanges();
        }
        catch (Exception e)
        {
            return (false, e);
        }

        return (true, null);
    }
}