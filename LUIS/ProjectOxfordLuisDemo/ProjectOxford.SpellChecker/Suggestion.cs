/********************************************************
*                                                        *
*   Copyright (c) Microsoft. All rights reserved.        *
*                                                        *
*********************************************************/

namespace Microsoft.ProjectOxford.Text.SpellCheck
{
    using System.Runtime.Serialization;

    /// <summary>
    /// Suggestion class.
    /// </summary>
    [DataContract]
    public class Suggestion
    {
        /// <summary>
        /// Gets or sets the Token.
        /// </summary>
        /// <value>
        /// The Token.
        /// </value>
        [DataMember(Name = "token")]
        public string Token { get; set; }
    }
}
