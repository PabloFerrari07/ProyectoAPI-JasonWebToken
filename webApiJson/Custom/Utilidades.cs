using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using webApiJson.Models;



namespace webApiJson.Custom
{
    public class Utilidades
    {
        private readonly IConfiguration _configuration;
        public Utilidades(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string encriptarSHA256(string texto)
        {
            using(SHA256 SHA256Hash = SHA256.Create())
            {

            byte[]bytes = SHA256Hash.ComputeHash(Encoding.UTF8.GetBytes(texto));
                StringBuilder builder = new StringBuilder();
                for(int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }

                return builder.ToString();  
            }

        }

        public string generarJWT(Usuario modelo)
        {
            //crear Informacion del usuario para el token
            var userClaims = new[]
            {
            new Claim(ClaimTypes.NameIdentifier,modelo.IdUsuario.ToString()),
        new Claim(ClaimTypes.Email,modelo.Correo!)
        };

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:key"]!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);
            //crear detalle toke
            var jwtConfig = new JwtSecurityToken(
                        claims: userClaims,
                        expires: DateTime.UtcNow.AddMinutes(10),
                        signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(jwtConfig);
         
        
        }
    }

}
