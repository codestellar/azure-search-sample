using System;
using System.Collections.Generic;

namespace AzureSearch.Sample.ConsoleApp
{
    public class Product
    {
        public string Id { get; set; }
        public string ProductName { get; set; }

        public string ProductCode { get; set; }

        public string ProductDescription { get; set; }

        public int Price { get; set; }

        public int Quantity { get; set; }

        public List<string> Size { get; set; }

        public List<string> Colors { get; set; }

        // Generate Mock Products
        public List<Product> GetProducts()
        {
            var products = new List<Product>();
            Random randomNumber = new Random();

            var sizes = new List<string> { "XS", "S", "M", "L", "XL", "XXL" };
            var colors = new List<string> { "Black", "White", "Red", "Orange", "Purple", "Yellow" };

            for (var i = 1; i <= 1000; i++)
            {
                products.Add(new Product
                {
                    Id = i.ToString(),
                    ProductName = $"Product{i}",
                    ProductCode = $"PRD{i}",
                    ProductDescription = $"This is description for Product {i}",

                    Price = i * randomNumber.Next(100, 300),
                    Quantity = i * randomNumber.Next(1, 100),
                    Size = new List<string> { sizes[randomNumber.Next(0, 2)], sizes[randomNumber.Next(3, 5)] },
                    Colors = new List<string> { colors[randomNumber.Next(0, 2)], colors[randomNumber.Next(3, 5)] }
                });
            }
            return products;
        }

    }
}
