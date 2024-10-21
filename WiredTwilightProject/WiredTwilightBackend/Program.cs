using Microsoft.EntityFrameworkCore;
using WiredTwilightBackend;

var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddDbContext<WiredTwilightDbContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("WiredTwilightDB")));

            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
                });

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapEndpoints();

app.Run();
