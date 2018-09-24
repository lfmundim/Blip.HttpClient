using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;
using Blip.HttpClient.Services;
using Lime.Protocol.Serialization;
using Lime.Protocol.Serialization.Newtonsoft;
using RestEase;
using Take.Blip.Client;
using Take.Blip.Client.Extensions;

namespace Blip.HttpClient.Decorators
{
    public class BlipHttpClientFactory
    {
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
