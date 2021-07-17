using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Diplom.Common;
using Diplom.Common.Bodies;
using Diplom.Common.Entities;
using Diplom.Common.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Diplom.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly SignInManager<SiteUser> _signInManager; // манагер для авторизации
        private readonly UserManager<SiteUser> _userManager; // манагер для управления пользователями

        public AccountController(SignInManager<SiteUser> signInManager, UserManager<SiteUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] AuthBody data)
        {
            //входим по логину и паролю БЕЗ блокировки пользователя при вводе неправильных данных
            var auth = await _signInManager.PasswordSignInAsync(data.Login, data.Password,
                                                                true, false);

            // а если не ок, то досвиданья
            if(!auth.Succeeded)
            {
                return BadRequest("Неправильный логин или пароль");
            }

            // если всё ок
            // ищем сущность этого пользователя
            var user = await _userManager.FindByNameAsync(data.Login);
            var token = AuthService.GenerateToken(user); // создаём токен
            var role = await _userManager.GetRolesAsync(user); // узнаем роль пользователя

            // возвращаем инфу
            var response = new AuthResponse
            {
                AccessToken = token,
                UserName = data.Login,
                Email = user.Email,
                UserId = user.Id,
                Role = role.First()
            };

            return Ok(response);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterBody data)
        {
            var existedUser = await _userManager.FindByNameAsync(data.Login);
            if(existedUser != null)
            {
                return BadRequest("Пользователь с таким логином уже существует");
            }

            var user = new SiteUser
            {
                Email = data.Email,
                UserName = data.Login,
                FirstName = data.FirstName,
                LastName = data.LastName,
                Year = data.Year,
                PhoneNumber = data.PhoneNumber,
                Sex = data.Sex
            };

            // создаём юзера
            var result = await _userManager.CreateAsync(user, data.Password);

            //добавление роль по дефолту
            await _userManager.AddToRoleAsync(user, RoleNames.User);

            if(!result.Succeeded)
            {
                return BadRequest("Произошла ошибка во время создания пользователя"); //TODO:
            }

            // если всё ок, то токен создаем и возвращаем
            var token = AuthService.GenerateToken(user);

            var response = new AuthResponse
            {
                AccessToken = token,
                UserName = data.Login,
                Email = user.Email,
                UserId = user.Id,
                Role = RoleNames.User
            };

            return Ok(response);
        }

        //получение данных пользователя для изменения
        [HttpGet("userGet")]
        [Authorize]
        public async Task<IActionResult> GetCurrentUserInfo()
        {
            var user = User.FindFirstValue(ClaimTypes.NameIdentifier); //найти id пользователя по токену
            var existedUser = await _userManager.FindByIdAsync(user);
            if(existedUser is null)
            {
                return BadRequest("Пользователь не найден");
            }
                              
            var data = new UserResponse
            {
                Email = existedUser.Email,
                Login = existedUser.UserName,
                FirstName = existedUser.FirstName,
                LastName = existedUser.LastName,
                Year = existedUser.Year,
                PhoneNumber = existedUser.PhoneNumber
            };
            return Ok(data);
        }

        //изменение данных пользователя
        [HttpPost("userEdit")]
        [Authorize]
        public async Task<IActionResult> EditUser([FromBody] UserResponse body)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // найти id пользователя по токену
            var existedUser = await _userManager.FindByIdAsync(userId); // найти строчку с пользователем
            if(existedUser is null)
            {
                return BadRequest("Пользователь не найден");
            }

            existedUser.UserName = body.Login;
            existedUser.FirstName = body.FirstName;
            existedUser.LastName = body.LastName;
            existedUser.Year = body.Year;
            existedUser.Email = body.Email;
            existedUser.PhoneNumber = body.PhoneNumber;

            var result = await _userManager.UpdateAsync(existedUser);
            if(!result.Succeeded)
            {
                return BadRequest("Произошла ошибка во время изменения данных");
            }

            var newBody = new UserResponse
            {
                Email = existedUser.Email,
                Login = existedUser.UserName,
                FirstName = existedUser.FirstName,
                LastName = existedUser.LastName,
                Year = existedUser.Year,
                PhoneNumber = existedUser.PhoneNumber
            };
            return Ok(newBody);
        }

        // Изменение пароля
        [HttpPost("PasswordEdit")]
        [Authorize]
        public async Task<IActionResult> EditPassword(string password, string newPassword)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); //найти id пользователя по токену
            var existedUser = await _userManager.FindByIdAsync(userId); //найти строчку с пользователем
            if(existedUser is null)
            {
                return BadRequest("Пользователь не найден");
            }

            var result = await _userManager.ChangePasswordAsync(existedUser, password, newPassword);
            if(!result.Succeeded)
            {
                return BadRequest("Что-то пошло не так при обновлении");
            }

            var token = AuthService.GenerateToken(existedUser); // создаём токен

            var response = new AuthResponse
            {
                AccessToken = token,
            };

            return Ok(response);
        }

        // регистрация работников
        [HttpPost("registerWork")]
        [Authorize]
        public async Task<IActionResult> RegisterWork([FromBody] RegisterBody data, int rol)
        {
            var existedUser = await _userManager.FindByNameAsync(data.Login);
            if (existedUser != null)
            {
                return BadRequest("Пользователь с таким логином уже существует");
            }

            var user = new SiteUser
            {
                Email = data.Email,
                UserName = data.Login,
                FirstName = data.FirstName,
                LastName = data.LastName,
                Year = data.Year,
                PhoneNumber = data.PhoneNumber,
                Sex = data.Sex
            };

            // создаём юзера
            var result = await _userManager.CreateAsync(user, data.Password);

            if(rol == 0)
            {
                await _userManager.AddToRoleAsync(user, RoleNames.Director);
            }
            if (rol == 1)
            {
                await _userManager.AddToRoleAsync(user, RoleNames.Worker);
            }
            if (rol == -1)
            {
                return BadRequest("Не указана роль регестрируемого пользователя");
            }
            if (!result.Succeeded)
            {
                return BadRequest("Произошла ошибка во время создания пользователя");
            }

            return Ok();
        }

        //получение списка работников
        [HttpGet("workerGet")]
        [Authorize]
        public async Task<IActionResult> GetworkerList()
        {
            var user = User.FindFirstValue(ClaimTypes.NameIdentifier); //найти id пользователя по токену
            var list = await _userManager.GetUsersInRoleAsync(RoleNames.Worker);
            if (list is null)
            {
                return BadRequest("Пользователи не найдены");
            }

            var data = list.Select(x => new WorkerList
            {
                UserName = x.UserName,
                FirstName = x.FirstName,
                LastName = x.LastName,
                Year = x.Year.ToString(),
                PhoneNumber = x.PhoneNumber,
                Email = x.Email,
            });
            return Ok(data);
        }
        // удаление работника
        [HttpPost("workerDell")]
        [Authorize]
        public async Task<IActionResult> DeleteFromSiteUser(WorkerList del)
        {
            var userData = await _userManager.FindByNameAsync(del.UserName);// найти пользователя по логину
            await _userManager.DeleteAsync(userData); // удалить пользователя            

            return Ok();
        }

    }
}