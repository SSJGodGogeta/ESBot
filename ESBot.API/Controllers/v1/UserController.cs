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
public class UserController(EsBotDbContext context,
    IMapper<CreateUserDto, UpdateUserDto, UserDto, User> mapper) 
    : BaseController<User, CreateUserDto, UpdateUserDto, UserDto>(context, mapper), 
        IController<User, UserFilter, CreateUserDto, UpdateUserDto>
{
 
    [HttpGet]
    public Task<IActionResult> Filter([FromQuery] UserFilter filter) => base.FilterEntities(filter);
    
    [HttpPost]
    public IActionResult Create([FromBody] CreateUserDto user) => base.CreateEntityAndRespond(user);

    
    [HttpDelete]
    public IActionResult Delete(Guid id) => base.DeleteEntityAndRespond(id);

    
    [HttpPut]
    public IActionResult Update(Guid id, [FromBody] UpdateUserDto user) => base.UpdateEntityAndRespond(id, user);
    
}