using System;
using System.Collections.Generic;
using System.Text;
using Lime.Protocol;
using Takenet.Iris.Messaging.Contents;

namespace Blip.HttpClient.Exceptions
{
    /// <summary>
    /// Exception thrown when the response status from BLiP is not Success
    /// </summary>
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

        /// <summary>
        /// Constructor to propagate a given <paramref name="message"/> in the exception
        /// </summary>
        /// <param name="message"></param>
        public BlipHttpClientException(string message)
        {
            _message = message;
        }

        /// <summary>
        /// Constructor to propagate a <paramref name="message"/> and a <paramref name="command"/>
        /// </summary>
        /// <param name="message"></param>
        /// <param name="command"></param>
        public BlipHttpClientException(string message, Command command)
        {
            _message = message;
            ResponseStatus = command.Status.ToString();
            Reason = command.Reason;
            Id = command.Id;
            Resource = command.Resource;
        }

        /// <summary>
        /// Base default ctor
        /// </summary>
        public BlipHttpClientException() : base()
        {
        }

        /// <summary>
        /// Base ctor propagating a given <paramref name="message"/> and a possible <paramref name="innerException"/>
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public BlipHttpClientException(string message, Exception innerException) : base(message, innerException)
        {
        }

        /// <summary>
        /// Exception message
        /// </summary>
        public override string Message
        {
            get { return _message; }
        }
    }
}
