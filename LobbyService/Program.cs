using LobbyService.Services.LobbyCode;
using LobbyService;
using LobbyService.Hubs;
using LobbyService.Hubs.ConnectionMapping;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddControllers();

builder.Services.AddSignalR();

builder.Services.AddSingleton<LobbyCodeService>();
builder.Services.AddSingleton<ConnectionMappingService>();
builder.Services.AddSingleton<ILobbyActionService, LobbyActionManager>();
builder.Services.AddSingleton<ILobbyService, LobbyManager>();

builder.Services.AddCors(options => {
    options.AddPolicy("AllowAll", policy => {
        policy.WithOrigins("http://10.0.2.2", "http://localhost")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();
if (app.Environment.IsDevelopment())
{
    //app.UseSwagger();
    //app.UseSwaggerUI();
}

app.UseCors("AllowAll");

//app.UseHttpsRedirection();
app.MapControllers();
app.MapHub<LobbyHub>("/lobbyHub");

app.Run();
