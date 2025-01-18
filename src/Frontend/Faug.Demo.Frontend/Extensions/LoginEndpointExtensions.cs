using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using System;


namespace Faug.Demo.Frontend.Extensions
{
    public static partial class EndpointRouteBuilderExtensions
    {
        /// <summary>
        /// This extension contains all login and logout related endpoints
        /// </summary>
        /// <param name="endpoints"></param>
        /// <param name="configuration"></param>
        public static void MapLoginEndpoints(this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapGet("/login", async (HttpContext httpContext, string provider, string method, string uiLocales, string redirectUri) =>
            {
                var properties = new AuthenticationProperties
                {
                    IsPersistent = false,
                    RedirectUri = "/", // Redirect URI determins the URL where user is returned after login
                };

                //  if (string.IsNullOrEmpty(provider) || string.IsNullOrEmpty(method))
                //  {
                // logger.Error("Login provider or method is not valid", provider, method);

                //      return;
                // }

                properties.Items.Add("provider", provider);
                properties.Items.Add("method", method);

                if (!string.IsNullOrEmpty(uiLocales))
                {
                    properties.Items.Add("ui_locales", uiLocales);
                }

                // properties.RedirectUri = BuildRedirectUri(httpContext, redirectUri);

                properties.RedirectUri = "https://localhost:3443";


                //This needs to be set to true, due to reverse proxies causing application to not see this as https even though it is. 
                httpContext.Request.IsHttps = true;
                await httpContext.ChallengeAsync(OpenIdConnectDefaults.AuthenticationScheme, properties);
            });

        }

        private static string ValidateRedirectUri(string redirectUri)
        {
            var path = "/";
            if (Uri.TryCreate(redirectUri, UriKind.RelativeOrAbsolute, out Uri uri))
            {
                if (uri.IsAbsoluteUri)
                {
                    path = uri.PathAndQuery;
                }
                else
                {
                    path = redirectUri;
                }
            }
            else
            {
                path = "/";
            }
            return path;
        }

        public static string BuildRedirectUri(HttpContext context, string redirectPath)
        {
            if (string.IsNullOrEmpty(redirectPath) || redirectPath.Equals("undefined", StringComparison.InvariantCultureIgnoreCase))
            {
                redirectPath = "/";
            }

            var path = ValidateRedirectUri(redirectPath);

            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
            {
                return $"http://localhost:8000{path}";
            }
            return path;
        }
    }

}