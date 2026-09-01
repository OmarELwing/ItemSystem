using SimpleProject.Data.Models;
using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    namespace SimpleProject.Models
{
        public class Item
        {
            [Key]
        public int Id { get; set; }

        public string? Name { get; set; }

        public string? Description { get; set; }

        public double Price { get; set; }

        public string? ImageUrl { get; set; }

        public int CategoryId { get; set; }

        public DateTime CreatedAt { get; set; }

        
        public Category? Category { get; set; }
    }
    }
