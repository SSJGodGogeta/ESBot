using ESBot.API.Filter.Entities;
using ESBot.Domain.Entities;
using ESBot.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;

namespace ESBot.API.Controllers.v1;

[Route("/v1/[controller]")]
[ApiController]
public class EvaluationResultsController(EsBotDbContext context)
    : BaseController<EvaluationResult>(context), IController<EvaluationResult, EvaluationResultFilter>
{
 
    [HttpGet]
    public Task<IActionResult> Filter([FromQuery] EvaluationResultFilter filter) => base.FilterEntities(filter);
    
    [HttpPost]
    public IActionResult Create([FromBody] EvaluationResult evaluationResult) => base.CreateEntityAndRespond(evaluationResult);

    
    [HttpDelete]
    public IActionResult Delete(int id) => base.DeleteEntityAndRespond(id);

    
    [HttpPut]
    public IActionResult Update(int id, [FromBody] EvaluationResult evaluationResult) => base.UpdateEntityAndRespond(id, evaluationResult);
    
}