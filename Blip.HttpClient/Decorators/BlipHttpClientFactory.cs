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

namespace Blip.HttpClient.Decorators
{
    public class BlipHttpClientFactory
    {
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
