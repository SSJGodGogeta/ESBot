using ESBot.API.Filter.Entities;
using ESBot.Domain.Entities;
using ESBot.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;

namespace ESBot.API.Controllers.v1;

[Route("/v1/[controller]")]
[ApiController]
public class MessageController(EsBotDbContext context): BaseController<Message>(context), IController<Message, MessageFilter>
{
 
    [HttpGet("filter")]
    public Task<IActionResult> Filter([FromQuery] MessageFilter filter) => base.FilterEntities(filter);
    
    [HttpPost]
    public IActionResult Create([FromBody] Message message) => base.CreateEntityAndRespond(message);

    
    [HttpDelete]
    public IActionResult Delete(int id) => base.DeleteEntityAndRespond(id);

    
    [HttpPut]
    public IActionResult Update(int id, [FromBody] Message message) => base.UpdateEntityAndRespond(id, message);
    
}