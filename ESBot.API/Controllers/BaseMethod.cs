using System.Linq.Expressions;
using ESBot.Domain.Interfaces;

namespace ESBot.API.Controllers;

public abstract partial class BaseController<TEntity, TCreateDto, TUpdateDto, TDto>
    where TEntity : class, new()
    where TCreateDto : ICreateDto
    where TUpdateDto : IUpdateDto
    where TDto : IDto
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

    protected (bool, Exception?) UpdateEntityAndSave(TEntity? entity)
    {
        try
        {
            if (entity is null) return (false, new ArgumentNullException(nameof(entity)));
            Context.SaveChanges();
        }
        catch (Exception e)
        {
            return (false, e);
        }

        return (true, null);
    }
}