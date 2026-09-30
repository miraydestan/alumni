var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllersWithViews();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseStaticFiles();

// Route root and about requests to HomeController views, and ensure /swagger opens Swagger UI
app.Use(async (context, next) =>
{
    var path = context.Request.Path.Value;
    if (string.IsNullOrEmpty(path) || path == "/")
    {
        context.Request.Path = "/Home/Index";
    }
    else if (path.Equals("/about", StringComparison.OrdinalIgnoreCase))
    {
        context.Request.Path = "/Home/About";
    }
    else if (path.Equals("/swagger", StringComparison.OrdinalIgnoreCase))
    {
        context.Response.Redirect("/swagger/index.html", permanent: false);
        return;
    }
    await next();
});

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllers();

app.Run();
