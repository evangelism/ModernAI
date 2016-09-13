/********************************************************
*                                                        *
*   Copyright (c) Microsoft. All rights reserved.        *
*                                                        *
*********************************************************/

namespace Microsoft.ProjectOxford.Text.SpellCheck
{
    using System.Runtime.Serialization;
    
    /// <summary>
    /// Spelling Error.
    /// </summary>
    [DataContract]
    public class SpellingError
    {
        /// <summary>
        /// Gets or sets the offset.
        /// </summary>
        /// <value>
        /// The offset.
        /// </value>
        [DataMember(Name = "offset")]
        public int Offset { get; set; }

        /// <summary>
        /// Gets or sets the token.
        /// </summary>
        /// <value>
        /// The token.
        /// </value>
        [DataMember(Name = "token")]
        public string Token { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        [DataMember(Name = "type")]
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the suggestions.
        /// </summary>
        /// <value>
        /// The suggestions.
        /// </value>
        [DataMember(Name = "suggestions")]
        public Suggestion[] Suggestions { get; set; }
    }   
}
