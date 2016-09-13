/********************************************************
*                                                        *
*   Copyright (c) Microsoft. All rights reserved.        *
*                                                        *
*********************************************************/

namespace Microsoft.ProjectOxford.Text.SpellCheck
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// The SpellCheck Client.
    /// </summary>
    public class SpellCheckClient : ISpellCheckClient
    {
        /// <summary>
        /// The Service Host
        /// </summary>
        private const string serviceHost = "https://api.projectoxford.ai/text/v1.0/";

        /// <summary>
        /// The Subscription Key Name
        /// </summary>
        private const string subscriptionKeyName = "ocp-apim-subscription-key";

        /// <summary>
        /// The default Resolver
        /// </summary>
        private static CamelCasePropertyNamesContractResolver defaultResolver = new CamelCasePropertyNamesContractResolver();

        /// <summary>
        /// The settings
        /// </summary>
        private static JsonSerializerSettings settings = new JsonSerializerSettings()
        {
            DateFormatHandling = DateFormatHandling.IsoDateFormat,
            NullValueHandling = NullValueHandling.Ignore,
            ContractResolver = defaultResolver
        };

        /// <summary>
        /// The HTTP Client
        /// </summary>
        private HttpClient httpClient = new HttpClient();

        /// <summary>
        /// The subscription Key
        /// </summary>
        private string subscriptionKey;

        /// <summary>
        /// Initializes a new instance of the <see cref="SpellCheckClient"/> class.
        /// </summary>
        /// <param name="subscriptionKey">The subscription Key.</param>
        public SpellCheckClient(string subscriptionKey)
        {
            this.subscriptionKey = subscriptionKey;
            httpClient.BaseAddress = new Uri(serviceHost);
            httpClient.DefaultRequestHeaders.Add(subscriptionKeyName, subscriptionKey);
        }

        /// <summary>
        /// Get suggestions asynchronous.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="preContextText">The pre context text.</param>
        /// <param name="postContextText">The post context text.</param>
        /// <returns>
        /// SpellCheckResult object.
        /// </returns>
        /// <exception cref="ClientException"></exception>
        /// <exception cref="ErrorResponse"></exception>
        /// <exception cref="ClientError"></exception>
        public async Task<SpellCheckResult> GetSuggestionsAsync(string text, string preContextText = null, string postContextText = null, CheckMode checkMode = CheckMode.Proof)
        {
            var queryStrings = new List<KeyValuePair<string, string>>();
            queryStrings.Add(new KeyValuePair<string, string>("text", text));
            if (!String.IsNullOrWhiteSpace(preContextText))
            {
                queryStrings.Add(new KeyValuePair<string, string>("PreContextText", preContextText));
            }

            if (!String.IsNullOrWhiteSpace(postContextText))
            {
                queryStrings.Add(new KeyValuePair<string, string>("PostContextText", postContextText));
            }

            var content = new FormUrlEncodedContent(queryStrings);
            string mode = null;
            switch (checkMode)
            {
                case CheckMode.Proof:
                    mode = "proof";
                    break;
                case CheckMode.Spell:
                    mode = "spell";
                    break;
            }

            var response = httpClient.PostAsync("SpellCheck?mode=" + mode, content).Result;

            if (response.IsSuccessStatusCode)
            {
                string responseContent = null;
                if (response.Content != null)
                {
                    responseContent = await response.Content.ReadAsStringAsync();
                }

                if (!String.IsNullOrWhiteSpace(responseContent))
                {
                    return JsonConvert.DeserializeObject<SpellCheckResult>(responseContent, settings);
                }
            }
            else
            {
                if (response.Content != null && response.Content.Headers.ContentType.MediaType.Contains("application/json"))
                {
                    var errorObjectString = await response.Content.ReadAsStringAsync();
                    ErrorResponse errorCollection = JsonConvert.DeserializeObject<ErrorResponse>(errorObjectString);
                    if (errorCollection != null && errorCollection.Errors != null)
                    {
                        throw new ClientException(errorCollection, response.StatusCode);
                    }
                    else
                    {
                        ClientError error = JsonConvert.DeserializeObject<ClientError>(errorObjectString);

                        if (error != null)
                        {
                            throw new ClientException(
                                new ErrorResponse()
                                {
                                    Errors = new ClientError[1] 
                                    { 
                                        new ClientError()
                                        { 
                                            Code = error.Code, 
                                            Message = error.Message
                                        }
                                    }
                                },
                                response.StatusCode);
                        }
                    }
                }

                response.EnsureSuccessStatusCode();
            }

            return null;
        }

        /// <summary>
        /// Reconstruct the corrected text from the spell check response and the original text
        /// </summary>
        /// <param name="response">spell check response</param>
        /// <param name="text">original text</param>
        /// <returns>the corrected text</returns>
        public static string ConstructCorrectedTextFromResponse(SpellCheckResult response, string text)
        {
            if (response.SpellingErrors != null && response.SpellingErrors.Length > 0)
            {
                Array.Sort(response.SpellingErrors, (token1, token2) => ((int)token1.Offset - (int)token2.Offset));
                StringBuilder ret = new StringBuilder();
                int prevStart = 0;
                int offset = -1;
                for (var i = 0; i < response.SpellingErrors.Length; i++)
                {
                    SpellingError token = response.SpellingErrors[i];

                    if (offset == (int)token.Offset)
                    {
                        // duplicate flags. This shouldn't happen, but also doesn't hurt, so silently ignore it
                        continue;
                    }
                    else
                    {
                        offset = (int)token.Offset;
                    }
                    if (offset > prevStart)
                    {
                        ret.Append(text.Substring(prevStart, offset - prevStart));
                    }
                    string expectedToken = text.Substring(offset, token.Token.Length);
                    if (String.Compare(expectedToken, token.Token, StringComparison.Ordinal) != 0)
                    {
                        throw new Exception("Token mismatch. Text:" + text + "\tToken does not match, expected: " + expectedToken + " but was: " + token.Token);
                    }

                    if (token.Type == "RepeatedToken")
                    {
                        // If there is a space before the repeated word, remove it.
                        if (ret.Length > 0 && ret[ret.Length - 1] == ' ')
                        {
                            ret.Remove(ret.Length - 1, 1);
                        }
                    }
                    else
                    {
                        string topSuggestion = null;
                        if (token.Suggestions.Length == 0)
                        {
                            // No suggestion. Use original text
                            topSuggestion = token.Token;
                        }
                        else
                        {
                            topSuggestion = token.Suggestions[0].Token;
                        }
                        ret.Append(topSuggestion);
                    }

                    prevStart = offset + token.Token.Length;
                }
                if (prevStart < text.Length)
                {
                    ret.Append(text.Substring(prevStart));
                }

                return ret.ToString();
            }
            else
            {
                // No flagged token, return the original query.
                return text;
            }
        }
    }
}
