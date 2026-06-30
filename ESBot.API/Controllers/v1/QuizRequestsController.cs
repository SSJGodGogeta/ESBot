using ESBot.API.Filter.Entities;
using ESBot.API.Interfaces;
using ESBot.API.Mapper;
using ESBot.Domain.Contracts.QuizRequest;
using ESBot.Domain.Entities;
using ESBot.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;

namespace ESBot.API.Controllers.v1;

[Route("/v1/[controller]")]
[ApiController]
public class QuizRequestsController(EsBotDbContext context,
    IMapper<CreateQuizRequestDto, UpdateQuizRequestDto, QuizRequestDto, QuizRequest> mapper) 
    : BaseController<QuizRequest, CreateQuizRequestDto, UpdateQuizRequestDto, QuizRequestDto>(context, mapper), 
        IController<QuizRequest, QuizRequestFilter, CreateQuizRequestDto, UpdateQuizRequestDto>
{
 
    [HttpGet]
    public async Task<IActionResult> Filter([FromQuery] QuizRequestFilter filter, [FromQuery] int page = 1, [FromQuery] int pageSize = 50) => await base.FilterEntities(filter, page, pageSize);
    
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateQuizRequestDto quizRequest) => await base.CreateEntityAndRespond(quizRequest);

    
    [HttpDelete]
    public async Task<IActionResult> Delete(Guid id) => await base.DeleteEntityAndRespond(id);

    
    [HttpPut]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateQuizRequestDto quizRequest) => await base.UpdateEntityAndRespond(id, quizRequest);
    
}