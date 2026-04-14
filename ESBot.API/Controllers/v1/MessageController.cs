using ESBot.API.Filter.Entities;
using ESBot.Domain.Entities;
using ESBot.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;

namespace ESBot.API.Controllers.v1;

[Route("/v1/[controller]")]
[ApiController]
public class MessagesController(EsBotDbContext context): BaseController<Message>(context), IController<Message, MessageFilter>
{
 
    [HttpGet]
    public Task<IActionResult> Filter([FromQuery] MessageFilter filter) => base.FilterEntities(filter);
    
    [HttpPost]
    public IActionResult Create([FromBody] Message message) => base.CreateEntityAndRespond(message);

    
    [HttpDelete("{id:guid}")]
    public IActionResult Delete(Guid id) => base.DeleteEntityAndRespond(id);

    
    [HttpPut("{id:guid}")]
    public IActionResult Update(Guid id, [FromBody] Message message) => base.UpdateEntityAndRespond(id, message);
    
}