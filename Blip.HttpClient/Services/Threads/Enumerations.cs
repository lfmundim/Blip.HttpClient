using System;
using System.Collections.Generic;
using System.Text;

namespace Blip.HttpClient.Services.Threads
{
    /// <summary>
    /// Enumerations used on Threads service
    /// </summary>
    public static class Enumerations
    {
        /// <summary>
        /// Order of the chat messages
        /// </summary>
        public enum ChatOrder
        {
            /// <summary>
            /// Oldest to newest
            /// </summary>
            Asc,
            /// <summary>
            /// Newest to oldest
            /// </summary>
            Desc
        }
    }
}
