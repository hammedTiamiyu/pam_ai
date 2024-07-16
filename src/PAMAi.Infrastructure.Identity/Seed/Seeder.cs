using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using PAMAi.Application.Services.Interfaces;
using PAMAi.Application.Storage;
using PAMAi.Infrastructure.Identity.Models;

namespace PAMAi.Infrastructure.Identity.Seed;
internal sealed partial class Seeder
{
    private readonly ILogger<Seeder> _logger;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<User> _userManager;
    private readonly IAccountService _accountService;
    private readonly IUnitOfWork _unitOfWork;

    public Seeder(
        ILogger<Seeder> logger,
        RoleManager<IdentityRole> roleManager,
        UserManager<User> userManager,
        IAccountService accountService,
        IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _roleManager = roleManager;
        _userManager = userManager;
        _accountService = accountService;
        _unitOfWork = unitOfWork;
    }
}
