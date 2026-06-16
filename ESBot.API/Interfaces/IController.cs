using ESBot.API.Filter;
using ESBot.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ESBot.API.Controllers;

public interface IController<TEntity, TFilter, TCreateDto, TUpdateDto>
    where TEntity : class
    where TFilter : IEntityFilter<TEntity>
    where TCreateDto : ICreateDto
    where TUpdateDto : IUpdateDto
{
    Task<IActionResult> Filter([FromQuery] TFilter filter);
    IActionResult Create([FromBody] TCreateDto dto);
    IActionResult Delete(Guid id);
    IActionResult Update(Guid id, [FromBody] TUpdateDto dto);
}