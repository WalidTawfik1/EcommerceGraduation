namespace EcommerceGraduation.API.Helper
{
    using System.Collections.Concurrent;

    namespace EcommerceGraduation.API.Helper
    {
        public static class RedirectUrlStore
        {
            private static readonly ConcurrentDictionary<string, string> _redirectUrls = new();

            public static void StoreRedirectUrl(string orderNumber, string redirectUrl)
            {
                _redirectUrls.AddOrUpdate(orderNumber, redirectUrl, (key, oldValue) => redirectUrl);
            }

            public static string GetRedirectUrl(string orderNumber)
            {
                _redirectUrls.TryGetValue(orderNumber, out var redirectUrl);
                return redirectUrl;
            }

            public static void RemoveRedirectUrl(string orderNumber)
            {
                _redirectUrls.TryRemove(orderNumber, out _);
            }
        }
    }

}
