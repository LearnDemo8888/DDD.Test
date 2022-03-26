using Microsoft.AspNetCore.Mvc;

namespace EFCore领域事件发布的时机.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly UserDbContext _userDbContext;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, UserDbContext userDbContext)
        {
            _logger = logger;
            _userDbContext = userDbContext;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            User user = new User("lrd","179722134@qq.com");
            user.ChangeNickName("大黄瓜");
            _userDbContext.Add(user);
            _userDbContext.SaveChanges();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}