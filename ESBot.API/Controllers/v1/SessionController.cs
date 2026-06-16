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
public class SessionController(EsBotDbContext context,
    IMapper<CreateSessionDto, UpdateSessionDto, SessionDto, Session> mapper) 
    : BaseController<Session, CreateSessionDto, UpdateSessionDto, SessionDto>(context, mapper), 
        IController<Session, SessionFilter, CreateSessionDto, UpdateSessionDto>
{
 
    [HttpGet]
    public Task<IActionResult> Filter([FromQuery] SessionFilter filter) => base.FilterEntities(filter);
    
    [HttpPost]
    public IActionResult Create([FromBody] CreateSessionDto session) => base.CreateEntityAndRespond(session);

    
    [HttpDelete]
    public IActionResult Delete(Guid id) => base.DeleteEntityAndRespond(id);

    
    [HttpPut]
    public IActionResult Update(Guid id, [FromBody] UpdateSessionDto session) => base.UpdateEntityAndRespond(id, session);
    
}