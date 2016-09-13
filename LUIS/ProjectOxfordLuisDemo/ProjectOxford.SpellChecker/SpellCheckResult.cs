/********************************************************
*                                                        *
*   Copyright (c) Microsoft. All rights reserved.        *
*                                                        *
*********************************************************/

namespace Microsoft.ProjectOxford.Text.SpellCheck
{
    using System.Runtime.Serialization;
    
    /// <summary>
    /// SpellCheckResult.
    /// </summary>
    [DataContract]
    public class SpellCheckResult
    {
        /// <summary>
        /// Gets or sets the spelling errors.
        /// </summary>
        /// <value>
        /// The spelling errors.
        /// </value>
        [DataMember(Name = "spellingErrors")]
        public SpellingError[] SpellingErrors { get; set; }
    }
}
