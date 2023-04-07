using Microsoft.EntityFrameworkCore;
using PlatformService.AsyncDataServices;
using PlatformService.Data;
using PlatformService.SyncDataServices.Grpc;
using PlatformService.SyncDataServices.Http;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

if(builder.Environment.IsDevelopment()){
    Console.WriteLine("--> Using InMem Database");
    builder.Services.AddDbContext<AppDbContext>(opt => opt.UseInMemoryDatabase("InMem"));
}else{
    //Production
    Console.WriteLine("--> Using SQL Server Database");
    builder.Services.AddDbContext<AppDbContext>(opt => 
        opt.UseSqlServer(builder.Configuration.GetConnectionString("PlatformsConn"))
    );
}

builder.Services.AddScoped<IPlatformRepo, PlatformRepo>();
builder.Services.AddSingleton<IMessageBusClient, MessageBusClient>();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddGrpc();
//bypass ssl
builder.Services.AddHttpClient<ICommandDataClient, HttpCommandDataClient>()
    .ConfigurePrimaryHttpMessageHandler(() =>
    {
        var handler = new HttpClientHandler();
        handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; };
        return handler;
    });

//Show endpoints
//CommandService Endpoint
Console.WriteLine("CommandService Endpoint: " + builder.Configuration["CommandsService"]);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.MapGrpcService<GrpcPlatformService>();
app.MapGet("/protos/platforms.proto", async context =>
{
    var platformProto = File.ReadAllText("Protos/platforms.proto");
    await context.Response.WriteAsync(platformProto);
});

PrepDb.PrepPopulation(app, builder.Environment.IsProduction());

app.Run();
