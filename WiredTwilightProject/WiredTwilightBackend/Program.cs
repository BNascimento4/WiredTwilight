using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WiredTwilightBackend;
using WiredTwilightBackend.Migrations;

var builder = WebApplication.CreateBuilder(args);

// Adiciona serviços ao contêiner.
builder.Services.AddControllers();  // Habilita o uso de controllers na Web API.

// Adiciona o Swagger para documentação automática da API.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configura o pipeline HTTP.
if (app.Environment.IsDevelopment())
{
    // Habilita o Swagger apenas em ambiente de desenvolvimento.
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();  // Redireciona requisições HTTP para HTTPS.

app.UseAuthorization();  // Habilita autorização (caso tenha implementado).

app.MapControllers();  // Mapeia as rotas para os controllers.

app.MapPost("/registro", ([FromBody] WiredTwilightDbContext banco, [FromServices] User usuario) =>
{
    banco.Add(usuario);
});

app.Run();
