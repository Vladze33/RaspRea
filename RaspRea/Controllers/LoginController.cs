using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using RaspRea.Dto;
using StackExchange.Redis;

namespace RaspRea.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly IConnectionMultiplexer _redis;
        public LoginController(IConnectionMultiplexer redis)
        {
            _redis = redis;
        }
        
        [HttpGet]
        [Route("/GetUser/")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(UserCredentials), StatusCodes.Status200OK)]
        public async Task<UserCredentials> GetUserFromRedis([FromQuery] string login)
        {
            var db = _redis.GetDatabase();
            var group = await db.StringGetAsync(login);
            UserCredentials userCredentials = new UserCredentials
            {
                Login = login,
                Group = group.ToString()
            };
            return userCredentials;
        }
        
        [HttpPost]
        [Route("/PostUser/")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(UserCredentials), StatusCodes.Status200OK)]
        public async Task<UserCredentials> PostUserIntoRedis([FromQuery] string login, [FromQuery] string group)
        {
            var db = _redis.GetDatabase();
            var user = await db.StringSetAsync(login, group);

            return new UserCredentials{ Login = login, Group = group };
        }
        
    }
}