using Blip.HttpClient.Extensions;
using Blip.HttpClient.Services;
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
    public class BlipHttpClientFactory
    {
        /// <summary>
        /// Creates or updates a Service Collection to include BLiP's extensions and any custom Documents
        /// </summary>
        /// <param name="authKey"></param>
        /// <param name="documents"></param>
        /// <param name="serviceCollection"></param>
        /// <returns></returns>
        public IServiceCollection BuildServiceCollection(string authKey, List<Document> documents = null, IServiceCollection serviceCollection = null)
        {
            if (serviceCollection == null)
            {
                serviceCollection = new ServiceCollection();
            }

            var documentResolver = new DocumentTypeResolver();
            documentResolver.WithBlipDocuments();

            if (documents != null)
            {
                foreach (Document d in documents)
                {
                    documentResolver.RegisterDocument(d.GetType());
                }
            }

            var envelopeSerializer = new EnvelopeSerializer(documentResolver);

            var client = new RestClient("https://msging.net/")
            {
                JsonSerializerSettings = envelopeSerializer.Settings
            }.For<IBlipHttpClient>();

            client.Authorization = new AuthenticationHeaderValue("Key", authKey);
            var sender = new BlipHttpClient(client);
            serviceCollection.AddSingleton<ISender>(sender);

            serviceCollection.RegisterBlipExtensions();

            return serviceCollection;
        }

        /// <summary>
        /// Builds an ISender using the given auth key and custom documents previously set
        /// </summary>
        /// <param name="authKey"></param>
        /// <param name="documentTypeResolver"></param>
        /// <returns></returns>
        public ISender BuildBlipHttpClient(string authKey, IDocumentTypeResolver documentTypeResolver)
        {
            var envelopeSerializer = new EnvelopeSerializer(documentTypeResolver);

            var client = new RestClient("https://msging.net/")
            {
                JsonSerializerSettings = envelopeSerializer.Settings
            }.For<IBlipHttpClient>();

            client.Authorization = new AuthenticationHeaderValue("Key", authKey);
            return new BlipHttpClient(client);
        }

        /// <summary>
        /// Builds an ISender using the given auth key
        /// </summary>
        /// <param name="authKey"></param>
        /// <returns></returns>
        public ISender BuildBlipHttpClient(string authKey)
        {
            var documentResolver = new DocumentTypeResolver();
            documentResolver.WithBlipDocuments();

            var envelopeSerializer = new EnvelopeSerializer(documentResolver);

            var client = new RestClient("https://msging.net/")
            {
                JsonSerializerSettings = envelopeSerializer.Settings
            }.For<IBlipHttpClient>();

            client.Authorization = new AuthenticationHeaderValue("Key", authKey);
            return new BlipHttpClient(client);
        }
    }
}
