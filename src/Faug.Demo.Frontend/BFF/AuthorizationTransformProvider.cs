using System.Net.Http.Headers;
using System.Threading.Tasks;
using Yarp.ReverseProxy.Transforms.Builder;
using Yarp.ReverseProxy.Transforms;
using Microsoft.AspNetCore.Authentication;

namespace Faug.Demo.Frontend.BFF
{

    /// <summary>
    /// Transform requests. Changes authentication against Pluto.
    /// </summary>
    internal class AuthorizationTransformProvider : ITransformProvider
    {

        public void Apply(TransformBuilderContext transformBuildContext)
        {
            transformBuildContext?.AddRequestTransform(async transformContext =>
            {
                await TransformAuthorizationHeader(transformContext);
            });
        }

        public void ValidateCluster(TransformClusterValidationContext context)
        {

        }

        public void ValidateRoute(TransformRouteValidationContext context)
        {

        }

        /// <summary>
        /// Adds Bearer token to request from cookie
        /// </summary>
        /// <param name="transformContext"></param>
        /// <returns></returns>
        private async Task TransformAuthorizationHeader(RequestTransformContext transformContext)
        {

            var accessToken = await transformContext.HttpContext.GetTokenAsync("access_token");

            if (accessToken != null)
            {
                transformContext.ProxyRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            }
        }
    }

}
