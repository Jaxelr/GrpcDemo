using Azure.Identity;
using Azure.Security.KeyVault.Keys;
using Azure.Security.KeyVault.Keys.Cryptography;
using Grpc.Core;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using Google.Protobuf.WellKnownTypes;

namespace GrpcBackendService.Services
{
    public class SignService : Sign.SignBase
    {
        private readonly ILogger<SignService> _logger;
        private readonly CryptographyClient _cryptoClient;
        public SignService(ILogger<SignService> logger, CryptographyClient cryptoClient) => (_logger, _cryptoClient) = (logger, cryptoClient);

        public async override Task<SignReply> SignDigest(SignRequest request, ServerCallContext context)
        {
            Stopwatch watch = Stopwatch.StartNew();
            byte[] data = System.Text.Encoding.UTF8.GetBytes(request.Digest);

            var stream = new MemoryStream();
            stream.Write(data, 0, data.Length);

            byte[] digest = await SHA256.HashDataAsync(stream);

            SignResult signResult = await _cryptoClient.SignAsync(SignatureAlgorithm.RS256, digest);
            _logger.LogInformation($"Signing Executed for Account: {request.Account}, Profile: {request.Profile} ms{watch.ElapsedMilliseconds}");

            return new SignReply
            {
                ResponseMessage = $"Succesfully signing for Account {request.Account}, profile {request.Profile}",
                Signature = Convert.ToBase64String(signResult.Signature),
                ExecutionTime = Timestamp.FromDateTime(DateTime.UtcNow)
            };
        }

        public async override Task<HashReply> HashDigest(SignRequest request, ServerCallContext context)
        {
            Stopwatch watch = Stopwatch.StartNew();
            byte[] data = System.Text.Encoding.UTF8.GetBytes(request.Digest);
            var stream = new MemoryStream();
            stream.Write(data, 0, data.Length);
            byte[] hash = await SHA256.HashDataAsync(stream);
            _logger.LogInformation($"Hashing Executed for Account: {request.Account}, Profile: {request.Profile} ms{watch.ElapsedMilliseconds}");

            return new HashReply
            {
                ResponseMessage = $"Succesfully hashing for Account {request.Account}, profile {request.Profile}",
                Hash = Convert.ToBase64String(hash),
                ExecutionTime = Timestamp.FromDateTime(DateTime.UtcNow)
            };
        }
    }
}
