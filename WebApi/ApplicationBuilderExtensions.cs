using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace DevRating.WebApi
{
    public static class ApplicationBuilderExtensions
    {
        public static IServiceCollection AddFirebaseAuthentication(this IServiceCollection services, string issuer, string audience)
        {
            var configurationManager = new ConfigurationManager<OpenIdConnectConfiguration>($"{issuer}/.well-known/openid-configuration", new OpenIdConnectConfigurationRetriever());
            services.AddAuthentication(o =>
            {
                o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                o.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(o =>
            {
                o.IncludeErrorDetails = true;
                o.RefreshOnIssuerKeyNotFound = true;
                o.MetadataAddress = $"{issuer}/.well-known/openid-configuration";
                o.ConfigurationManager = configurationManager;
                o.Audience = audience;
            });
            return services;
        }

        public static IServiceCollection AddFirebaseAuthentication(this IServiceCollection services, string firebaseProject)
        {
            return services.AddFirebaseAuthentication("https://securetoken.google.com/" + firebaseProject, firebaseProject);
        }
    }
}
