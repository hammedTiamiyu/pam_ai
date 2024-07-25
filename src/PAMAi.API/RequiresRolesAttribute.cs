using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

namespace PAMAi.API;

public class RequiresRolesAttribute: AuthorizeAttribute
{
    public RequiresRolesAttribute(string roles)
    {
        AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme;
        Roles = roles;
    }
}
