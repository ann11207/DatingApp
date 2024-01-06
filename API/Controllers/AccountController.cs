using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly DataContext _context;

        public AccountController(DataContext context )
        {
            _context = context;
        }

        [HttpPost("Register")]
        public async Task<ActionResult<AppUser>> Register(RegisteDto registerDto)
        {

            if (await UserExists(registerDto.Username))
                return BadRequest(" user name is taken !");
            using var hmac = new HMACSHA512();

            var users = new AppUser
            {
                UserName = registerDto.Username.ToLower(),
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
                PasswordSalt = hmac.Key
            };   

            _context.Users.Add(users);
            await _context.SaveChangesAsync();
            return users;
        }

        private async Task<bool> UserExists(string username){
            return await _context.Users.AnyAsync(x => x.UserName == username.ToLower());
        }
    }
}