
using Microsoft.Extensions.Primitives;
using System.Runtime.CompilerServices;

namespace Ultimate7App.CustomMiddleware;

public class MyCustomMiddleware
{
    public readonly RequestDelegate _next;

    public MyCustomMiddleware(RequestDelegate next)
    {
        _next = next;
    }
    public async Task InvokeAsync(HttpContext context)
    {
        if ((context.Request.Method == "POST") && (context.Request.Path == "/"))
        {
            //Stream
            StreamReader streamReader = new StreamReader(context.Request.Body);
            string stringBody = await streamReader.ReadToEndAsync();

            //Parsing into dictionary
            Dictionary<string, StringValues> dict = new Dictionary<string, StringValues>();
            dict = Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(stringBody);

            string? email = null;
            string? password = null;

            //Email input
            if (dict.ContainsKey("email"))
            {
                email = Convert.ToString(dict["email"][0]);
            }
            else
            {
                context.Response.StatusCode = 400;
                await context.Response.WriteAsync("Invalid email\n");
            }

            //Password input
            if (dict.ContainsKey("password"))
            {
                password = Convert.ToString(dict["password"][0]);
            }
            else
            {
                context.Response.StatusCode = 400;
                await context.Response.WriteAsync("Invalid password");
            }

            //
            if ((!(String.IsNullOrEmpty(email) == true)) && (!(String.IsNullOrEmpty(password) == true)))
            {

                //Example #1 - Valid email and password are submitted
                if ((email == "admin@example.com") && (password == "admin1234"))
                {
                    context.Response.StatusCode = 200;
                    await context.Response.WriteAsync("Valid login");
                }

                //Example #2 - Either email or password is incorrect
                else if ((email != "manager@example.com") || (password != "manager-password"))
                {
                    context.Response.StatusCode = 400;
                    await context.Response.WriteAsync("Valid login");
                }

                else
                {
                    context.Response.StatusCode = 400;
                    await context.Response.WriteAsync("Invalid login");
                }
            }

            //Example #3 - Neither email and password is submitted
            else if ((String.IsNullOrEmpty(email) == true) && (String.IsNullOrEmpty(password) == true))
            {
                context.Response.StatusCode = 400;
                await context.Response.WriteAsync("Invalid input for 'email'\nInvalid input for 'password'");
            }

            //Example #4 - Valid email ("test@example.com") and password is not submitted
            else if ((String.IsNullOrEmpty(email) != true) && (String.IsNullOrEmpty(password) == true))
            {
                context.Response.StatusCode = 400;
                await context.Response.WriteAsync("Invalid input for 'password'");
            }

            //Example #5 - Valid password ("1234") and email is not submitted
            else if ((String.IsNullOrEmpty(email) == true) && (String.IsNullOrEmpty(password) != true))
            {
                context.Response.StatusCode = 400;
                await context.Response.WriteAsync("Invalid input for 'email'");
            }
            else
            {
                context.Response.StatusCode = 400;
                await context.Response.WriteAsync("Please fill out missing values.");
            }
        }
        else if ((context.Request.Method == "GET") && (context.Request.Path == "/"))
        {
            context.Response.StatusCode = 200;
            await context.Response.WriteAsync("No response");
        }
        else
        {
            await _next(context);
        }
    }
}

public static class CustomMiddlewareExtension
{
    public static IApplicationBuilder UseMyCustomMiddleware(this IApplicationBuilder app)
    {
        return app.UseMiddleware<MyCustomMiddleware>();
    }
}
