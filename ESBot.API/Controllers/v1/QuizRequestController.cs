using ESBot.API.Filter.Entities;
using ESBot.Domain.Entities;
using ESBot.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;

namespace ESBot.API.Controllers.v1;

[Route("/v1/[controller]")]
[ApiController]
public class QuizRequestsController(EsBotDbContext context): BaseController<QuizRequest>(context), IController<QuizRequest, QuizRequestFilter>
{
 
    [HttpGet]
    public Task<IActionResult> Filter([FromQuery] QuizRequestFilter filter) => base.FilterEntities(filter);
    
    [HttpPost]
    public IActionResult Create([FromBody] QuizRequest quizRequest) => base.CreateEntityAndRespond(quizRequest);

    
    [HttpDelete("{id:guid}")]
    public IActionResult Delete(Guid id) => base.DeleteEntityAndRespond(id);

    
    [HttpPut("{id:guid}")]
    public IActionResult Update(Guid id, [FromBody] QuizRequest quizRequest) => base.UpdateEntityAndRespond(id, quizRequest);
    
}