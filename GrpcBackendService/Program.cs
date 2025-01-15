using Azure.Identity;
using Azure.Security.KeyVault.Keys.Cryptography;
using Azure.Security.KeyVault.Keys;
using GrpcBackendService.Services;

var builder = WebApplication.CreateBuilder(args);

var credentials = new DefaultAzureCredential();
builder.Services.AddSingleton(new KeyClient(new Uri("Missing Url"), credentials));
builder.Services.AddSingleton<CryptographyClient>(sp =>
{
    var keyClient = sp.GetRequiredService<KeyClient>();
    var key = keyClient.GetKey("unit-test");
    return new CryptographyClient(key.Value.Id, credentials);
});
builder.Services.AddGrpc();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<SignService>();
app.MapGet("/", () => "Grpc service listening for requests. Please use a grpc client.");

app.Run();
