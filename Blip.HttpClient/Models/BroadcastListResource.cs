using System;
using System.Collections.Generic;
using System.Text;
using Lime.Protocol;
using Newtonsoft.Json;

namespace Blip.HttpClient.Models
{
    /// <summary>
    /// Resource to be sent along with broadcast list creation 
    /// </summary>
    public class BroadcastListResource : Document
    {
        /// <summary>
        /// Mime Media type
        /// </summary>
        public static MediaType MEDIA_TYPE = MediaType.Parse("application/vnd.iris.distribution-list+json");

        /// <summary>
        /// Creates the resource using a given identity string
        /// </summary>
        /// <param name="identity"></param>
        public BroadcastListResource(string identity) : base(MEDIA_TYPE)
        {
            Identity = identity;
        }

        /// <summary>
        /// Identity.ToString() of the list to be created
        /// </summary>
        [JsonProperty("identity")]
        public string Identity { get; set; }
    }
}
