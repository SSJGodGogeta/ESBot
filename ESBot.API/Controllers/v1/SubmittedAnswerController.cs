using ESBot.API.Filter.Entities;
using ESBot.Domain.Entities;
using ESBot.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;

namespace ESBot.API.Controllers.v1;

[Route("/v1/[controller]")]
[ApiController]
public class SubmittedAnswersController(EsBotDbContext context): BaseController<SubmittedAnswer>(context), IController<SubmittedAnswer, SubmittedAnswerFilter>
{
 
    [HttpGet]
    public Task<IActionResult> Filter([FromQuery] SubmittedAnswerFilter filter) => base.FilterEntities(filter);
    
    [HttpPost]
    public IActionResult Create([FromBody] SubmittedAnswer submittedAnswer) => base.CreateEntityAndRespond(submittedAnswer);

    
    [HttpDelete]
    public IActionResult Delete(Guid id) => base.DeleteEntityAndRespond(id);

    
    [HttpPut]
    public IActionResult Update(Guid id, [FromBody] SubmittedAnswer submittedAnswer) => base.UpdateEntityAndRespond(id, submittedAnswer);
    
}