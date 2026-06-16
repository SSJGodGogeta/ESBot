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
    public Task<IActionResult> Filter([FromQuery] SubmittedAnswerFilter filter) => base.FilterEntities(filter);
    
    [HttpPost]
    public IActionResult Create([FromBody] CreateSubmittedAnswerDto submittedAnswer) => base.CreateEntityAndRespond(submittedAnswer);

    
    [HttpDelete]
    public IActionResult Delete(Guid id) => base.DeleteEntityAndRespond(id);

    
    [HttpPut]
    public IActionResult Update(Guid id, [FromBody] UpdateSubmittedAnswerDto submittedAnswer) => base.UpdateEntityAndRespond(id, submittedAnswer);
    
}