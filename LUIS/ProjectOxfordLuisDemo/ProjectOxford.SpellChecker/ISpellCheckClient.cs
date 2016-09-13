/********************************************************
*                                                        *
*   Copyright (c) Microsoft. All rights reserved.        *
*                                                        *
*********************************************************/

namespace Microsoft.ProjectOxford.Text.SpellCheck
{
    using System.Threading.Tasks;

    /// <summary>
    /// SpellCheck client interface.
    /// </summary>
    public interface ISpellCheckClient
    {
        /// <summary>
        /// Get suggestions asynchronous.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="checkMode">The check mode.</param>
        /// <param name="preContextText">The pre context text.</param>
        /// <param name="postContextText">The post context text.</param>
        /// <returns>
        /// SpellCheckResult object.
        /// </returns>
        Task<SpellCheckResult> GetSuggestionsAsync(string text,string preContextText = null, string postContextText = null, CheckMode checkMode = CheckMode.Proof);
    }
}
