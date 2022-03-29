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
        private readonly IServiceProvider _serviceProvider;


        public UsersController(ILogger<UsersController> logger, UserDbContext userDbContext, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _userDbContext = userDbContext;
            _serviceProvider = serviceProvider;

        }


        [HttpGet]
        public async  Task<IActionResult> NewUser()
        {
            var  middleares= _serviceProvider.GetService<IEnumerable<IMiddleware<string>>>();
            EventHandlerDelegate @delegate = async () => {

                Console.WriteLine("sdfsdf");
                await Task.CompletedTask; 
            
            };


            await middleares.Reverse().Aggregate(@delegate, (next, middleware) => () =>  middleware.HandleAsync("ddf",next))(); 
            // User user = new User("Dong","179722134@qq.com");
            //_userDbContext.Add(user);
            //await _userDbContext.SaveChangesAsync();
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


    public delegate Task EventHandlerDelegate();

    public interface IMiddleware<TEvent> where TEvent : class
    {
        Task HandleAsync(TEvent @event, EventHandlerDelegate next);
    }


    public class TestMiddleware1 : IMiddleware<string>
    {
        public async Task HandleAsync(string @event, EventHandlerDelegate next)
        {
            Console.WriteLine("TestMiddleware1");
        

            await next();
            Console.WriteLine("TestMiddleware1-2");
        }
    }

    public class TestMiddleware2 : IMiddleware<string>
    {
        public async Task HandleAsync(string @event, EventHandlerDelegate next)
        {

            Console.WriteLine("TestMiddleware2");
            await next();
            Console.WriteLine("TestMiddleware2-2");
        }
    }


    public class TestMiddleware3 : IMiddleware<string>
    {
        public async Task HandleAsync(string @event, EventHandlerDelegate next)
        {

            Console.WriteLine("TestMiddleware3");
            await next();
            Console.WriteLine("TestMiddleware3-2");
        }
    }
}
