using ESBot.API.Filter.Entities;
using ESBot.Domain.Entities;
using ESBot.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;

namespace ESBot.API.Controllers.v1;

[Route("/v1/[controller]")]
[ApiController]
public class QuizRequestController(EsBotDbContext context): BaseController<QuizRequest>(context), IController<QuizRequest, QuizRequestFilter>
{
 
    [HttpGet("filter")]
    public Task<IActionResult> Filter([FromQuery] QuizRequestFilter filter) => base.FilterEntities(filter);
    
    [HttpPost]
    public IActionResult Create([FromBody] QuizRequest quizRequest) => base.CreateEntityAndRespond(quizRequest);

    
    [HttpDelete]
    public IActionResult Delete(int id) => base.DeleteEntityAndRespond(id);

    
    [HttpPut]
    public IActionResult Update(int id, [FromBody] QuizRequest quizRequest) => base.UpdateEntityAndRespond(id, quizRequest);
    
}