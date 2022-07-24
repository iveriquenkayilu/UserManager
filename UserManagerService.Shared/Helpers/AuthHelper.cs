﻿using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using UAParser;
using UserManagerService.Shared.Interfaces.Shared;
using UserManagerService.Shared.Models.Company;
using UserManagerService.Shared.Models.User;
using UserManagerService.Shared.Settings;

namespace OBS.UserManagementService.Domain.Helpers
{
    public class AuthHelper : IAuthHelper
    {
        private readonly WebProtocolSettings _webProtocolSettings;
        private readonly IMemoryCache _cache;
        private readonly IHttpContextAccessor _httpContext;
        private readonly ILogger<AuthHelper> _logger;
        public AuthHelper(IOptions<WebProtocolSettings> webProtocolSettings, IMemoryCache cache, IHttpContextAccessor httpContext, ILogger<AuthHelper> logger)
        {
            if (webProtocolSettings.Value.AccessTokenExpiresInMinutes <= 0)
                throw new Exception("Access Token cannot expire after a nonpositive minute value");
            if (webProtocolSettings.Value.AccessTokenExpiresInMinutes > webProtocolSettings.Value.RefreshTokenExpiresInMinutes)
                throw new Exception("Refresh Token cannot expire before access token");

            _webProtocolSettings = webProtocolSettings.Value;
            _cache = cache;
            _httpContext = httpContext;
            _logger = logger;
        }

        public AuthTokenModel CreateSecurityToken(Guid userId, string username, List<string> roles, CompanyShortModel company)
        {
            if (string.IsNullOrEmpty(username))
            {
                return null;
            }

            // 1. Create Security Token Handler
            var tokenHandler = new JwtSecurityTokenHandler();

            // 2. Create Private Key to Encrypted
            var tokenKey = Encoding.ASCII.GetBytes(_webProtocolSettings.EncryptionKey);

            //3. Create JETdescriptor
            var claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.Name, username));
            claims.Add(new Claim(ClaimTypes.NameIdentifier, userId.ToString()));

            claims.Add(new Claim("CompanyId", company == null ? Guid.Empty.ToString() : company.Id.ToString()));
            claims.Add(new Claim("CompanyName", company == null ? "" : company.Name));

            roles.ForEach(r =>
            {
                //claims.Add(new Claim(ClaimTypes.Role, r.NormalizedName));
                claims.Add(new Claim(ClaimTypes.Role, r));
            });

            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(_webProtocolSettings.AccessTokenExpiresInMinutes),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256Signature)
            };
            //4. Create Token
            var token = tokenHandler.CreateToken(tokenDescriptor);

            // create refresh token
            var refreshToken = GenerateAndCacheRefreshToken(userId);

            // 5. Return Token from method
            return new AuthTokenModel { AccessToken = tokenHandler.WriteToken(token), RefreshToken = refreshToken, Duration = _webProtocolSettings.AccessTokenExpiresInMinutes };
        }

        public RefreshTokenModel GetCachedRefreshTokenWithRequestIpValidation(string refreshToken)
        {
            var token = _cache.Get<RefreshTokenModel>(refreshToken);
            if (token is null)
                return null;

            var requestIpAddress = GetRequestIPAddress();
            if (token.IpAddress != requestIpAddress)
            {
                _logger.LogCritical($"Security: expected refresh token {refreshToken} (user {token.UserId}) and ip {token.IpAddress}. Request IP: {requestIpAddress}");
                return null;
            }

            if (token.Expires < DateTime.UtcNow)
            {
                _logger.LogInformation($"User tried to refresh tokens with an expired refresh token {refreshToken}. ip {token.IpAddress}, expired at {token.Expires}");
                RevokeCachedRefreshToken(refreshToken);
                return null;
            }
            return token;
        }

        public bool RevokeCachedRefreshToken(string refreshToken)
        {
            _cache.Remove(refreshToken);
            return _cache.Get(refreshToken) is null ? true : false;
        }

        public string GetRequestIPAddress()
        {
            var ipAddress = _httpContext.HttpContext.Connection.RemoteIpAddress;

            if (ipAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
                ipAddress = System.Net.Dns.GetHostEntry(ipAddress).AddressList
                    .First(x => x.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);

            return ipAddress.ToString();
        }

        public  VisitorModel GetVisitorInfo()
        {
            var visitor = new VisitorModel();
            try
            {

                var userAgent = _httpContext.HttpContext.Request.Headers["User-Agent"];

                var parser = Parser.GetDefault();
                var operatingSystem = parser.ParseOS(userAgent).ToString();
                var accessType = parser.ParseUserAgent(userAgent).ToString();
                var device = parser.ParseDevice(userAgent).ToString();
                var addressIp = GetRequestIPAddress();

                visitor = new VisitorModel
                {
                    AccessType = accessType,
                    AddressIp = addressIp,
                    Device = device,
                    OperatingSystem = operatingSystem,
                };
            }
            catch (Exception e)
            {
                _logger.LogError("Failed  to get visitor info", e.Message,e);
            };

            return visitor;
        }
        private string GenerateAndCacheRefreshToken(Guid userId)
        {
            var refreshToken = "";
            using (var rngCryptoServiceProvider = new RNGCryptoServiceProvider())
            {
                var randomBytes = new byte[64];
                rngCryptoServiceProvider.GetBytes(randomBytes);
                refreshToken = Convert.ToBase64String(randomBytes);
            }

            _cache.Set<RefreshTokenModel>(refreshToken, new RefreshTokenModel
            {
                UserId = userId,
                IpAddress = GetRequestIPAddress(),
                Expires = DateTime.UtcNow.AddMinutes(_webProtocolSettings.RefreshTokenExpiresInMinutes),
                Created = DateTime.UtcNow,
            });
            return refreshToken;
        }
    }
}