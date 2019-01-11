using System;
using System.Collections.Generic;
using System.Text;
using Lime.Protocol;
using Takenet.Iris.Messaging.Contents;

namespace Blip.HttpClient.Exceptions
{
    public class BlipHttpClientException : Exception
    {
        public string _message { get; set; }
        public string ResponseStatus { get; set; }
        public Reason Reason { get; set; }
        public string Id { get; set; }
        public Document Resource { get; set; }

        public BlipHttpClientException(string message)
        {
            _message = message;
        }

        public BlipHttpClientException(string message, Command command)
        {
            _message = message;
            ResponseStatus = command.Status.ToString();
            Reason = command.Reason;
            Id = command.Id;
            Resource = command.Resource;
        }

        public override string Message
        {
            get { return _message; }
        }
    }
}
