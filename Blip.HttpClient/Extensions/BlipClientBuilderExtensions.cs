using Lime.Protocol;
using System;
using System.Collections.Generic;
using System.Text;
using Take.Blip.Client;

namespace Blip.HttpClient.Extensions
{
    public static class BlipClientBuilderExtensions
    {
        private const string KEY_PREFIX = "Key ";

        /// <summary>
        /// Configures the authorization key inside the builder
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="authorizationKey"></param>
        public static BlipClientBuilder UsingAuthorizationKey(this BlipClientBuilder builder, string authorizationKey)
        {
            authorizationKey = authorizationKey.Replace(KEY_PREFIX, "");
            var decodedTokens = authorizationKey.FromBase64().Split(':');
            var identifier = decodedTokens[0];
            var accessKey = decodedTokens[1].ToBase64();

            return builder.UsingAccessKey(identifier, accessKey);
        }
    }
}
