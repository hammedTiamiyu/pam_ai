using System.Text.Json.Serialization;
using PAMAi.Domain.Entities;

namespace PAMAi.Application.Dto.Account;

public record ReadProfileResponse
{
    /// <example>Jane</example>
    public string FirstName { get; set; } = string.Empty;

    /// <example>Doe</example>
    public string LastName { get; set; } = string.Empty;
    
    /// <example>jane.doe@gmail.com</example>
    public string Email { get; set; } = string.Empty;
    
    /// <example>janedoe</example>
    public string Username { get; set; } = string.Empty;
    
    /// <example>+2348010000000</example>
    public string PhoneNumber { get; set; } = string.Empty;

    /// <example>Esse diam et in tempor commodo aliquyam in et placerat duis at ipsum et sanctus consequat dolore.</example>
    public string Bio { get; set; } = string.Empty;

    /// <example>2</example>
    public int? GenderId { get; set; }
    
    /// <example>Female</example>
    public string? Gender { get; set; }
    
    /// <example>1</example>
    public string? HouseNumber { get; set; }
    
    /// <example>Calabar</example>
    public string? City { get; set; }
    
    /// <example>299</example>
    public int? StateId { get; set; }
    
    /// <example>Calabar</example>
    public string? State { get; set; }

    /// <example>1</example>
    public int? CountryId { get; set; } = null;
    
    /// <example>Nigeria</example>
    public string? Country { get; set; }
    
    /// <example>null</example>
    public string? CompanyName { get; set; }

    [JsonIgnore]
    public static TypeAdapterConfig FromUserProfile => TypeAdapterConfig<UserProfile, ReadProfileResponse>
        .NewConfig()
        .EnumMappingStrategy(EnumMappingStrategy.ByName)
        .Map(dest => dest.GenderId, src => src.Gender)
        .Map(dest => dest.State, src => src.State.Name)
        .Map(dest => dest.CountryId, src => src.State.CountryId)
        .Map(dest => dest.Country, src => src.State.Country.Name)
        .Config;
}
