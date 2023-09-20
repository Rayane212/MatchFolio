using MatchFolio_Authentication.Model;
using System.Data.SqlClient;
using Dapper;
using System.Security.Claims;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using MatchFolio_Authentication.Utils;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


// SignIn 
app.MapPost("/signIn", async (IConfiguration _config, HttpContext http, string? mail, string? username, string password) =>  // Pour tester en front supprimer les var username et password des param�tre et mette la route en post 
{
    try
    {
        // R�cup�ration des identifiants de l'utilisateur
        //string username = http.Request.Form["Username"];
        //string password = http.Request.Form["Password"];

        // V�rifier que les identifiants sont valides
        using var connection = new SqlConnection(builder.Configuration.GetConnectionString("SQL"));
        var userRepos = new UserRepos(_config);
        var user = await userRepos.GetUserAuthAsync(mail, username, password);
        if (user == null)
        {
            http.Response.StatusCode = 401; // Code HTTP 401 Unauthorized
            await http.Response.WriteAsync("Adresse email ou mot de passe incorrect.");
            return;
        }

        // Cr�ation du token d'authentification
        var jwtUtils = new JwtUtils(_config);
        string tokenString = jwtUtils.CreateToken(user);

        http.Response.Headers.Add("Authorization", "Bearer " + tokenString);


        // Retourner le token d'authentification dans la r�ponse du serveur
        http.Response.StatusCode = 200;
        http.Response.ContentType = "application/json";
        await http.Response.WriteAsync(JsonConvert.SerializeObject(new { Token = tokenString }));
    }
    catch (Exception ex)
    {
        http.Response.StatusCode = 400; // Code HTTP 400 Bad Request
        await http.Response.WriteAsync(ex.Message);
        return;
    }
});

// SignUp

app.MapPost("/signUp", async (IConfiguration _config, HttpContext http, UserEntity user) =>
{
    var userId = user.NameUser.ToUpper();
    using var connection = new SqlConnection(builder.Configuration.GetConnectionString("SQL"));
    var userRepos = new UserRepos(_config);
    var existingUser = await userRepos.ExistingUser(user);
    if (existingUser != null)
    {
        http.Response.StatusCode = 409; // Code HTTP 409 Conflict
        await http.Response.WriteAsync("");
        return;
    }
    await userRepos.InsertUserAsync(user, userId);

    http.Response.StatusCode = 200; // Code HTTP 200 OK
    await http.Response.WriteAsync($"{userId} user successfully created.");
});


// logout
app.MapPost("/logout", async (HttpContext http, IConfiguration _config) =>
{
    var token = http.Request.Headers["Authorization"].ToString().Split(" ")[1];
    var claims = JwtUtils.DecodeJwt(token, _config["JwtConfig:Secret"]);
    var userId = claims[ClaimTypes.NameIdentifier];
    if (token != "")
    {
        http.Request.Headers.Remove("Authorization");
        http.Response.Redirect("/");
        http.Response.StatusCode = 200;
        await http.Response.WriteAsync($"Utilisateur {userId} a �t� d�connect�.");
    }
    else
    {
        http.Response.StatusCode = 409;
        await http.Response.WriteAsync("Il y'a eu un probl�me.");
        return;
    }
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
