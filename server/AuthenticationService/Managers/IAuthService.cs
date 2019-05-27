using AuthenticationService.Models;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace AuthenticationService.Managers
{
    public interface IAuthService
    {
        string SecretKey { get; set; }

        bool IsValidKey(string token);
        string GenerateToken(IAuthContainerModel model);
        IEnumerable<Claim> GetTokenClaims(string token);
    }
}
