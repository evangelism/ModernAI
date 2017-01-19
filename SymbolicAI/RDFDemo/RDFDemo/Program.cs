using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDS.RDF;
using VDS.RDF.Parsing;
using VDS.RDF.Query;
using VDS.RDF.Query.Inference;

namespace RDFDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            IGraph g = new Graph();
            UriLoader.Load(g, new Uri("http://dbpedia.org/resource/Russia"));
            
            Console.WriteLine("=== TRIPLES ===");
            foreach (var x in g.Triples)
            {
                if (x.Predicate.ToString().Contains("population"))
                {
                    Console.WriteLine($"O={x.Object}, P={x.Predicate}, S={x.Subject}");
                }
            }

            Console.WriteLine("=== QUERY ===");

            SparqlRemoteEndpoint endpoint = new SparqlRemoteEndpoint(new Uri("http://dbpedia.org/sparql"), "http://dbpedia.org");
            SparqlResultSet results = endpoint.QueryWithResultSet(@"
PREFIX owl: <http://www.w3.org/2002/07/owl#>
PREFIX xsd: <http://www.w3.org/2001/XMLSchema#>
PREFIX rdfs: <http://www.w3.org/2000/01/rdf-schema#>
PREFIX rdf: <http://www.w3.org/1999/02/22-rdf-syntax-ns#>
PREFIX foaf: <http://xmlns.com/foaf/0.1/>
PREFIX dc: <http://purl.org/dc/elements/1.1/>
PREFIX : <http://dbpedia.org/resource/>
PREFIX dbpedia2: <http://dbpedia.org/property/>
PREFIX dbpedia: <http://dbpedia.org/>
PREFIX skos: <http://www.w3.org/2004/02/skos/core#>
SELECT ?birth STR(?name) WHERE {
      ?person a dbo:MusicalArtist .
      ?person dbo:birthPlace :Moscow .
      ?person dbo:birthDate ?birth .
      ?person foaf:name ?name .
} ORDER BY ?name
");
            foreach (SparqlResult result in results)
            {
                Console.WriteLine(result.ToString());
            }


            Console.ReadKey();
        }
    }
}
