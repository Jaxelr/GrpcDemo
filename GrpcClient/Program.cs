using Grpc.Net.Client;
using GrpcClient;
using System.Diagnostics.Metrics;

using var channel = GrpcChannel.ForAddress("https://localhost:7105");
var client = new Sign.SignClient(channel);

var reply = await client.HashDigestAsync(new SignRequest
{
    Account = "Jaxel",
    Profile = "Public",
    Digest = "Hello world message"
});

Console.WriteLine($"Reply: {reply.ResponseMessage}");
Console.WriteLine($"Hash: {reply.Hash}");
Console.WriteLine($"ExecutionTime {reply.ExecutionTime.ToDateTime()}");
Console.ReadLine();

//var reply = await client.SignDigestAsync(new SignRequest()
//{
//    Account = "Jaxel",
//    Profile = "Public",
//    Digest = $"Hello world message"
//});

//Console.WriteLine($"Reply: {reply.ResponseMessage}");
//Console.WriteLine($"Signature: {reply.Signature}");
