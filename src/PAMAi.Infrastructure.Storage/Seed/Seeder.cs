using Microsoft.Extensions.Logging;
using PAMAi.Application.Services.Interfaces;
using PAMAi.Application.Storage;

namespace PAMAi.Infrastructure.Storage.Seed;
internal sealed partial class Seeder
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<Seeder> _logger;
    private readonly ICountryService _countryService;

    public Seeder(IUnitOfWork unitOfWork, ILogger<Seeder> logger, ICountryService countryService)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _countryService = countryService;
    }
}
