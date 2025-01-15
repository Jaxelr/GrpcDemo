using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using Grpc.Net.Client;
using GrpcClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ServiceClientBenchmarkClient;

[BenchmarkCategory("ApiClient")]
[AllStatisticsColumn]
[MemoryDiagnoser]
[HideColumns("Q1", "Q3", "Median", "RatioSD")]
[MediumRunJob]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[MarkdownExporterAttribute.GitHub]
public class Benchmarks
{
    private HttpClient? _httpClient;
    private GrpcChannel? _grpcChannel;
    private GrpcClient.Sign.SignClient? _grpcClient;

    public Benchmarks()
    {
    }

    [Benchmark]
    public async Task<HashResponse?> HttpClientHashDigestAsync()
    {
        _httpClient = new HttpClient() { BaseAddress = new Uri("https://localhost:7017") };

        var request = new SignRequest
        {
            Account = Convert.ToBase64String(GenerateRandomBytes(16)),
            Profile = Convert.ToBase64String(GenerateRandomBytes(16)),
            Digest = Convert.ToBase64String(GenerateRandomBytes(32))
        };
        var json = JsonSerializer.Serialize(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync("/hash", content);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<HashResponse>();
    }

    [Benchmark]
    public async Task<HashReply?> GrpcClientHashDigestAsync()
    {
        _grpcChannel = GrpcChannel.ForAddress("https://localhost:7105");
        _grpcClient = new GrpcClient.Sign.SignClient(_grpcChannel);

        var reply = await _grpcClient.HashDigestAsync(new GrpcClient.SignRequest
        {
            Account = Convert.ToBase64String(GenerateRandomBytes(16)),
            Profile = Convert.ToBase64String(GenerateRandomBytes(16)),
            Digest = Convert.ToBase64String(GenerateRandomBytes(32))
        });

        return reply;
    }

    private static byte[] GenerateRandomBytes(int length)
    {
        byte[] bytes = new byte[length];
        using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(bytes);
        }
        return bytes;
    }
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
    }
    public record HashResponse
    {
        public string? Message { get; set; }
        public string? Hash { get; set; }
    }
}
