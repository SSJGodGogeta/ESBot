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
public class QuizRequestController(EsBotDbContext context,
    IMapper<CreateQuizRequestDto, UpdateQuizRequestDto, QuizRequestDto, QuizRequest> mapper) 
    : BaseController<QuizRequest, CreateQuizRequestDto, UpdateQuizRequestDto, QuizRequestDto>(context, mapper), 
        IController<QuizRequest, QuizRequestFilter, CreateQuizRequestDto, UpdateQuizRequestDto>
{
 
    [HttpGet]
    public Task<IActionResult> Filter([FromQuery] QuizRequestFilter filter) => base.FilterEntities(filter);
    
    [HttpPost]
    public IActionResult Create([FromBody] CreateQuizRequestDto quizRequest) => base.CreateEntityAndRespond(quizRequest);

    
    [HttpDelete]
    public IActionResult Delete(Guid id) => base.DeleteEntityAndRespond(id);

    
    [HttpPut]
    public IActionResult Update(Guid id, [FromBody] UpdateQuizRequestDto quizRequest) => base.UpdateEntityAndRespond(id, quizRequest);
    
}