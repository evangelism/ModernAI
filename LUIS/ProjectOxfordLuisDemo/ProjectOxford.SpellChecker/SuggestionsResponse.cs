/********************************************************
*                                                        *
*   Copyright (c) Microsoft. All rights reserved.        *
*                                                        *
*********************************************************/

namespace Microsoft.ProjectOxford.Text.SpellCheck
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using System.Runtime.Serialization;


    /// <summary>
    /// GetSuggestionsResponse.
    /// </summary>
    [DataContract]
    public class SuggestionsResponse
    {
        /// <summary>
        /// Gets or sets the flagged tokens.
        /// </summary>
        /// <value>
        /// The flagged tokens.
        /// </value>
        [DataMember(Name = "flaggedTokens")]
        public FlaggedToken[] FlaggedTokens { get; set; }
    }

    /// <summary>
    /// Flagged Token.
    /// </summary>
    [DataContract]
    public class FlaggedToken
    {
        /// <summary>
        /// Gets or sets the offset.
        /// </summary>
        /// <value>
        /// The offset.
        /// </value>
        [DataMember(Name = "offset")]
        public double Offset { get; set; }

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
        public SearchSuggestion[] Suggestions { get; set; }
    }

    /// <summary>
    /// Suggestion class.
    /// </summary>
    [DataContract]
    public class SearchSuggestion
    {
        /// <summary>
        /// Gets or sets the suggestion.
        /// </summary>
        /// <value>
        /// The suggestion.
        /// </value>
        [DataMember(Name = "suggestion")]
        public string Suggestion { get; set; }
    }
}
