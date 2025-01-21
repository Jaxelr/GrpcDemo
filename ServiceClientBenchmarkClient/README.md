# Benchmark results for GRPC operation vs HTTP Client operation (from localhost)

Dont take this benchmarks as gospel, they are just a simple comparison of the performance of GRPC vs HTTP Client in a simple operation. Note: HttpClient is incorrectly used, see here for [proper guidance](https://learn.microsoft.com/en-us/dotnet/fundamentals/networking/http/httpclient-guidelines).

```

BenchmarkDotNet v0.14.0, Windows 11 (10.0.26100.2894)
11th Gen Intel Core i7-1185G7 3.00GHz, 1 CPU, 8 logical and 4 physical cores
.NET SDK 9.0.102
  [Host]    : .NET 9.0.1 (9.0.124.61010), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  MediumRun : .NET 9.0.1 (9.0.124.61010), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI

Job=MediumRun  IterationCount=15  LaunchCount=2  
WarmupCount=10  

```
| Method                    | Mean      | Error     | StdDev    | StdErr   | Min        | Max       | Op/s   | Allocated |
|-------------------------- |----------:|----------:|----------:|---------:|-----------:|----------:|-------:|----------:|
| GrpcClientHashDigestAsync |  10.11 ms |  0.578 ms |  0.866 ms | 0.158 ms |   8.434 ms |  11.91 ms | 98.960 |  51.39 KB |
| HttpClientHashDigestAsync | 178.41 ms | 22.761 ms | 33.363 ms | 6.195 ms | 131.926 ms | 277.51 ms |  5.605 |  26.38 KB |
