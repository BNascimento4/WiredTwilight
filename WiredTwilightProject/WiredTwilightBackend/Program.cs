using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WiredTwilightBackend;

var builder = WebApplication.CreateBuilder(args);

// Configuração da string de conexão (deve estar no appsettings.json)
builder.Services.AddDbContext<WiredTwilightDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("WiredTwilightDB")));

// Adiciona serviços ao contêiner.
builder.Services.AddControllers();
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

// Endpoint POST para registro de usuário
app.MapPost("/registro", async (WiredTwilightDbContext banco, [FromBody] User usuario) =>
{
    banco.Users.Add(usuario);
    await banco.SaveChangesAsync();
    return Results.Created($"/registro/{usuario.Id}", usuario);
});

// Endpoint GET para obter registros de usuários
app.MapGet("/PegarRegistro", async (WiredTwilightDbContext banco) =>
{
    var usuarios = await banco.Users.ToListAsync();
    return Results.Ok(usuarios);
});

app.Run();

