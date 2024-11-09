using APICore.Data;
using APICore.DTOs;
using APICore.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace APICore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {

        private readonly UserManager<Usuario> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<Usuario> _signInManager;
        private readonly IConfiguration _configuration;

        public UsuarioController(UserManager<Usuario> userManager,
                                  RoleManager<IdentityRole> roleManager,
                                  SignInManager<Usuario> signInManager,
                                  IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }

        [HttpGet("GetEmpleados")]
        public async Task<IActionResult> GetEmpleados()
        {
            try
            {
                var usuarios = await _userManager.Users.ToListAsync();
                var empleados = new List<Usuario>();

                foreach (var usuario in usuarios)
                {
                    if (await _userManager.IsInRoleAsync(usuario, "Empleado"))
                    {
                        empleados.Add(usuario);
                    }
                }

                var empleadosDTO = empleados.Select(e => new {
                    e.Id,
                    e.Nombre,
                    e.UserName,
                    e.Email
                });

                return Ok(empleadosDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }



        // Método para el inicio de sesión
        // Método para el inicio de sesión
        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginModel login)
        {
            var user = await _userManager.FindByNameAsync(login.UserName);
            if (user == null)
            {
                return Unauthorized("Usuario o contraseña incorrectos.");
            }

            // Verifica la contraseña
            var result = await _signInManager.PasswordSignInAsync(user, login.Password, false, false);
            if (!result.Succeeded)
            {
                return Unauthorized("Usuario o contraseña incorrectos.");
            }

            // Genera un token JWT
            var authClaims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, user.UserName),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
    };

            // Agregar roles del usuario como claims
            var userRoles = await _userManager.GetRolesAsync(user);
            foreach (var role in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, role));
            }

            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));
            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddHours(3),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                expiration = token.ValidTo,
                role = userRoles.FirstOrDefault(), // Devuelve el primer rol
                usuarioId = user.Id // Agrega el ID del usuario a la respuesta
            });
        }



        //get 

        [HttpGet("GetRoles/{userName}")]
        public async Task<IActionResult> GetRoles(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
            {
                return NotFound("Usuario no encontrado.");
            }

            var roles = await _userManager.GetRolesAsync(user);
            return Ok(roles);
        }


        // POST: api/Usuario/CreateUser
        [HttpPost("CreateUser")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserModel model)
        {
            var user = new Usuario
            {
                UserName = model.UserName,
                Email = model.Email,
                Nombre = model.Nombre
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                return Ok("Usuario creado exitosamente.");
            }

            return BadRequest(result.Errors);
        }

        // POST: api/Usuario/AssignRole
        [HttpPost("AssignRole")]
        public async Task<IActionResult> AssignRole([FromBody] AssignRoleModel model)
        {
            var user = await _userManager.FindByNameAsync(model.UserName);
            if (user == null)
            {
                return NotFound("Usuario no encontrado.");
            }

            var roleExists = await _roleManager.RoleExistsAsync(model.RoleName);
            if (!roleExists)
            {
                return BadRequest("Rol no existe.");
            }

            var result = await _userManager.AddToRoleAsync(user, model.RoleName);

            if (result.Succeeded)
            {
                return Ok("Rol asignado exitosamente.");
            }

            return BadRequest(result.Errors);
        }
    }

    // Modelos auxiliares
    public class CreateUserModel
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Nombre { get; set; }
        public string Password { get; set; }
    }

    public class AssignRoleModel
    {
        public string UserName { get; set; }
        public string RoleName { get; set; }
    }
}
