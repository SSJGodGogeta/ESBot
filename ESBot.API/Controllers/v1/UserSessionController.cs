using ESBot.API.Filter.Entities;
using ESBot.Domain.Entities;
using ESBot.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;

namespace ESBot.API.Controllers.v1;

[Route("/v1/[controller]")]
[ApiController]
public class UserSessionsController(EsBotDbContext context): BaseController<UserSession>(context), IController<UserSession, UserSessionFilter>
{
 
    [HttpGet]
    public Task<IActionResult> Filter([FromQuery] UserSessionFilter filter) => base.FilterEntities(filter);
    
    [HttpPost]
    public IActionResult Create([FromBody] UserSession userSession) => base.CreateEntityAndRespond(userSession);

    
    [HttpDelete]
    public IActionResult Delete(Guid id) => base.DeleteEntityAndRespond(id);

    
    [HttpPut]
    public IActionResult Update(Guid id, [FromBody] UserSession userSession) => base.UpdateEntityAndRespond(id, userSession);
    
}