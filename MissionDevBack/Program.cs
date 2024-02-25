using MissionDevBack.Db;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MissionDevBack.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using MissionDevBack.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

EnsureSecretsAreDefined(builder.Configuration);
FileStorageService.EnsureStorageDirectoryAreCreated(builder.Configuration);

builder.Services.AddDbContext<MissionDevContext>(options =>
        options.UseNpgsql(builder.Configuration["MissionDevDbCredentials"]));

builder.Services.AddTransient<AuthService>();
builder.Services.AddTransient<NoyauSihService>();
builder.Services.AddTransient<FileStorageService>();
builder.Services.AddTransient<MailBoxService>();

builder.Services.AddIdentityCore<User>()
            .AddEntityFrameworkStores<MissionDevContext>()
            .AddDefaultTokenProviders();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = AuthService.GetAuthServiceTokenValidationParameters(builder.Configuration);
        });

builder.Services.AddAuthorization();

builder.Services.AddCors(options =>
    {
        options.AddPolicy("DevCorsPolicy", builder =>
        {
            builder.AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
    });



var app = builder.Build();

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<MissionDevContext>();
    var userManager = services.GetRequiredService<UserManager<User>>();
    context.Database.Migrate();
    await MissionDevSeed.RunAsync(context, userManager);
}

app.UseHttpsRedirection();

app.UseCors("DevCorsPolicy");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();


void EnsureSecretsAreDefined(IConfiguration config)
{
    var secretsToCheck = new string[]
    {
        "MissionDevDbCredentials",
        "MissionDevJwtPrivateKey",
        "MissionDevPathMainStorage"
    };
    foreach (string secret in secretsToCheck)
    {
        if (config[secret] is null)
        {
            throw new Exception($"Secret {secret} is not defined !");
        }
    }
}
