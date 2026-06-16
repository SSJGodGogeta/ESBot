using ESBot.API.Filter.Entities;
using ESBot.API.Interfaces;
using ESBot.API.Mapper;
using ESBot.Domain.Contracts.EvaluationResult;
using ESBot.Domain.Entities;
using ESBot.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;

namespace ESBot.API.Controllers.v1;

[Route("/v1/[controller]")]
[ApiController]
public class EvaluationResultsController(EsBotDbContext context,
    IMapper<CreateEvaluationResultDto, UpdateEvaluationResultDto, EvaluationResultDto, EvaluationResult> mapper) 
    : BaseController<EvaluationResult, CreateEvaluationResultDto, UpdateEvaluationResultDto, EvaluationResultDto>(context, mapper), 
        IController<EvaluationResult, EvaluationResultFilter, CreateEvaluationResultDto, UpdateEvaluationResultDto>
{
 
    [HttpGet]
    public Task<IActionResult> Filter([FromQuery] EvaluationResultFilter filter) => base.FilterEntities(filter);
    
    [HttpPost]
    public IActionResult Create([FromBody] CreateEvaluationResultDto evaluationResult) => base.CreateEntityAndRespond(evaluationResult);

    
    [HttpDelete]
    public IActionResult Delete(Guid id) => base.DeleteEntityAndRespond(id);

    
    [HttpPut]
    public IActionResult Update(Guid id, [FromBody] UpdateEvaluationResultDto evaluationResult) => base.UpdateEntityAndRespond(id, evaluationResult);
    
}