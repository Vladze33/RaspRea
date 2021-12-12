using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using RaspRea.Dto;

namespace RaspRea.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly IDistributedCache _cache;
        public LoginController(IDistributedCache cache)
        {
            _cache = cache;
        }
        
        [HttpGet]
        [Route("/GetUser/")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(UserCredentials), StatusCodes.Status200OK)]
        public async Task<UserCredentials> GetUserFromRedis([FromQuery] string login)
        {
            var password = await _cache.GetStringAsync(login);
            UserCredentials userCredentials = new UserCredentials
            {
                Login = login,
                Password = password
            };
            return userCredentials;
        }
        
        [HttpPost]
        [Route("/PostUser/")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(UserCredentials), StatusCodes.Status200OK)]
        public async Task<UserCredentials> PostUserIntoRedis([FromQuery] string login, [FromQuery] string password)
        {
            if (_cache.GetStringAsync(login) != null)
                await _cache.SetStringAsync(login, password);

            return new UserCredentials{ Login = login, Password = password };
        }
        
    }
}