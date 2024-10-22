using Microsoft.EntityFrameworkCore;
using webApiJson.Custom;
using webApiJson.Models;
using Microsoft.AspNetCore.Authentication;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using webApiJson.Models.Dtos;

namespace webApiJson.Controllers
{
    [Route("api/[controller]")]
    [AllowAnonymous]
    [ApiController]
    public class AccesoController : ControllerBase
    {
        private readonly DbpruebaContext _dbPruebaContext;
        private readonly Utilidades _utilidades;
        public AccesoController(DbpruebaContext dbPruebaContext, Utilidades utilidades)
        {
            _dbPruebaContext = dbPruebaContext;
            _utilidades = utilidades;
        }

        [HttpPost]
        [Route("Registrarse")]
        public async Task<IActionResult> Registrarse(UsuarioDto objeto)
        {
            var modelUsuario = new Usuario
            {
                Nombre = objeto.Nombre,
                Correo = objeto.Correo,
                Clave = _utilidades.encriptarSHA256(objeto.Clave)
            };

            await _dbPruebaContext.Usuarios.AddAsync(modelUsuario);
            await _dbPruebaContext.SaveChangesAsync();

            if (modelUsuario.IdUsuario != 0)
            {
                return StatusCode(StatusCodes.Status200OK, new { isSucces = true });
            }
            else
            {
                return StatusCode(StatusCodes.Status200OK, new { isSucces = false });
            }

        }
        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login(loginDto objeto)
        {
            var usuarioEncontrado = await _dbPruebaContext.Usuarios
                 .Where(u =>
             u.Correo == objeto.Correo && u.Clave == _utilidades.encriptarSHA256(objeto.Clave)

             ).FirstOrDefaultAsync();

            if (usuarioEncontrado == null)
            {
                return StatusCode(StatusCodes.Status200OK, new {isSuccess = false,token = ""});
            }
            else
            {
                return StatusCode(StatusCodes.Status200OK, new { isSuccess = true, token = _utilidades.generarJWT(usuarioEncontrado) });
            }
        }
    }
}
