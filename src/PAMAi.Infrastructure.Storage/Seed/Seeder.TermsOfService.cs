using Microsoft.Extensions.Logging;
using PAMAi.Application;
using PAMAi.Application.Dto.TermsOfService;

namespace PAMAi.Infrastructure.Storage.Seed;

internal sealed partial class Seeder
{
    internal async Task SeedDefaultTermsOfServiceAsync()
    {
        _logger.LogInformation("Seeding default Terms of Service");
        string file = "Terms of Service.txt";
        string filePath = Path.Combine(AppContext.BaseDirectory, file);

        if (!File.Exists(filePath))
        {
            _logger.LogWarning("Could not seed Terms of Service. File '{File}' not found", file);
            return;
        }

        var content = await File.ReadAllTextAsync("Terms of Service.txt");
        CreateTermsOfServiceRequest termsOfService = new()
        {
            Content = content,
            EffectiveFromUtc = DateTime.Now,
            Version = 1.0,
        };
        Result result = await _legalService.CreateTermsOfServiceAsync(termsOfService);
        if (result.IsSuccess)
        {
            _logger.LogInformation("Default Terms of Service seeded");
            return;
        }

        _logger.LogWarning("Default Terms of Service not seeded. Result: {@Result}", result);
    }
}
