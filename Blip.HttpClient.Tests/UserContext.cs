using Lime.Protocol;
using System.Runtime.Serialization;

namespace Blip.HttpClient.Tests
{
    public class UserContext : Document
    {
        public static MediaType MEDIA_TYPE = MediaType.Parse("application/vnd.httpclient.test+json");
        public UserContext() : base(MEDIA_TYPE)
        {
            IsNewUser = true;
        }

        [DataMember(Name = "isnewuser")]
        public bool IsNewUser { get; set; }
    }
}
