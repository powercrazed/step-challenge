using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StepChallenge.DataModels;
using StepChallenge.Services;

namespace StepChallenge.Controllers
{
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<RegisterController> _logger;
        private readonly UserService _userService;
        private readonly StepContext _stepContext;

        public UserController(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ILogger<RegisterController> logger,
            UserService userService,
            StepContext stepContext
            )
        {
            _userManager = userManager;
           _signInManager = signInManager;
            _logger = logger;
            _userService = userService;
            _stepContext = stepContext;
        }
       
        [HttpPost]
        [Route("login")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> OnPostAsync([FromBody] InputModel inputModel)
        {
            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, set lockoutOnFailure: true
            var result = await _signInManager.PasswordSignInAsync(inputModel.Username, inputModel.Password, false, lockoutOnFailure: true);
            if (!result.Succeeded)
            {
                 var err = new Dictionary<string, string>()
                     {{"error", "Incorrect username or password. Try again"}};
                 return new BadRequestObjectResult(err);
            }
            
            var response = new Dictionary<string, string>();
            response.Add("success", "User logged in");
            return new OkObjectResult(response);
        }

        [HttpPost]
        [Route("logout")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> OnPostLogOut(InputModel mode)
        {
            await _signInManager.SignOutAsync();
            return new OkResult();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [Route("edit_user")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> OnPostEditUser([FromBody] EditUserModel model)
        {
            var user = await _stepContext.Participants
                .Where(u => u.ParticipantId == model.ParticipantId)
                .Include(u => u.IdentityUser)
                .SingleOrDefaultAsync();

            if (user == null)
            {
                return BadRequest("No user found");
            }

            if (!string.IsNullOrEmpty(model.Password))
            {
                await _userManager.RemovePasswordAsync(user.IdentityUser);
                var passwordResult = await _userManager.AddPasswordAsync(user.IdentityUser, model.Password);
                if (passwordResult.Errors.Any())
                {
                    var err = new Dictionary<string, string> {{"error", passwordResult.Errors.First().Description}};
                    return new BadRequestObjectResult(err);
                }
            }

            var result = await _userManager.UpdateAsync(user.IdentityUser);
            if (result.Errors.Any())
            {
                var err = new Dictionary<string, string> {{"error", result.Errors.First().Description}};
                return new BadRequestObjectResult(err);
            }

            _logger.LogInformation($"Admin reset {user.ParticipantName} password ");

            var response = new Dictionary<string, string> {{"success", "User updated"}};
            return new OkObjectResult(response);
        }

    }
}