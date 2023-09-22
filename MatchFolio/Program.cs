using MatchFolio_Authentication.Model;
using MatchFolio_Profile.Model;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using System.Text;
using UtilityLibraries;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "API Profile", Version = "v1" });

    // Ajoutez ces lignes pour configurer l'authentification JWT dans Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

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
    try
    {
        var authServiceClient = clientFactory.CreateClient("authServiceClient");
        var response = await authServiceClient.PostAsJsonAsync("signUp", user);
        var result = await response.Content.ReadAsStringAsync();
        http.Response.StatusCode = (int)response.StatusCode;
        await http.Response.WriteAsync(result);
    }
    catch (Exception ex)
    {
        http.Response.StatusCode = 500;
        await http.Response.WriteAsync(ex.Message);
    }
});

// Route pour se connecter
app.MapPost("/matchFolio/signIn", async (IHttpClientFactory clientFactory, HttpContext http, string? mail, string? username, string password) =>
{
    try
    {
        var authServiceClient = clientFactory.CreateClient("authServiceClient");
        var payload = new { mail, username, password };
        var response = await authServiceClient.PostAsJsonAsync("signIn", payload);
        var result = await response.Content.ReadAsStringAsync();
        http.Response.StatusCode = (int)response.StatusCode;
        await http.Response.WriteAsync(result);
    }
    catch (Exception ex)
    {
        http.Response.StatusCode = 500;
        await http.Response.WriteAsync(ex.Message);
    }

});

// Route pour se déconnecter
app.MapPost("/matchFolio/logout", async (IHttpClientFactory clientFactory, HttpContext http) =>
{
    try
    {
        var authServiceClient = clientFactory.CreateClient("authServiceClient");
        var token = http.Request.Headers["Authorization"].ToString();
        var response = await authServiceClient.PostAsJsonAsync("logout", new { token });
        var result = await response.Content.ReadAsStringAsync();
        http.Response.StatusCode = (int)response.StatusCode;
        await http.Response.WriteAsync(result);
    }
    catch (Exception ex)
    {
        http.Response.StatusCode = 500;
        await http.Response.WriteAsync(ex.Message);
    }

});

// Route pour récupérerle profil de l'utilisateur
app.MapGet("/matchFolio/userProfile/profile", async (IHttpClientFactory clientFactory, HttpContext http) =>
{
    try
    {
        var profileServiceClient = clientFactory.CreateClient("profileServiceClient");
        var token = http.Request.Headers["Authorization"].ToString();

        var requestMessage = new HttpRequestMessage(HttpMethod.Get, "userProfile/profile");
        requestMessage.Headers.Add("Authorization", token);
        var response = await profileServiceClient.SendAsync(requestMessage);

        var result = await response.Content.ReadAsStringAsync();
        http.Response.StatusCode = (int)response.StatusCode;
        await http.Response.WriteAsync(result);
    }
    catch (Exception ex)
    {
        http.Response.StatusCode = 500;
        await http.Response.WriteAsync(ex.Message);
    }
});

// Route pour mettre à jour le profil de l'utilisateur
app.MapPost("/matchFolio/userProfile/updateProfile", async (IHttpClientFactory clientFactory, HttpContext http, UserEntityProfile user) =>
{
    try
    {
        var profileServiceClient = clientFactory.CreateClient("profileServiceClient");
        var token = http.Request.Headers["Authorization"].ToString();

        var request = new HttpRequestMessage(HttpMethod.Post, "userProfile/updateProfile");
        request.Headers.Add("Authorization", token);
        request.Content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");

        var response = await profileServiceClient.SendAsync(request);

        var result = await response.Content.ReadAsStringAsync();
        http.Response.StatusCode = (int)response.StatusCode;
        await http.Response.WriteAsync(result);
    }
    catch (Exception ex)
    {
        http.Response.StatusCode = 500;
        await http.Response.WriteAsync(ex.Message);
    }

});

// Route pour mettre à jour le mot de passe de l'utilisateur
app.MapPost("/matchFolio/userProfile/UpdatePassword", async (IHttpClientFactory clientFactory, HttpContext http, string oldPassword, string newPassword, string confirmPassword) =>
{
    try
    {
        var profileServiceClient = clientFactory.CreateClient("profileServiceClient");
        var token = http.Request.Headers["Authorization"].ToString();

        var payload = new { oldPassword, newPassword, confirmPassword };
        var request = new HttpRequestMessage(HttpMethod.Post, "userProfile/UpdatePassword");
        request.Headers.Add("Authorization", token);
        request.Content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");

        var response = await profileServiceClient.SendAsync(request);

        var result = await response.Content.ReadAsStringAsync();
        http.Response.StatusCode = (int)response.StatusCode;
        await http.Response.WriteAsync(result);
    }
    catch (Exception ex)
    {
        http.Response.StatusCode = 500;
        await http.Response.WriteAsync(ex.Message);
    }
});

// Route pour supprimer le profil de l'utilisateur
app.MapDelete("/matchFolio/user/deleteProfile", async (IHttpClientFactory clientFactory, HttpContext http) =>
{
    try
    {
        var profileServiceClient = clientFactory.CreateClient("profileServiceClient");
        var token = http.Request.Headers["Authorization"].ToString();

        var requestMessage = new HttpRequestMessage(HttpMethod.Delete, "user/deleteProfile");
        requestMessage.Headers.Add("Authorization", token);
        var response = await profileServiceClient.SendAsync(requestMessage);

        var result = await response.Content.ReadAsStringAsync();
        http.Response.StatusCode = (int)response.StatusCode;
        await http.Response.WriteAsync(result);
    }
    catch (Exception ex)
    {
        http.Response.StatusCode = 500;
        await http.Response.WriteAsync(ex.Message);
    }
});





app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
