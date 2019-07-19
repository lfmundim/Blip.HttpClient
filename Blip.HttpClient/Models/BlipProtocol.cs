using System;
using System.Collections.Generic;
using System.Text;

namespace Blip.HttpClient.Models
{
    /// <summary>
    /// Supported protocols for BLiP's <c>ISender</c>
    /// </summary>
    public enum BlipProtocol
    {
        /// <summary>
        /// More unstable, cheaper to open and close
        /// </summary>
        Http,
        /// <summary>
        /// Highly stable, faster, more expensive to open and close
        /// </summary>
        Tcp
    }
}
