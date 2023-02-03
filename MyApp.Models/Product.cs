using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApp.Models
{
    public class Product
    {
        public int Id { get; set; }
        [Required]
        public int Name { get; set; }
        [Required]
        public int Description { get; set; }
        [Required]
        public double   Price { get; set; }
        public int ImageUrl { get; set; }
        public int CategoryId { get; set; }

        public Category Category { get; set; }
    }
}
