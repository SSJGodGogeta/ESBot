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
    public Task<IActionResult> Filter([FromQuery] MessageFilter filter) => base.FilterEntities(filter);
    
    [HttpPost]
    public IActionResult Create([FromBody] CreateMessageDto message) => base.CreateEntityAndRespond(message);

    
    [HttpDelete]
    public IActionResult Delete(Guid id) => base.DeleteEntityAndRespond(id);

    
    [HttpPut]
    public IActionResult Update(Guid id, [FromBody] UpdateMessageDto message) => base.UpdateEntityAndRespond(id, message);
    
}