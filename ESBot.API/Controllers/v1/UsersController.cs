using ESBot.API.Filter.Entities;
using ESBot.API.Interfaces;
using ESBot.API.Mapper;
using ESBot.Domain.Contracts.User;
using ESBot.Domain.Entities;
using ESBot.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;

namespace ESBot.API.Controllers.v1;

[Route("/v1/[controller]")]
[ApiController]
public class UsersController(EsBotDbContext context,
    IMapper<CreateUserDto, UpdateUserDto, UserDto, User> mapper) 
    : BaseController<User, CreateUserDto, UpdateUserDto, UserDto>(context, mapper), 
        IController<User, UserFilter, CreateUserDto, UpdateUserDto>
{
 
    [HttpGet]
    public async Task<IActionResult> Filter([FromQuery] UserFilter filter, [FromQuery] int page = 1, [FromQuery] int pageSize = 50) => await base.FilterEntities(filter, page, pageSize);
    
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUserDto user) => await base.CreateEntityAndRespond(user);

    
    [HttpDelete]
    public async Task<IActionResult> Delete(Guid id) => await base.DeleteEntityAndRespond(id);

    
    [HttpPut]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUserDto user) => await base.UpdateEntityAndRespond(id, user);
    
}