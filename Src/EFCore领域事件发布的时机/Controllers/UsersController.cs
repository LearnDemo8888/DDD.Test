using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EFCore领域事件发布的时机.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {

        private readonly ILogger<UsersController> _logger;
        private readonly UserDbContext _userDbContext;

     
        public UsersController(ILogger<UsersController> logger, UserDbContext userDbContext)
        {
            _logger = logger;
            _userDbContext = userDbContext;

        }


        [HttpGet]
        public async  Task<IActionResult> NewUser()
        {
         
             User user = new User("Dong","179722134@qq.com");
            _userDbContext.Add(user);
            await _userDbContext.SaveChangesAsync();
            return new JsonResult("添加成功");
        }


        [HttpPost]
        public async Task<IActionResult> UserNameChange(Guid id,string userName)
        {

            var user = await _userDbContext.Users.FindAsync(id);
            user?.ChangeUserName(userName);
            _userDbContext.Update(user);
            await _userDbContext.SaveChangesAsync();
            return new JsonResult("更改用户名成功");
        }
    }
}
