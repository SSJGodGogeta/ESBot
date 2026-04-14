using ESBot.API.Filter.Entities;
using ESBot.Domain.Entities;
using ESBot.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;

namespace ESBot.API.Controllers.v1;

[Route("/v1/[controller]")]
[ApiController]
public class QuizItemsController(EsBotDbContext context): BaseController<QuizItem>(context), IController<QuizItem, QuizItemFilter>
{
 
    [HttpGet]
    public Task<IActionResult> Filter([FromQuery] QuizItemFilter filter) => base.FilterEntities(filter);
    
    [HttpPost]
    public IActionResult Create([FromBody] QuizItem quizItem) => base.CreateEntityAndRespond(quizItem);

    
    [HttpDelete("{id:guid}")]
    public IActionResult Delete(Guid id) => base.DeleteEntityAndRespond(id);

    
    [HttpPut("{id:guid}")]
    public IActionResult Update(Guid id, [FromBody] QuizItem quizItem) => base.UpdateEntityAndRespond(id, quizItem);
    
}