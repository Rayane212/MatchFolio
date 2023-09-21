using MatchFolio_Authentication.Model;
using System.Data.SqlClient;
using System.Security.Claims;
using Newtonsoft.Json;
using UtilityLibraries;

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
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "API Authentication");
    });
}


app.MapPost("/signIn", async (IConfiguration _config, HttpContext http, SignInModel model) =>
{
    try
    {
        // Récupération des identifiants de l'utilisateur
        string? mail = model.Mail;
        string? username = model.Username;
        string password = model.Password;

        // Vérifier que les identifiants sont valides
        using var connection = new SqlConnection(builder.Configuration.GetConnectionString("SQL"));
        var userRepos = new UserRepos(_config);
        var user = await userRepos.GetUserAuthAsync(mail, username, password);
        if (user == null)
        {
            http.Response.StatusCode = 401; // Code HTTP 401 Unauthorized
            await http.Response.WriteAsync("Adresse email ou mot de passe incorrect.");
            return;
        }

        // Création du token d'authentification
        var jwtUtils = new JwtUtils(_config);
        string tokenString = jwtUtils.CreateToken(user);

        http.Response.Headers.Add("Authorization", "Bearer " + tokenString);

        // Retourner le token d'authentification dans la réponse du serveur
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
    var userId = user.username.ToUpper();
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
        await http.Response.WriteAsync($"Utilisateur {userId} a été déconnecté.");
    }
    else
    {
        http.Response.StatusCode = 409;
        await http.Response.WriteAsync("Il y'a eu un problème.");
        return;
    }
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
