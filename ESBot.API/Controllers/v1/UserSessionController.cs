using ESBot.API.Filter.Entities;
using ESBot.Domain.Entities;
using ESBot.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;

namespace ESBot.API.Controllers.v1;

[Route("/v1/[controller]")]
[ApiController]
public class SessionsController(EsBotDbContext context): BaseController<Session>(context), IController<Session, SessionFilter>
{
 
    [HttpGet]
    public Task<IActionResult> Filter([FromQuery] SessionFilter filter) => base.FilterEntities(filter);
    
    [HttpPost]
    public IActionResult Create([FromBody] Session session) => base.CreateEntityAndRespond(session);

    
    [HttpDelete]
    public IActionResult Delete(Guid id) => base.DeleteEntityAndRespond(id);

    
    [HttpPut]
    public IActionResult Update(Guid id, [FromBody] Session session) => base.UpdateEntityAndRespond(id, session);
    
}