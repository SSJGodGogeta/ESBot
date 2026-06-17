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
    public async Task<IActionResult> Filter([FromQuery] QuizItemFilter filter, [FromQuery] int page = 1, [FromQuery] int pageSize = 50) => await base.FilterEntities(filter, page, pageSize);
    
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateQuizItemDto quizItem) => await base.CreateEntityAndRespond(quizItem);

    
    [HttpDelete]
    public async Task<IActionResult> Delete(Guid id) => await base.DeleteEntityAndRespond(id);

    
    [HttpPut]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateQuizItemDto quizItem) => await base.UpdateEntityAndRespond(id, quizItem);
    
}