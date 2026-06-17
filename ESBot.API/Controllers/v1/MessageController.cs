using ESBot.API.Filter.Entities;
using ESBot.API.Interfaces;
using ESBot.API.Mapper;
using ESBot.Domain.Contracts.Message;
using ESBot.Domain.Entities;
using ESBot.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;

namespace ESBot.API.Controllers.v1;

[Route("/v1/[controller]")]
[ApiController]
public class MessageController(EsBotDbContext context,
    IMapper<CreateMessageDto, UpdateMessageDto, MessageDto, Message> mapper) 
    : BaseController<Message, CreateMessageDto, UpdateMessageDto, MessageDto>(context, mapper), 
        IController<Message, MessageFilter, CreateMessageDto, UpdateMessageDto>
{
 
    [HttpGet]
    public async Task<IActionResult> Filter([FromQuery] MessageFilter filter, [FromQuery] int page = 1, [FromQuery] int pageSize = 50) => await base.FilterEntities(filter, page, pageSize);
    
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateMessageDto message) => await base.CreateEntityAndRespond(message);

    
    [HttpDelete]
    public async Task<IActionResult> Delete(Guid id) => await base.DeleteEntityAndRespond(id);

    
    [HttpPut]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateMessageDto message) => await base.UpdateEntityAndRespond(id, message);
    
}