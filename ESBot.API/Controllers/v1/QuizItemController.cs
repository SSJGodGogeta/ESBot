using ESBot.API.Filter.Entities;
using ESBot.API.Interfaces;
using ESBot.API.Mapper;
using ESBot.Domain.Contracts.QuizItem;
using ESBot.Domain.Entities;
using ESBot.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;

namespace ESBot.API.Controllers.v1;

[Route("/v1/[controller]")]
[ApiController]
public class QuizItemController(EsBotDbContext context,
    IMapper<CreateQuizItemDto, UpdateQuizItemDto, QuizItemDto, QuizItem> mapper) 
    : BaseController<QuizItem, CreateQuizItemDto, UpdateQuizItemDto, QuizItemDto>(context, mapper), 
        IController<QuizItem, QuizItemFilter, CreateQuizItemDto, UpdateQuizItemDto>
{
 
    [HttpGet]
    public Task<IActionResult> Filter([FromQuery] QuizItemFilter filter) => base.FilterEntities(filter);
    
    [HttpPost]
    public IActionResult Create([FromBody] CreateQuizItemDto quizItem) => base.CreateEntityAndRespond(quizItem);

    
    [HttpDelete]
    public IActionResult Delete(Guid id) => base.DeleteEntityAndRespond(id);

    
    [HttpPut]
    public IActionResult Update(Guid id, [FromBody] UpdateQuizItemDto quizItem) => base.UpdateEntityAndRespond(id, quizItem);
    
}