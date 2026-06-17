using System.Linq.Expressions;
using ESBot.Domain.Interfaces;

namespace ESBot.API.Controllers;

public abstract partial class BaseController<TEntity, TCreateDto, TUpdateDto, TDto>
    where TEntity : class, new()
    where TCreateDto : ICreateDto
    where TUpdateDto : IUpdateDto
    where TDto : IDto
{
    protected async Task<TEntity?> GetEntityById(Guid id) => await DbSet.FindAsync(id);

    protected async Task<(bool, Exception?)> CreateAndSaveEntityAsync(TEntity? entity)
    {
        try
        {
            if (entity == null) return (false, new ArgumentNullException(nameof(entity)));
            DbSet.Add(entity);
            await Context.SaveChangesAsync();
            return (true, null);
        }
        catch (Exception e)
        {
            return (false, e);
        }
    }

    protected async Task<(bool, Exception?)> DeleteEntityAndSave(TEntity entity)
    {
        try
        {
            DbSet.Remove(entity);
            await Context.SaveChangesAsync();
            return (true, null);
        }
        catch (Exception e)
        {
            return (false, e);
        }
    }

    protected async Task<(bool, Exception?)> UpdateEntityAndSave(TEntity? entity)
    {
        try
        {
            if (entity is null) return (false, new ArgumentNullException(nameof(entity)));
            await Context.SaveChangesAsync();
            return (true, null);

        }
        catch (Exception e)
        {
            return (false, e);
        }
    }
}