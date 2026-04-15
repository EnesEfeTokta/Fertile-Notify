using System;

namespace FertileNotify.Domain.ValueObjects
{
    public class CustomUrl
    {
        public string Value { get; private set; } = string.Empty;

        private CustomUrl() { }

        private CustomUrl(string value)
        {
            Value = value;
        }

        public static CustomUrl Create(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentException("URL cannot be empty.");

            if (!Uri.TryCreate(url, UriKind.Absolute, out var uriResult) ||
                (uriResult.Scheme != Uri.UriSchemeHttp && uriResult.Scheme != Uri.UriSchemeHttps))
            {
                throw new ArgumentException("Invalid URL format.");
            }

            return new CustomUrl(url);
        }
    }
}
