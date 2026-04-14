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

    
    [HttpDelete]
    public IActionResult Delete(int id) => base.DeleteEntityAndRespond(id);

    
    [HttpPut]
    public IActionResult Update(int id, [FromBody] QuizItem quizItem) => base.UpdateEntityAndRespond(id, quizItem);
    
}