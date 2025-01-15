namespace MinimalBackendService.Models;

record class SignRequest
{
    public string Account { get; set; } = string.Empty;
    public string Profile { get; set; } = string.Empty;
    public string Digest { get; set; } = string.Empty;
}