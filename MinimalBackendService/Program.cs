using Azure.Identity;
using Azure.Security.KeyVault.Keys;
using Azure.Security.KeyVault.Keys.Cryptography;
using Microsoft.Identity.Client.Platforms.Features.DesktopOs.Kerberos;
using System.Diagnostics;
using System.Net;
using System.Security.Cryptography;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var credentials = new DefaultAzureCredential();
builder.Services.AddSingleton(new KeyClient(new Uri("Missing Url"), credentials));
builder.Services.AddSingleton<CryptographyClient>(sp =>
{
    var keyClient = sp.GetRequiredService<KeyClient>();
    var key = keyClient.GetKey("unit-test");
    return new CryptographyClient(key.Value.Id, credentials);
});
builder.Services.AddLogging();

var app = builder.Build();

app.UseHttpsRedirection();
app.MapGet("/", () => "Http service listening for requests. Please use an http client to invoke.");
app.MapPost("/sign", async (SignRequest request, CryptographyClient cryptoClient, ILogger<Program> logger) =>
{
    Stopwatch watch = Stopwatch.StartNew();
    byte[] data = Encoding.UTF8.GetBytes(request.Digest!);

    using var stream = new MemoryStream();
    stream.Write(data, 0, data.Length);

    byte[] digest = await SHA256.HashDataAsync(stream);

    SignResult signResult = await cryptoClient.SignAsync(SignatureAlgorithm.RS256, digest);
    logger.LogInformation($"Signing Executed for Account: {request.Account}, Profile: {request.Profile} ms{watch.ElapsedMilliseconds}");

    return Results.Ok(new SignResponse
    {
        Message = $"Successfully signing for Account {request.Account}, request {request.Profile}",
        Signature = Convert.ToBase64String(signResult.Signature),
        ExecutionTime = DateTime.UtcNow
    });
});

app.MapPost("/hash", async (SignRequest request, CryptographyClient cryptoClient, ILogger<Program> logger) =>
{
    Stopwatch watch = Stopwatch.StartNew();
    byte[] data = Encoding.UTF8.GetBytes(request.Digest!);

    using var stream = new MemoryStream();
    stream.Write(data, 0, data.Length);

    byte[] digest = await SHA256.HashDataAsync(stream);

    logger.LogInformation($"Signing Executed for Account: {request.Account}, Profile: {request.Profile} ms{watch.ElapsedMilliseconds}");

    return Results.Ok(new HashResponse
    {
        Message = $"Successfully hashing for Account {request.Account}, request {request.Profile}",
        Hash = Convert.ToBase64String(digest),
        ExecutionTime = DateTime.UtcNow
    });
});

app.Run();

public record SignRequest
{
    public string? Account { get; set; }
    public string? Profile { get; set; }
    public string? Digest { get; set; }
}

public record SignResponse
{
    public string? Message { get; set; }
    public string? Signature { get; set; }
    public DateTime ExecutionTime { get; set; }
}

public record HashResponse
{
    public string? Message { get; set; }
    public string? Hash { get; set; }
    public DateTime ExecutionTime { get; set; }
}