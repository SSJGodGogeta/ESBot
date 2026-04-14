using ESBot.API.Filter;
using Microsoft.AspNetCore.Mvc;

namespace ESBot.API.Controllers.v1;

public interface IController<TEntity, TFilter> where TFilter : IEntityFilter<TEntity>
{
    Task<IActionResult> Filter([FromQuery] TFilter filter);
    IActionResult Create([FromBody] TEntity evaluationResult);
    IActionResult Delete(int id);
    IActionResult Update(int id, [FromBody] TEntity entity);
}