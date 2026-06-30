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
    public async Task<IActionResult> Filter([FromQuery] EvaluationResultFilter filter, [FromQuery] int page = 1, [FromQuery] int pageSize = 50) => await base.FilterEntities(filter, page, pageSize);
    
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateEvaluationResultDto evaluationResult) => await base.CreateEntityAndRespond(evaluationResult);

    
    [HttpDelete]
    public async Task<IActionResult> Delete(Guid id) => await base.DeleteEntityAndRespond(id);

    
    [HttpPut]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateEvaluationResultDto evaluationResult) => await base.UpdateEntityAndRespond(id, evaluationResult);
    
}