using Flurl.Http;

namespace MobileWorker
{
    public static class RequestBuilder
    {
        private static IFlurlClient _client;

        internal static IFlurlRequest Create()
        {

            if(_client is null)
            {
                _client = new FlurlClient
                {
                    BaseUrl = Constants.Endpoint
                };
                _client = _client.AllowAnyHttpStatus();
            }

            if(!string.IsNullOrWhiteSpace(MySettings.Token))
            {
                return _client.Request()
                              .WithOAuthBearerToken(MySettings.Token);
            }

            return _client.Request();
        }
    }
}