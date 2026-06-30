using ESBot.API.Filter.Entities;
using ESBot.API.Interfaces;
using ESBot.API.Mapper;
using ESBot.Domain.Contracts.Session;
using ESBot.Domain.Entities;
using ESBot.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;

namespace ESBot.API.Controllers.v1;

[Route("/v1/[controller]")]
[ApiController]
public class SessionsController(EsBotDbContext context,
    IMapper<CreateSessionDto, UpdateSessionDto, SessionDto, Session> mapper) 
    : BaseController<Session, CreateSessionDto, UpdateSessionDto, SessionDto>(context, mapper), 
        IController<Session, SessionFilter, CreateSessionDto, UpdateSessionDto>
{
 
    [HttpGet]
    public async Task<IActionResult> Filter([FromQuery] SessionFilter filter, [FromQuery] int page = 1, [FromQuery] int pageSize = 50) => await base.FilterEntities(filter, page, pageSize);
    
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateSessionDto session) => await base.CreateEntityAndRespond(session);

    
    [HttpDelete]
    public async Task<IActionResult> Delete(Guid id) => await base.DeleteEntityAndRespond(id);

    
    [HttpPut]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateSessionDto session) => await base.UpdateEntityAndRespond(id, session);
    
}