using System;
using System.Collections.Generic;
using System.Text;
using Lime.Protocol;
using Takenet.Iris.Messaging.Contents;

namespace Blip.HttpClient.Exceptions
{
    public class BlipHttpClientException : Exception
    {
        /// <summary>
        /// Exception message
        /// </summary>
        public string _message { get; set; }
        /// <summary>
        /// Response status returned by the BLiP API
        /// </summary>
        public string ResponseStatus { get; set; }
        /// <summary>
        /// Reason for failure returned by the BLiP API
        /// </summary>
        public Reason Reason { get; set; }
        /// <summary>
        /// Error ID returned by the BLiP API
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// Any extra resources returned by the BLiP API
        /// </summary>
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
