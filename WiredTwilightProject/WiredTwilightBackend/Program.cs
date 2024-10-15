using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WiredTwilightBackend;

// Método para obter o usuário atual
static async Task<User?> GetCurrentUserAsync(WiredTwilightDbContext db, ClaimsPrincipal user)
{
    if (user.Identity?.IsAuthenticated != true)
        return null;

    var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    if (string.IsNullOrEmpty(userId))
        return null;

    return await db.Users.FirstOrDefaultAsync(u => u.Id == userId);
}

var builder = WebApplication.CreateBuilder(args);

// Configuração do banco de dados
builder.Services.AddDbContext<WiredTwilightDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("WiredTwilightDB")));

// Adicionar serviços ao contêiner
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configuração de autenticação e autorização
var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("YourNewSuperSecretKeyThatIsAtLeast32BytesLong123"));
builder.Services.AddAuthentication(options =>
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
        ValidIssuer = "https://seu-issuer-aqui",
        ValidAudience = "your-audience",
        IssuerSigningKey = key // Chave segura
    };
});

// Adicionando políticas de autorização
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminPolicy", policy => policy.RequireClaim("role", "admin"));
});

var app = builder.Build();

// Configurar o pipeline de requisições HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Endpoint POST para login
app.MapPost("/login", async (WiredTwilightDbContext banco, [FromBody] LoginRequest loginRequest) =>
{
    if (string.IsNullOrWhiteSpace(loginRequest.Username) || string.IsNullOrWhiteSpace(loginRequest.Password))
    {
        return Results.BadRequest("Username e Password são obrigatórios.");
    }

    var usuario = await banco.Users.FirstOrDefaultAsync(u => u.Username == loginRequest.Username);

    if (usuario == null)
    {
        return Results.NotFound("Usuário não encontrado.");
    }

    if (usuario.VerifyPassword(loginRequest.Password))
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, usuario.Id),
            new Claim(ClaimTypes.Name, usuario.Username),
        };

        var token = new JwtSecurityToken(
            issuer: "http://seu-issuer-aqui",
            audience: "your-audience",
            claims: claims,
            expires: DateTime.Now.AddHours(1),
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256) // Use a chave definida acima
        );

        return Results.Ok(new
        {
            token = new JwtSecurityTokenHandler().WriteToken(token)
        });
    }
    else
    {
        return Results.BadRequest("Senha incorreta.");
    }
});

// Endpoint POST para criar um fórum
app.MapPost("/forum", async (WiredTwilightDbContext db, [FromBody] Forum forum, HttpContext http) =>
{
    var currentUser = await GetCurrentUserAsync(db, http.User);
    if (currentUser == null)
    {
        return Results.Unauthorized();
    }

    forum.CreatedByUserId = currentUser.Id;
    db.Forums.Add(forum);
    await db.SaveChangesAsync();
    return Results.Created($"/forum/{forum.Id}", forum);
});

// Endpoint POST para criar um post em um fórum
app.MapPost("/forum/{forumId}/post", async (WiredTwilightDbContext db, int forumId, [FromBody] Post post, HttpContext http) =>
{
    var currentUser = await GetCurrentUserAsync(db, http.User);
    if (currentUser == null)
    {
        return Results.Unauthorized();
    }

    var forum = await db.Forums.FindAsync(forumId);
    if (forum == null || !forum.IsActive)
    {
        return Results.NotFound("Fórum não encontrado ou inativo.");
    }

    post.CreatedByUserId = currentUser.Id;
    post.ForumId = forumId;

    db.Posts.Add(post);
    await db.SaveChangesAsync();
    return Results.Created($"/forum/{forumId}/post/{post.Id}", post);
});

// Endpoint POST para comentar em um post
app.MapPost("/post/{postId}/comment", async (WiredTwilightDbContext db, int postId, [FromBody] Comment comment, HttpContext http) =>
{
    var currentUser = await GetCurrentUserAsync(db, http.User);
    if (currentUser == null)
    {
        return Results.Unauthorized();
    }

    var post = await db.Posts.FindAsync(postId);
    if (post == null)
    {
        return Results.NotFound("Post não encontrado.");
    }

    comment.CreatedByUserId = currentUser.Id;
    comment.PostId = postId;

    db.Comments.Add(comment);
    await db.SaveChangesAsync();
    return Results.Created($"/post/{postId}/comment/{comment.Id}", comment);
});

// Endpoint GET para busca avançada
app.MapGet("/search", async (WiredTwilightDbContext db, [FromQuery] string? keyword, [FromQuery] string? tag, [FromQuery] string? username) =>
{
    var query = db.Posts.Include(p => p.Tags).Include(p => p.CreatedByUser).AsQueryable();

    if (!string.IsNullOrEmpty(keyword))
    {
        query = query.Where(p => p.Title.Contains(keyword) || p.Content.Contains(keyword));
    }

    if (!string.IsNullOrEmpty(tag))
    {
        query = query.Where(p => p.Tags.Any(t => t.Name == tag));
    }

    if (!string.IsNullOrEmpty(username))
    {
        query = query.Where(p => p.CreatedByUser.Username == username);
    }

    var results = await query.Select(p => new
    {
        p.Id,
        p.Title,
        Author = p.CreatedByUser.Username,
        p.CreatedAt,
        p.ForumId
    }).ToListAsync();

    return Results.Ok(results);
});

// Endpoint POST para enviar uma mensagem privada
app.MapPost("/message", async (WiredTwilightDbContext db, [FromBody] PrivateMessage message, HttpContext http) =>
{
    var currentUser = await GetCurrentUserAsync(db, http.User);
    if (currentUser == null)
    {
        return Results.Unauthorized();
    }

    message.FromUserId = currentUser.Id;
    db.PrivateMessages.Add(message);
    await db.SaveChangesAsync();
    return Results.Created($"/message/{message.Id}", message);
});

// Endpoint DELETE para remover um post (moderação)
app.MapDelete("/post/{postId}", async (WiredTwilightDbContext db, int postId, HttpContext http) =>
{
    var currentUser = await GetCurrentUserAsync(db, http.User);
    if (currentUser == null)
    {
        return Results.Unauthorized();
    }

    var post = await db.Posts.FindAsync(postId);
    if (post == null)
    {
        return Results.NotFound("Post não encontrado.");
    }

    // Verifica se o usuário é admin
    if (!currentUser.IsAdmin)
    {
        return Results.Forbid();
    }

    db.Posts.Remove(post);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

// Endpoint GET para análise de dados do fórum
app.MapGet("/analytics/forum/{forumId}", async (WiredTwilightDbContext db, int forumId) =>
{
    var forum = await db.Forums
        .Include(f => f.Posts)
            .ThenInclude(p => p.Comments)
        .FirstOrDefaultAsync(f => f.Id == forumId);

    if (forum == null)
    {
        return Results.NotFound("Fórum não encontrado.");
    }

    var analytics = new
    {
        forum.Title,
        TotalPosts = forum.Posts.Count,
        TotalComments = forum.Posts.Sum(p => p.Comments.Count),
        MostPopularPost = forum.Posts.OrderByDescending(p => p.Comments.Count).FirstOrDefault()
    };

    return Results.Ok(analytics);
});
app.MapPost("/registro", async (WiredTwilightDbContext banco, [FromBody] User usuario) =>
{
    // Verifique se o usuário não está fornecendo uma senha em branco
    if (string.IsNullOrWhiteSpace(usuario.Password))
    {
        return Results.BadRequest("A senha é obrigatória.");
    }

    // Gerar o hash da senha
    usuario.PasswordHash = BCrypt.Net.BCrypt.HashPassword(usuario.Password);

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
