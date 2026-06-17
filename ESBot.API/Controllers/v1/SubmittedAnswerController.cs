using ESBot.API.Filter.Entities;
using ESBot.API.Interfaces;
using ESBot.API.Mapper;
using ESBot.Domain.Contracts.SubmittedAnswer;
using ESBot.Domain.Entities;
using ESBot.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;

namespace ESBot.API.Controllers.v1;

[Route("/v1/[controller]")]
[ApiController]
public class SubmittedAnswerController(EsBotDbContext context,
    IMapper<CreateSubmittedAnswerDto, UpdateSubmittedAnswerDto, SubmittedAnswerDto, SubmittedAnswer> mapper) 
    : BaseController<SubmittedAnswer, CreateSubmittedAnswerDto, UpdateSubmittedAnswerDto, SubmittedAnswerDto>(context, mapper), 
        IController<SubmittedAnswer, SubmittedAnswerFilter, CreateSubmittedAnswerDto, UpdateSubmittedAnswerDto>
{
 
    [HttpGet]
    public async Task<IActionResult> Filter([FromQuery] SubmittedAnswerFilter filter, [FromQuery] int page = 1, [FromQuery] int pageSize = 50) => await base.FilterEntities(filter, page, pageSize);
    
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateSubmittedAnswerDto submittedAnswer) => await base.CreateEntityAndRespond(submittedAnswer);

    
    [HttpDelete]
    public async Task<IActionResult> Delete(Guid id) => await base.DeleteEntityAndRespond(id);

    
    [HttpPut]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateSubmittedAnswerDto submittedAnswer) => await base.UpdateEntityAndRespond(id, submittedAnswer);
    
}