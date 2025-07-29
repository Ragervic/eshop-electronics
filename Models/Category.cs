using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace TestP.Models
{
    public class Category
    {

        public int Id { get; set; }
        public string? Name { get; set; }
        public ICollection<Product>? Products { get; set; }

        [NotMapped]
        public int ProductCount { get; set; }
    }
}