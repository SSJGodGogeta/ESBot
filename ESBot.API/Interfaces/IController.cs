using ESBot.API.Filter;
using ESBot.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ESBot.API.Interfaces;

public interface IController<TEntity, TFilter, TCreateDto, TUpdateDto>
    where TEntity : class
    where TFilter : IEntityFilter<TEntity>
    where TCreateDto : ICreateDto
    where TUpdateDto : IUpdateDto
{
    Task<IActionResult> Filter([FromQuery] TFilter filter, [FromQuery] int page, [FromQuery] int pageSize);
    Task<IActionResult> Create([FromBody] TCreateDto dto);
    Task<IActionResult> Delete(Guid id);
    Task<IActionResult> Update(Guid id, [FromBody] TUpdateDto dto);
}