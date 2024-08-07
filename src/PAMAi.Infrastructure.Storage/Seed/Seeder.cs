using Microsoft.Extensions.Logging;
using PAMAi.Application.Services.Interfaces;
using PAMAi.Application.Storage;

namespace PAMAi.Infrastructure.Storage.Seed;
internal sealed partial class Seeder
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<Seeder> _logger;
    private readonly ICountryService _countryService;
    private readonly ILegalService _legalService;

    public Seeder(IUnitOfWork unitOfWork, ILogger<Seeder> logger, ICountryService countryService, ILegalService legalService)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _countryService = countryService;
        _legalService = legalService;
    }
}
