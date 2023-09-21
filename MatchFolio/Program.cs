using MatchFolio_Authentication.Model;
using MatchFolio_Profile.Model;
using System.Net.Http;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient("authServiceClient", c =>
{
    c.BaseAddress = new Uri("https://localhost:7064/");
});

builder.Services.AddHttpClient("profileServiceClient", c =>
{
    c.BaseAddress = new Uri("https://localhost:7138/"); 
});

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

// Route pour s'inscrire
app.MapPost("/matchFolio/signUp", async (IHttpClientFactory clientFactory, HttpContext http, UserEntity user) =>
{
    var authServiceClient = clientFactory.CreateClient("authServiceClient");
    var response = await authServiceClient.PostAsJsonAsync("signUp", user);
    var result = await response.Content.ReadAsStringAsync();
    http.Response.StatusCode = (int)response.StatusCode;
    await http.Response.WriteAsync(result);
});

// Route pour se connecter
app.MapPost("/matchFolio/signIn", async (IHttpClientFactory clientFactory, HttpContext http, string? mail, string? username, string password) =>
{
    var authServiceClient = clientFactory.CreateClient("authServiceClient");
    var payload = new { mail, username, password };
    var response = await authServiceClient.PostAsJsonAsync("signIn", payload);
    var result = await response.Content.ReadAsStringAsync();
    http.Response.StatusCode = (int)response.StatusCode;
    await http.Response.WriteAsync(result);
});

// Route pour se déconnecter
app.MapPost("/matchFolio/logout", async (IHttpClientFactory clientFactory, HttpContext http) =>
{
    var authServiceClient = clientFactory.CreateClient("authServiceClient");
    var token = http.Request.Headers["Authorization"].ToString();
    var response = await authServiceClient.PostAsJsonAsync("logout", new { token });
    var result = await response.Content.ReadAsStringAsync();
    http.Response.StatusCode = (int)response.StatusCode;
    await http.Response.WriteAsync(result);
});

app.MapGet("/matchFolio/userProfile/profile", async (IHttpClientFactory clientFactory, HttpContext http) =>
{
    var profileServiceClient = clientFactory.CreateClient("profileServiceClient");
    var token = http.Request.Headers["Authorization"].ToString();

    var requestMessage = new HttpRequestMessage(HttpMethod.Get, "userProfile/profile");
    requestMessage.Headers.Add("Authorization", token);
    var response = await profileServiceClient.SendAsync(requestMessage);

    var result = await response.Content.ReadAsStringAsync();
    http.Response.StatusCode = (int)response.StatusCode;
    await http.Response.WriteAsync(result);
});



app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
