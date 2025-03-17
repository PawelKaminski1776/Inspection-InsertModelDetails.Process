using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using System.Globalization;
using InspectionInsertModelDetails.Handlers;
using InspectionInsertModelDetails.Controllers.DtoFactory;
using InspectionInsertModelDetails.Process;
using InspectionInsertModelDetails.Channel.Services;
using InspectionInsertModelDetails.Messages.Dtos;
using InspectionUserDetails.Channel;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<AppConfiguration>(_ => AppConfiguration.Instance);
builder.Services.AddSingleton<IDtoFactory, DtoFactory>();

var appConfig = AppConfiguration.Instance;

if (builder.Environment.IsDevelopment())
{
    builder.WebHost.ConfigureKestrel(options =>
    {
        options.ListenAnyIP(5007);
        options.ListenAnyIP(5008, listenOptions =>
        {
            listenOptions.UseHttps();
        });
    });
}
else
{
    builder.WebHost.ConfigureKestrel(options =>
    {
        options.ListenAnyIP(5007);
    });
}

builder.Services.AddScoped<MongoConnect>(provider =>
{
    var connectionString = appConfig.GetSetting("ConnectionStrings:DefaultConnection");
    return new MongoConnect(connectionString);
});

builder.Services.AddScoped<InsertModelDetailsService>(provider =>
{
    var connectionString = appConfig.GetSetting("ConnectionStrings:DefaultConnection");
    return new InsertModelDetailsService(connectionString);
});

builder.Services.AddScoped<InsertModelDetailsHandler>();


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});

var endpointConfiguration = new EndpointConfiguration("NServiceBusHandlers");
string instanceId = Environment.MachineName;
endpointConfiguration.MakeInstanceUniquelyAddressable(instanceId);
endpointConfiguration.EnableCallbacks();

var settings = new JsonSerializerSettings
{
    TypeNameHandling = TypeNameHandling.Auto,
    Converters =
    {
        new IsoDateTimeConverter
        {
            DateTimeStyles = DateTimeStyles.RoundtripKind
        }
    }
};
var serialization = endpointConfiguration.UseSerialization<NewtonsoftJsonSerializer>();
serialization.Settings(settings);

var transport = endpointConfiguration.UseTransport<LearningTransport>();
transport.StorageDirectory("/app/.learningtransport");
var persistence = endpointConfiguration.UsePersistence<LearningPersistence>();

var routing = transport.Routing();
routing.RouteToEndpoint(typeof(ModelDetailsRequest), "NServiceBusHandlers");

var scanner = endpointConfiguration.AssemblyScanner().ScanFileSystemAssemblies = true;

builder.UseNServiceBus(endpointConfiguration);

var app = builder.Build();
app.UseRouting();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
        c.RoutePrefix = string.Empty;
    });
}
app.UseCors("AllowAll");
app.UseMiddleware<LoggingMiddleware>();
app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.Run();
