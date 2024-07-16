using Microsoft.Extensions.Logging;
using PAMAi.Application.Storage;

namespace PAMAi.Infrastructure.Storage.Seed;
internal sealed partial class Seeder
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<Seeder> _logger;

    public Seeder(IHttpClientFactory httpClientFactory, IUnitOfWork unitOfWork, ILogger<Seeder> logger)
    {
        _httpClientFactory = httpClientFactory;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }
}
