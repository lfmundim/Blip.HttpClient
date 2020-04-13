using Blip.HttpClient.Extensions;
using Blip.HttpClient.Models;
using Blip.HttpClient.Services;
using Lime.Messaging.Resources;
using Lime.Protocol;
using Lime.Protocol.Serialization;
using Lime.Protocol.Serialization.Newtonsoft;
using Microsoft.Extensions.DependencyInjection;
using RestEase;
using System.Collections.Generic;
using System.Net.Http.Headers;
using Take.Blip.Client;
using Take.Blip.Client.Extensions;

namespace Blip.HttpClient.Factories
{
    /// <summary>
    /// Factory that allows the creation of a BlipClient that uses Http or Tcp
    /// </summary>
    public class BlipClientFactory
    {
        private const string KEY_PREFIX = "Key";
        private const string DEFAULT_BASE_DOMAIN = "msging.net";
        private const string HTTP_DOMAIN_PREFIX = "http.";

        private readonly string _domain;
        private string MSGING_BASE_URL(string domain = DEFAULT_BASE_DOMAIN) => $"https://{domain}/";

        public BlipClientFactory(string domain = DEFAULT_BASE_DOMAIN)
        {
            _domain = domain.IsNullOrWhiteSpace() ? DEFAULT_BASE_DOMAIN : domain;
        }
      
        /// <summary>
        /// Creates or updates a Service Collection to include BLiP's extensions and any custom Documents, including an <c>ISender</c>
        /// </summary>
        /// <param name="authKey"></param>
        /// <param name="protocol"></param>
        /// <param name="documents"></param>
        /// <param name="serviceCollection"></param>
        public IServiceCollection BuildServiceCollection(string authKey, BlipProtocol protocol, List<Document> documents = null, IServiceCollection serviceCollection = null)
        {
            serviceCollection = serviceCollection ?? new ServiceCollection();

            var documentResolver = new DocumentTypeResolver();
            documentResolver.WithBlipDocuments();

            documents?.ForEach(d => documentResolver.RegisterDocument(d.GetType()));

            var envelopeSerializer = new EnvelopeSerializer(documentResolver);
            serviceCollection.AddSingleton<IEnvelopeSerializer>(envelopeSerializer);

            var sender = BuildBlipClient(authKey, protocol);
            serviceCollection.AddSingleton(sender);

            serviceCollection.RegisterBlipExtensions();

            return serviceCollection;
        }

        /// <summary>
        /// Builds an ISender using the given auth key for the given protocol
        /// </summary>
        /// <param name="authKey"></param>
        /// <param name="protocol"></param>
        /// <param name="documents"></param>
        public ISender BuildBlipClient(string authKey, BlipProtocol protocol = BlipProtocol.Tcp, List<Document> documents = null)
        {
            authKey = FixAuthKey(authKey);

            var envelopeSerializer = CreateEnvelopeSerializer(documents);

            switch (protocol)
            {
                case BlipProtocol.Http:
                    return BuildHttpClient(authKey, envelopeSerializer);
                default:
                    return BuildTcpClient(authKey, envelopeSerializer);
            }
        }

        private ISender BuildTcpClient(string authKey, EnvelopeSerializer envelopeSerializer)
        {
            return new BlipClientBuilder(new TcpTransportFactory(envelopeSerializer))
                                        .UsingAuthorizationKey(authKey)
                                        .UsingRoutingRule(RoutingRule.Instance)
                                        .WithChannelCount(2)
                                        .UsingHostName(_domain)
                                        .Build();
        }

        private ISender BuildHttpClient(string authKey, EnvelopeSerializer envelopeSerializer)
        {
            var domain = _domain.Equals(DEFAULT_BASE_DOMAIN)
                       ? HTTP_DOMAIN_PREFIX + _domain
                       : _domain;

            var client = new RestClient(MSGING_BASE_URL(domain)) {
                JsonSerializerSettings = envelopeSerializer.Settings
            }.For<IBlipHttpClient>();

            client.Authorization = new AuthenticationHeaderValue(KEY_PREFIX, authKey);
            return new BlipHttpClient(client);
        }

        private string FixAuthKey(string authKey)
        {
            if (authKey.StartsWith(KEY_PREFIX))
            {
                authKey = authKey.Replace(KEY_PREFIX, string.Empty).Trim();
            }

            return authKey;
        }

        private EnvelopeSerializer CreateEnvelopeSerializer(List<Document> documents = null)
        {
            var documentResolver = new DocumentTypeResolver();
            documentResolver.WithBlipDocuments();

            documents?.ForEach(d => documentResolver.RegisterDocument(d.GetType()));

            var envelopeSerializer = new EnvelopeSerializer(documentResolver);

            return envelopeSerializer;
        }
    }
}