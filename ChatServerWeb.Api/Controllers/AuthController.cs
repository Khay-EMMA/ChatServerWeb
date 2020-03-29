using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ChatServerWeb.Api.Infrastructure;
using ChatServerWeb.Api.Model;
using ChatServerWeb.BusinessLogic;
using ChatServerWeb.Data;
using ChatServerWeb.Model.Entity;
using ChatServerWeb.Model.Identity;
using ChatServerWeb.Model.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ChatServerWeb.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly ChatApplicationsLogic _chatApplicationsLogic;
        private readonly IRepository _repository;
        readonly Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> userManager;
        readonly Microsoft.AspNetCore.Identity.SignInManager<ApplicationUser> signInManager;
        private readonly JWTAUthenticator jwtAuthenticator;

        public AuthController(Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> _userManager,
                              Microsoft.AspNetCore.Identity.SignInManager<ApplicationUser> _signInManager,
                              JWTAUthenticator _jwtAuthenticaor, IRepository repository, ChatApplicationsLogic chatApplicationsLogic)
        {
            userManager = _userManager;
            signInManager = _signInManager;
            jwtAuthenticator = _jwtAuthenticaor;
            _chatApplicationsLogic = chatApplicationsLogic;
            _repository = repository;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginModel model)
        {
            ApplicationUser user = await userManager.FindByEmailAsync(model.Email);
            var result = await signInManager.PasswordSignInAsync(user, model.Password,true,false);

            if (result.Succeeded)
            {
               var jwtResponse =  jwtAuthenticator.GenerateJwtToken(user);
                return Ok(jwtResponse);
            }


            return BadRequest("Invalid Auth Credentials");
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            ApplicationUser user = new ApplicationUser()
            {
                UserName = model.Email,
                Email = model.Email,

            };
                
             var result = await userManager.CreateAsync(user,model.Password);

            if (result.Succeeded)
            {
               var signInResult = await signInManager.PasswordSignInAsync(user, model.Password, true, false);

                if (signInResult.Succeeded)
                {
                    var jwtResponse = jwtAuthenticator.GenerateJwtToken(user);
                    return Ok(jwtResponse);
                }
            }


            return BadRequest("Invalid Auth Credentials");
        }

        [HttpPost("CreateChatApplication")]
        public IActionResult CreateChatApplication(ChatApplicationViewModel model)
        {
            ChatApplication chatApplication = new ChatApplication()
            {
                AppId = Guid.NewGuid().ToString(),
                DateCreated = DateTime.UtcNow,
                IsActive = true,
                AppName = model.AppName

            };
            _chatApplicationsLogic.AddEntity(chatApplication);
            int saveCount = _repository.Save();
            if (saveCount > 0)
            {
                return Ok(chatApplication);
            }


            return BadRequest("Invalid Auth Credentials");

        }

    }
}