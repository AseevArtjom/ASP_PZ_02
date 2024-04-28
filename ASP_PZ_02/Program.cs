var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.UseStaticFiles();

var users = new[]
{
    new { username = "admin", password = "admin123" },
    new { username = "user", password = "user123" },
    new { username = "user2", password = "user2123" }
};


app.Use(async (context, next) =>
{
    var isLoggedIn = context.Request.Cookies["IsLoggedIn"];
    if (context.Request.Path == "/" && (string.IsNullOrEmpty(isLoggedIn) || isLoggedIn != "true"))
    {
        context.Response.Redirect("/login");
        return;
    }

    await next();
});

app.MapGet("/", async context =>
{
    string username = context.Request.Cookies["Username"];

    await context.Response.WriteAsync(
        $"<div class=\"container\">" +
            $"<h2>Hello {username}</h2>" +
            $"<form action=\"/logout\" method=\"post\">" +
                $"<button type=\"submit\">Log out</button>" +
            $"</form>"+
        $"</div>"
        );
});

app.MapPost("/logout", async context =>
{
    context.Response.Cookies.Delete("IsLoggedIn");
    context.Response.Redirect("/login");
});

app.MapGet("/login", async context =>
{
    await context.Response.WriteAsync(
        $"<!DOCTYPE html>" +
        $"<html lang=\"en\">" +
        $"<head>" +
        $"<meta charset=\"UTF-8\">" +
        $"<link type=\"text/css\" rel=\"stylesheet\" href=\"./src/style.css\">" +
        $"<title>PZ_01</title>" +
        $"</head>" +
        $"<body>" +
        $"<form class=\"login\" action=\"/login\" method=\"post\">" + 
            $"<div>" +
                $"<input type=\"login\" name=\"login\">" +
                $"<input type=\"password\" name=\"password\">" +
            $"</div>" + 
            $"<button type=\"submit\">Log in</button>" +
        $"</form>" +
        $"</body>" +
        $"</html>");
});

app.MapPost("/login", async context =>
{
    string username = context.Request.Form["login"];
    string password = context.Request.Form["password"];

    bool isAuth = false;

    foreach (var user in users)
    {
        if (username == user.username && password == user.password)
        {
            context.Response.Cookies.Append("Username", username);
            context.Response.Cookies.Append("IsLoggedIn", "true");
            context.Response.Redirect("/");
            break;
        }
    }

    if (!isAuth)
    {
        await context.Response.WriteAsync(
               $"<!DOCTYPE html>" +
               $"<html lang=\"en\">" +
               $"<head>" +
               $"<meta charset=\"UTF-8\">" +
               $"<link type=\"text/css\" rel=\"stylesheet\" href=\"./src/style.css\">" +
               $"<title>PZ_01</title>" +
               $"</head>" +
               $"<body>" +
               $"<p class=\"error-message\">Username or password is incorrect.</p>" +
               $"<form class=\"login\" action=\"/login\" method=\"post\">" +
                   $"<div>" +
                       $"<input type=\"login\" name=\"login\">" +
                       $"<input type=\"password\" name=\"password\">" +
                   $"</div>" +
                   $"<button type=\"submit\">Log in</button>" +
               $"</form>" +
               $"</body>" +
               $"</html>");
    }
    
});



app.Run();