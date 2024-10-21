using Microsoft.AspNetCore.Authentication.JwtBearer;
using WiredTwilightBackend;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace WiredTwilightBackend.Controller
{
    public class AuthenticationService
    {
        public AuthenticationService(IServiceCollection services, IConfiguration configuration)
        {
            // Configuração de autenticação e autorização
            var jwtSettings = configuration.GetSection("JwtSettings");
            var keyString = jwtSettings["Key"]; // A chave secreta vem da configuração

            if (string.IsNullOrEmpty(keyString))
            {
                throw new InvalidOperationException("A chave JWT não está configurada. Verifique o arquivo de configuração.");
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = "https://seu-issuer-aqui", // Ajuste conforme necessário
                    ValidAudience = "your-audience", // Ajuste conforme necessário
                    IssuerSigningKey = key // Chave segura para assinatura
                };
            });

            // Adicionando políticas de autorização
            services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminPolicy", policy => policy.RequireClaim("role", "admin"));
            });
        }
    }
}
