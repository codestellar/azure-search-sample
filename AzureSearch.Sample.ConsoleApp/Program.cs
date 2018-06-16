using System;
using System.Collections.Generic;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
namespace AzureSearch.Sample.ConsoleApp
{
    public class Program
    {
        static void Main(string[] args)
        {
            var azureSearchName = "[Your Search Service Name]";

            // The key of Search Service
            var azureSearchKey = "[Your Search Service Admin Key]";

            // Index name variable
            string indexName = "products";

            // Create a service client connection
            ISearchServiceClient azureSearchService = new SearchServiceClient(azureSearchName, new SearchCredentials(azureSearchKey));

            var indexClient = CreateSearchIndex(indexName, azureSearchService);

            var wantToSearch = "y";
            // Search by word            
            do
            {
                Console.WriteLine("What do you want to search?");
                var searchText = Console.ReadLine();

                Search(indexClient, searchText: $"/.*{ searchText }.*/");
                System.Threading.Thread.Sleep(1000);
                Console.WriteLine("Search Again (y/n)? ");
                wantToSearch = Console.ReadLine();
            } while (wantToSearch == "y");

        }

        private static ISearchIndexClient CreateSearchIndex(string indexName, ISearchServiceClient azureSearchService)
        {
            // Get the Azure Search Index
            ISearchIndexClient indexClient = azureSearchService.Indexes.GetClient(indexName);

            // If the Azure Search Index exists delete it.
            Console.WriteLine("Deleting index...\n");
            if (azureSearchService.Indexes.Exists(indexName))
            {
                azureSearchService.Indexes.Delete(indexName);
            }

            // Create an Index Model
            Console.WriteLine("Creating index Model...\n");
            Index indexModel = new Index()
            {
                Name = indexName,
                Fields = new[]
                {
                    new Field("Id", DataType.String) { IsKey = true, IsRetrievable = true, IsFacetable = false },
                    new Field("ProductName", DataType.String) {IsRetrievable = true, IsSearchable = true, IsFacetable = false },
                    new Field("ProductCode", DataType.String) {IsRetrievable = true, IsSearchable = true, IsFacetable = false },
                    new Field("ProductDescription", DataType.String) {IsRetrievable = true, IsSearchable = true, IsFacetable = false },
                    new Field("Price", DataType.Int32) { IsFacetable = true, IsFilterable= true, IsRetrievable = true },
                    new Field("Quantity", DataType.Int32) { IsFacetable = true, IsFilterable= true, IsRetrievable = true },
                    new Field("Size", DataType.Collection(DataType.String)) {IsSearchable = true, IsRetrievable = true, IsFilterable = true, IsFacetable = false },
                    new Field("Colors", DataType.Collection(DataType.String)) {IsSearchable = true, IsRetrievable = true, IsFilterable = true, IsFacetable = false },

                }
            };

            // Create the Index in AzureSearch
            Console.WriteLine("Creating index...\n");
            var resultIndex = azureSearchService.Indexes.Create(indexModel);

            // Add documents to the Index
            Console.WriteLine("Add documents...\n");
            var productsList = new Product().GetProducts();
            indexClient.Documents.Index(IndexBatch.MergeOrUpload<Product>(productsList));
            System.Threading.Thread.Sleep(1000);

            return indexClient;
        }

        private static void Search(ISearchIndexClient indexClient, string searchText, string filter = null, List<string> order = null, List<string> facets = null)
        {
            // Execute search based on search text and optional filter
            var sp = new SearchParameters
            {
                QueryType = QueryType.Full
            };

            // Add Filter
            if (!String.IsNullOrEmpty(filter))
            {
                sp.Filter = filter;
            }

            // Order
            if (order != null && order.Count > 0)
            {
                sp.OrderBy = order;
            }

            // facets
            if (facets != null && facets.Count > 0)
            {
                sp.Facets = facets;
            }

            // Search
            DocumentSearchResult<Product> response = indexClient.Documents.Search<Product>(searchText, sp);
            Console.WriteLine($"\n{response.Results.Count} Results found:\n");
            foreach (SearchResult<Product> result in response.Results)
            {
                Console.WriteLine("############################");
                Console.WriteLine($"Product Name - {result.Document.ProductName}");
                Console.WriteLine($"Product Code - {result.Document.ProductCode}");
                Console.WriteLine($"Price - {result.Document.Price}");
                Console.WriteLine($"Colors - {string.Join(", ", result.Document.Colors.ToArray())}");
                Console.WriteLine($"Size - {string.Join(", ", result.Document.Size.ToArray())}");
                Console.WriteLine(result.Document.ProductDescription + " - Score: " + result.Score);
                Console.WriteLine("############################\n");
            }

            if (response.Facets != null)
            {
                foreach (var facet in response.Facets)
                {
                    Console.WriteLine("\n Facet Name: " + facet.Key);
                    foreach (var value in facet.Value)
                    {
                        Console.WriteLine("Value :" + value.Value + " - Count: " + value.Count);
                    }
                }
            }
        }

    }
}

