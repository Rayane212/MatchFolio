using MatchFolio_Authentication.Model;
using System.Net.Http;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "API Gateway");
    });
}
var authServiceClient = new HttpClient { BaseAddress = new Uri("https://localhost:7064/") };

// Route pour s'inscrire
app.MapPost("/matchFolio/signUp", async (HttpContext http, UserEntity user) =>
{
    var response = await authServiceClient.PostAsJsonAsync("signUp", user);
    var result = await response.Content.ReadAsStringAsync();
    http.Response.StatusCode = (int)response.StatusCode;
    await http.Response.WriteAsync(result);
});

// Route pour se connecter
app.MapPost("/matchFolio/signIn", async (HttpContext http, string? mail, string? username, string password) =>
{
    var payload = new { mail, username, password };
    var response = await authServiceClient.PostAsJsonAsync("signIn", payload);
    var result = await response.Content.ReadAsStringAsync();
    http.Response.StatusCode = (int)response.StatusCode;
    await http.Response.WriteAsync(result);
});


// Route pour se déconnecter
app.MapPost("/matchFolio/logout", async (HttpContext http) =>
{
    var token = http.Request.Headers["Authorization"].ToString();
    var response = await authServiceClient.PostAsJsonAsync("logout", new { token });
    var result = await response.Content.ReadAsStringAsync();
    http.Response.StatusCode = (int)response.StatusCode;
    await http.Response.WriteAsync(result);
});


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
