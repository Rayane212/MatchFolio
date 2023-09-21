using MatchFolio_Profile.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using System.Security.Claims;
using UtilityLibraries;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "API Profile");
    });
}
app.MapGet("/userProfile/profile", async (IConfiguration _config, HttpContext http) =>
{
    try
    {
        var token = http.Request.Headers["Authorization"].ToString().Split(" ")[1];
        var claims = JwtUtils.DecodeJwt(token, _config["JwtConfig:Secret"]);
        var userId = claims[ClaimTypes.NameIdentifier];

        var userRepos = new UserReposProfile(_config);
        var userProfile = await userRepos.GetCurrentUser(userId);
        if (userProfile != null)
        {
            return Results.Ok(userProfile);
        }
        else
        {
            return Results.NotFound("Profil utilisateur non trouvé.");
        }
    }
    catch (Exception ex)
    {
        return Results.BadRequest(ex.Message);
    }
});

app.MapPost("/userProfile/updateProfile", async (IConfiguration _config, HttpContext http, UserEntityProfile user) =>
{
    try
    {
        var token = http.Request.Headers["Authorization"].ToString().Split(" ")[1];
        var claims = JwtUtils.DecodeJwt(token, _config["JwtConfig:Secret"]);
        var userId = int.Parse(claims[ClaimTypes.NameIdentifier]);


        var userRepos = new UserReposProfile(_config);
        var existingUserWithEmail = await userRepos.ExistingUserByEmail(user);
        var existingUserWithUsername = await userRepos.ExistingUserByUsername(user);

        if (existingUserWithEmail != null && existingUserWithEmail.id != userId)
        {
            return Results.BadRequest("L'email est déjà utilisé par un autre utilisateur.");
        }

        if (existingUserWithUsername != null && existingUserWithUsername.id != userId)
        {
            return Results.BadRequest("Le nom d'utilisateur est déjà utilisé par un autre utilisateur.");
        }

        var success = await userRepos.UpdateUserProfile(user);
        if (success)
        {
            return Results.Ok("Profil mis à jour avec succès.");
        }
        else
        {
            return Results.BadRequest("Erreur lors de la mise à jour du profil.");
        }
    }
    catch (Exception ex)
    {
        return Results.BadRequest(ex.Message);
    }
});

app.MapPost("/userProfile/UpdatePassword", async (IConfiguration _config, HttpContext http, string oldPassword, string newPassword, string confirmPassword) =>
{
    try
    {
        var token = http.Request.Headers["Authorization"].ToString().Split(" ")[1];
        var claims = JwtUtils.DecodeJwt(token, _config["JwtConfig:Secret"]);
        var userId = int.Parse(claims[ClaimTypes.NameIdentifier]);
        var userRepos = new UserReposProfile(_config);


        //var oldPassword = http.Request.Form["OldPassword"];
        //var newPassword = http.Request.Form["NewPassword"];
        //var confirmPassword = http.Request.Form["ConfirmPassword"];

        if (newPassword != confirmPassword)
        {
            return Results.BadRequest("Le nouveau mot de passe et la confirmation ne correspondent pas.");
        }

        var success = await userRepos.UpdatePassword(userId, oldPassword, newPassword);
        if (success)
        {
            return Results.Ok("Mot de passe modifié avec succès.");
        }
        else
        {
            return Results.BadRequest("Erreur lors de la modification du mot de passe.");
        }
    }
    catch (Exception ex)
    {
        return Results.BadRequest(ex.Message);
    }
});

app.MapDelete("/user/deleteProfile", async (IConfiguration _config, HttpContext http) =>
{
    try
    {
        var token = http.Request.Headers["Authorization"].ToString().Split(" ")[1];
        var claims = JwtUtils.DecodeJwt(token, _config["JwtConfig:Secret"]);
        var userId = int.Parse(claims[ClaimTypes.NameIdentifier]);
        var userRepos = new UserReposProfile(_config);

        var success = await userRepos.DeleteUserProfile(userId);
        if (success)
        {
            return Results.Ok("Profil supprimé avec succès.");
        }
        else
        {
            return Results.BadRequest("Erreur lors de la suppression du profil.");
        }
    }
    catch (Exception ex)
    {
        return Results.BadRequest(ex.Message);
    }
});




app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
