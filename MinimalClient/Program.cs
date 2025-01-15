using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

var request = new SignRequest
{
    Account = "Jaxel",
    Profile = "Public",
    Digest = "Hello world message"
};

var json = JsonSerializer.Serialize(request);
var content = new StringContent(json, Encoding.UTF8, "application/json");

using var client = new HttpClient() { BaseAddress = new Uri("https://localhost:7017") };

var response = await client.PostAsync("/hash", content);

response.EnsureSuccessStatusCode();

var signResponse = await response.Content.ReadFromJsonAsync<HashResponse>();

Console.WriteLine($"Message: {signResponse?.Message!}");
Console.WriteLine($"Hash: {signResponse?.Hash!}");
Console.WriteLine($"ExecutionTime: {signResponse?.ExecutionTime}");
Console.ReadLine();


//var response = await client.PostAsync("/sign", content);

//response.EnsureSuccessStatusCode();

//var signResponse = await response.Content.ReadFromJsonAsync<SignResponse>();

//Console.WriteLine($"Message: {signResponse?.Message!}");
//Console.WriteLine($"Signature: {signResponse?.Signature!}");


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