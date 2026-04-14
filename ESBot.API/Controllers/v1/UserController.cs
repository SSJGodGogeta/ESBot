using ESBot.API.Filter.Entities;
using ESBot.Domain.Entities;
using ESBot.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;

namespace ESBot.API.Controllers.v1;

[Route("/v1/[controller]")]
[ApiController]
public class UserController(EsBotDbContext context): BaseController<User>(context), IController<User, UserFilter>
{
 
    [HttpGet("filter")]
    public Task<IActionResult> Filter([FromQuery] UserFilter filter) => base.FilterEntities(filter);
    
    [HttpPost]
    public IActionResult Create([FromBody] User user) => base.CreateEntityAndRespond(user);

    
    [HttpDelete]
    public IActionResult Delete(int id) => base.DeleteEntityAndRespond(id);

    
    [HttpPut]
    public IActionResult Update(int id, [FromBody] User user) => base.UpdateEntityAndRespond(id, user);
    
}