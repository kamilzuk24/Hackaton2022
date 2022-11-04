using EmailReaderApi.Services;
using Google.Apis.Auth.AspNetCore3;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddAuthentication(o =>
{
    o.DefaultChallengeScheme = GoogleOpenIdConnectDefaults.AuthenticationScheme;
    o.DefaultForbidScheme = GoogleOpenIdConnectDefaults.AuthenticationScheme;
    o.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
}).AddCookie().AddGoogleOpenIdConnect(options => { 
    options.ClientId = config.GetSection("GoogleAPI:ClientId").Value;
    options.ClientSecret = config.GetSection("GoogleAPI:ClientSecret").Value; 
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder => 
        builder.AllowAnyHeader().AllowAnyOrigin().AllowAnyMethod());
});

builder.Services.AddScoped<IEmailService, EmailService>();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();