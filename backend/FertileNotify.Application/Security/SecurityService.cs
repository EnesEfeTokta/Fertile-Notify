using System;
using System.Security.Cryptography;
using System.Text;
using FertileNotify.Application.Interfaces;
using Microsoft.Extensions.Configuration;

namespace FertileNotify.Application.Security
{
    public class SecurityService : ISecurityService
    {
        private readonly string _secretKey;

        public SecurityService(IConfiguration configuration)
        {
            _secretKey = configuration["Security:UnsubscribeSecret"]!;
        }

        public string GenerateUnsubscribeToken(string email, Guid subscriberId)
        {
            var data = $"{email.ToLower()}|{subscriberId}";
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_secretKey));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
            return Convert.ToBase64String(hash).Replace("+", "-").Replace("/", "_");
        }


        public bool VerifyUnsubscribeToken(string email, Guid subscriberId, string token)
        {
            var expectedToken = GenerateUnsubscribeToken(email, subscriberId);
            return token == expectedToken;
        }
    }
}