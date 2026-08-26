using TicTacToeApi.Services;
using TicTacToeApi.Hubs;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<GameManager>();
builder.Services.AddSignalR();
builder.Services.AddCors(options =>
{
  options.AddPolicy("AllowAll", policy =>
  {
    policy.WithOrigins("null").AllowAnyHeader().AllowAnyMethod().AllowCredentials(); ;
  });
});
var app = builder.Build();
app.UseCors("AllowAll");
app.MapHub<TicTacToeHub>("/gameHub");
app.Run(); 