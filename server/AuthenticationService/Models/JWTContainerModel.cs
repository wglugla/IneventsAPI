using System;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace AuthenticationService.Models
{
    public class JWTContainerModel : IAuthContainerModel
    {
        #region Public Methods
        public string SecretKey { get; set; }
        public string SecurityAlgorithm { get; set; } = SecurityAlgorithms.HmacSha256Signature;
        public int ExpireMinutes { get; set; } = 10080; // 7 days
        public Claim[] Claims { get; set; }
        #endregion
    }
}
