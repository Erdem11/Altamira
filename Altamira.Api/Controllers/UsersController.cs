using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Altamira.Api.Models;
using Altamira.Api.Models.Users;
using Altamira.Api.Models.Users.Login;
using Altamira.Cache;
using Altamira.EfRepository;
using Altamira.Entities;
using Altamira.Repository;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Altamira.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly ICache _cache;
        private readonly IMapper _mapper;
        
        public UsersController(IUserRepository userRepository, ICache cache, IMapper mapper)
        {
            _userRepository = userRepository;
            _cache = cache;
            _mapper = mapper;
        }

        [Route("login")]
        [AllowAnonymous]
        [HttpPost]
        public async Task<ResponseBase<LoginResponse>> Login(LoginRequest request)
        {
            var user = await _userRepository.GetByUserName(request.UserName);

            if (user == default || user.Password != request.Password)
            {
                return new ResponseBase<LoginResponse>
                {
                    Error = "Check your credentials"
                };
            }

            var userclaim = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, request.UserName)
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Guid.Empty.ToString()));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expireToken = DateTime.UtcNow.AddDays(1);

            var token = new JwtSecurityToken(
            issuer: "http://localhost/",
            audience: "http://localhost/",
            claims: userclaim,
            expires: expireToken,
            signingCredentials: creds);

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return new ResponseBase<LoginResponse>()
            {
                Data = new LoginResponse()
                {
                    Token = tokenString
                }
            };
        }

        // GET: api/<UsersController>
        [HttpGet]
        public async Task<ResponseBase<List<User>>> Get()
        {
            var cacheResult = await _cache.GetCache<List<User>>("UserList");

            List<User> userList = cacheResult;
            if (cacheResult == default)
            {
                var iEnumerable = await _userRepository.Get();
                userList = iEnumerable.ToList();

                await _cache.SetCache(userList, "UserList", new TimeSpan(0, 2, 0));
            }

            return new ResponseBase<List<User>>()
            {
                Data = userList
            };
        }

        // GET api/<UsersController>/5
        [HttpGet("{id}")]
        public async Task<ResponseBase<User>> Get(int id)
        {
            var cacheResult = await _cache.GetCache<User>(id);

            User user = cacheResult;
            if (cacheResult == default)
            {
                user = await _userRepository.GetDetail(id);

                await _cache.SetCache(user, id, new TimeSpan(0, 2, 0));
            }

            return new ResponseBase<User>()
            {
                Data = user
            };
        }

        // POST api/<UsersController>
        [HttpPost]
        public async Task<ResponseBase<User>> Post([FromBody] UserModel model)
        {
            var user = _mapper.Map<User>(model);
            
            await _userRepository.Add(user);

            await _cache.SetCache(user, user.Id, new TimeSpan(0, 2, 0));

            return new ResponseBase<User>()
            {
                Data = user
            };
        }

        // PUT api/<UsersController>/5
        [HttpPut("{id}")]
        public async Task<ResponseBase<User>> Put(int id, [FromBody] UserModel model)
        {
            var user = _mapper.Map<User>(model);
            
            var result = await _userRepository.Update(id, user);

            if (result.IsT1)
            {
                return new ResponseBase<User>()
                {
                   Error = result.AsT1.Message
                };
            }

            await _cache.SetCache(user, user.Id, new TimeSpan(0, 2, 0));

            return new ResponseBase<User>()
            {
                Data = user
            };
        }

        // DELETE api/<UsersController>/5
        [HttpDelete("{id}")]
        public async Task<ResponseBase<bool>> Delete(int id)
        {
            await _userRepository.Delete(id);

            await _cache.DeleteCache($"{nameof(User)}_{id}");

            return new ResponseBase<bool>()
            {
                Data = true
            };
        }
    }
}