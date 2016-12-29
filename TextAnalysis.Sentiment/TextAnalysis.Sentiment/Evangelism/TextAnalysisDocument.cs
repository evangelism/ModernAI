using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Evangelism
{

    public class TextAnalysisDocument
    {
        public TextAnalysisDocument() { }
        public TextAnalysisDocument(string id, string lang, string text)
        {
            this.id = id;
            this.language = lang;
            this.text = text;
        }
        public string language { get; set; }
        public string id { get; set; }
        public string text { get; set; }
        public double score { get; set; }
        public string[] keyPhrases { get; set; }
        public string errMessage { get; set; }
    }

    public class TextAnalysisError
    {
        public string id { get; set; }
        public string message { get; set; }
    }

    public class TextAnalysisDocumentStore
    {
        public List<TextAnalysisDocument> documents { get; set; }
        public TextAnalysisError[] errors { get; set; }
        public TextAnalysisDocumentStore()
        {
            documents = new List<Evangelism.TextAnalysisDocument>();
        }

        public TextAnalysisDocumentStore(TextAnalysisDocument doc)
        {
            documents = new List<Evangelism.TextAnalysisDocument>();
            documents.Add(doc);
        }

    }
}
