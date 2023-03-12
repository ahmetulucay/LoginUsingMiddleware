
using Ultimate7App.CustomMiddleware;

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.UseMyCustomMiddleware();

app.Run();



// p.26, min 9








