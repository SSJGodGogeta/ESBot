using ESBot.API.Filter.Entities;
using ESBot.Domain.Entities;
using ESBot.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;

namespace ESBot.API.Controllers.v1;

[Route("/v1/[controller]")]
[ApiController]
public class UserSessionController(EsBotDbContext context): BaseController<UserSession>(context), IController<UserSession, UserSessionFilter>
{
 
    [HttpGet("filter")]
    public Task<IActionResult> Filter([FromQuery] UserSessionFilter filter) => base.FilterEntities(filter);
    
    [HttpPost]
    public IActionResult Create([FromBody] UserSession userSession) => base.CreateEntityAndRespond(userSession);

    
    [HttpDelete]
    public IActionResult Delete(int id) => base.DeleteEntityAndRespond(id);

    
    [HttpPut]
    public IActionResult Update(int id, [FromBody] UserSession userSession) => base.UpdateEntityAndRespond(id, userSession);
    
}