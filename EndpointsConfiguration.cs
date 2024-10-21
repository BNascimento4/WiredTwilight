using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WiredTwilightBackend;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;

using System.Text.Json;
using System.Text;


public static class EndpointsConfiguration
{

    private static readonly string SecretKey = "sua-chave-secreta-aqui";
    private static readonly SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey));

    public static void MapEndpoints(this IEndpointRouteBuilder app)
    {
        var builder = WebApplication.CreateBuilder();
        var jwtSettings = builder.Configuration.GetSection("JwtSettings");
        var keyString = jwtSettings["Key"]; // A chave secreta vem da configuração

        if (string.IsNullOrEmpty(keyString))
        {
            throw new InvalidOperationException("A chave JWT não está configurada. Verifique o arquivo de configuração.");
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));


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
            new Claim("role", usuario.IsAdmin ? "admin" : "user")
                };

                var token = new JwtSecurityToken(
                    issuer: "https://seu-issuer-aqui",
                    audience: "your-audience",
                    claims: claims,
                    expires: DateTime.Now.AddHours(1),
                    signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
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



        app.MapPost("/registro", async (WiredTwilightDbContext banco, [FromBody] User usuario) =>
        {
            if (string.IsNullOrWhiteSpace(usuario.Password))
            {
                return Results.BadRequest("A senha é obrigatória.");
            }

            usuario.PasswordHash = BCrypt.Net.BCrypt.HashPassword(usuario.Password);

            banco.Users.Add(usuario);
            await banco.SaveChangesAsync();
            return Results.Created($"/registro/{usuario.Id}", usuario);
        });



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



        app.MapPost("/forum/{forumId}/post", async (WiredTwilightDbContext db, int forumId, [FromBody] PostDTO postDto, HttpContext http) =>
        {
            try
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

                if (!Validator.TryValidateObject(postDto, new ValidationContext(postDto), null, true))
                {
                    return Results.BadRequest("Dados inválidos.");
                }

                var post = new Post
                {
                    Title = postDto.Title,
                    Content = postDto.Content,
                    CreatedByUserId = currentUser.Id,
                    ForumId = forumId
                };

                db.Posts.Add(post);
                await db.SaveChangesAsync();
                return Results.Created($"/forum/{forumId}/post/{post.Id}", post);
            }
            catch (JsonException jsonEx)
            {
                return Results.BadRequest("Erro de serialização: " + jsonEx.Message);
            }
            catch (Exception ex)
            {
                return Results.Problem("Erro interno do servidor: " + ex.Message, statusCode: 500);
            }
        });



        app.MapPost("/forum/{forumId}/post/{postId}/comment", async (WiredTwilightDbContext db, int forumId, int postId, [FromBody] Comment comment, HttpContext http) =>
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

            var post = await db.Posts.FirstOrDefaultAsync(p => p.Id == postId && p.ForumId == forumId);
            if (post == null)
            {
                return Results.NotFound("Post não encontrado ou não pertence ao fórum especificado.");
            }

            comment.CreatedByUserId = currentUser.Id;
            comment.PostId = postId;

            db.Comments.Add(comment);
            await db.SaveChangesAsync();
            return Results.Created($"/forum/{forumId}/post/{postId}/comment/{comment.Id}", comment);
        });



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



        app.MapGet("/PegarRegistro", async (WiredTwilightDbContext banco) =>
        {
            var usuarios = await banco.Users.ToListAsync();
            return Results.Ok(usuarios);
        });



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



        app.MapGet("/forums", async (WiredTwilightDbContext db) =>
        {
            try
            {
                var forums = await db.Forums
                    .Include(f => f.Posts)
                    .Where(f => f.IsActive)
                    .Select(f => new
                    {
                        f.Id,
                        f.Title,
                        f.Description,
                        Posts = f.Posts.Select(p => new
                        {
                            p.Id,
                            p.Title,
                            p.Content,
                            p.CreatedAt,
                            CreatedByUserId = p.CreatedByUserId
                        }).ToList()
                    })
                    .ToListAsync();

                return Results.Ok(forums);
            }
            catch (Exception ex)
            {
                return Results.Problem("Erro interno do servidor: " + ex.Message, statusCode: 500);
            }
        });



        app.MapGet("/forum/{forumId}/posts", async (WiredTwilightDbContext db, int forumId) =>
        {
            try
            {
                var forum = await db.Forums
                    .Include(f => f.Posts)
                    .FirstOrDefaultAsync(f => f.Id == forumId && f.IsActive);

                if (forum == null)
                {
                    return Results.NotFound("Fórum não encontrado ou inativo.");
                }

                var posts = forum.Posts.Select(p => new
                {
                    p.Id,
                    p.Title,
                    p.Content,
                    p.CreatedAt,
                    CreatedByUserId = p.CreatedByUserId
                }).ToList();

                return Results.Ok(posts);
            }
            catch (Exception ex)
            {
                return Results.Problem("Erro interno do servidor: " + ex.Message, statusCode: 500);
            }
        });



        app.MapGet("/forum/{forumId}/post/{postId}/comments", async (WiredTwilightDbContext db, int forumId, int postId) =>
        {
            try
            {
                var post = await db.Posts
                    .Include(p => p.Comments)
                    .FirstOrDefaultAsync(p => p.Id == postId);

                if (post == null || post.ForumId != forumId)
                {
                    return Results.NotFound("Post não encontrado ou não pertence ao fórum especificado.");
                }

                var comments = post.Comments.Select(c => new
                {
                    c.Id,
                    c.Content,
                    c.CreatedAt,
                    CreatedByUserId = c.CreatedByUserId
                }).ToList();

                return Results.Ok(comments);
            }
            catch (Exception ex)
            {
                return Results.Problem("Erro interno do servidor: " + ex.Message, statusCode: 500);
            }
        });



        app.MapGet("/message", async (WiredTwilightDbContext db, HttpContext http) =>
        {
            var currentUser = await GetCurrentUserAsync(db, http.User);
            if (currentUser == null)
            {
                return Results.Unauthorized();
            }

            var messages = await db.PrivateMessages
                .Where(m => m.ToUserId == currentUser.Id || m.FromUserId == currentUser.Id)
                .ToListAsync();

            return Results.Ok(messages);
        });



        app.MapGet("/message/{messageId}", async (WiredTwilightDbContext db, int messageId, HttpContext http) =>
        {
            var currentUser = await GetCurrentUserAsync(db, http.User);
            if (currentUser == null)
            {
                return Results.Unauthorized();
            }

            var message = await db.PrivateMessages.FindAsync(messageId);
            if (message == null || (message.ToUserId != currentUser.Id && message.FromUserId != currentUser.Id))
            {
                return Results.NotFound("Mensagem não encontrada ou acesso negado.");
            }

            return Results.Ok(message);
        });



        app.MapGet("/user/{userId}", async (WiredTwilightDbContext db, string userId) =>
        {
            var user = await db.Users.FindAsync(userId);
            if (user == null)
            {
                return Results.NotFound("Usuário não encontrado.");
            }

            return Results.Ok(new
            {
                user.Id,
                user.Username,
                user.IsAdmin
            });
        });



        app.MapGet("/user/{userId}/messages", async (WiredTwilightDbContext db, string userId) =>
        {
            var messages = await db.PrivateMessages
                .Where(m => m.ToUserId == userId || m.FromUserId == userId)
                .ToListAsync();

            return Results.Ok(messages);
        });




        app.MapGet("/user/{userId}/forums", async (WiredTwilightDbContext db, string userId) =>
        {
            var forums = await db.Forums
                .Where(f => f.CreatedByUserId == userId)
                .ToListAsync();

            return Results.Ok(forums);
        });




        app.MapGet("/user/{userId}/posts", async (WiredTwilightDbContext db, string userId) =>
        {
            var posts = await db.Posts
                .Where(p => p.CreatedByUserId == userId)
                .ToListAsync();

            return Results.Ok(posts);
        });




        app.MapGet("/user/{userId}/comments", async (WiredTwilightDbContext db, string userId) =>
        {
            var comments = await db.Comments
                .Where(c => c.CreatedByUserId == userId)
                .ToListAsync();

            return Results.Ok(comments);
        });



        app.MapPut("/usuario/atualizar-username", async (WiredTwilightDbContext db, [FromBody] UpdateUsernameRequest request, HttpContext http) =>
        {
            var currentUser = await GetCurrentUserAsync(db, http.User);
            if (currentUser == null)
            {
                return Results.Unauthorized();
            }

            // Valida se o novo nome de usuário não está vazio
            if (string.IsNullOrWhiteSpace(request.NewUsername))
            {
                return Results.BadRequest("O novo nome de usuário é obrigatório.");
            }

            // Verifica se o nome de usuário já está em uso
            var existingUser = await db.Users.FirstOrDefaultAsync(u => u.Username == request.NewUsername);
            if (existingUser != null)
            {
                return Results.Conflict("O nome de usuário já está em uso.");
            }

            // Atualiza o nome de usuário do usuário atual
            currentUser.Username = request.NewUsername;
            db.Users.Update(currentUser);
            await db.SaveChangesAsync();

            return Results.Ok("Nome de usuário atualizado com sucesso.");
        });


        app.MapDelete("/post/{postId}", [Authorize(Policy = "AdminPolicy")] async (WiredTwilightDbContext db, int postId, HttpContext http) =>
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

            db.Posts.Remove(post);
            await db.SaveChangesAsync();
            return Results.NoContent();
        });


        app.MapDelete("/forum/{forumId}/post/{postId}", [Authorize(Policy = "AdminPolicy")] async (WiredTwilightDbContext db, int forumId, int postId, HttpContext http) =>
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

            var post = await db.Posts.FirstOrDefaultAsync(p => p.Id == postId && p.ForumId == forumId);
            if (post == null)
            {
                return Results.NotFound("Post não encontrado no fórum especificado.");
            }

            if (!currentUser.IsAdmin && post.CreatedByUserId != currentUser.Id)
            {
                return Results.Forbid();
            }

            db.Posts.Remove(post);
            await db.SaveChangesAsync();

            return Results.NoContent();
        });

    }

    private static async Task<User?> GetCurrentUserAsync(WiredTwilightDbContext db, ClaimsPrincipal user)
    {
        if (user.Identity?.IsAuthenticated != true)
            return null;

        var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            return null;

        return await db.Users.FirstOrDefaultAsync(u => u.Id == userId);
    }
}