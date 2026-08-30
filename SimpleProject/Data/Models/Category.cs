using SimpleProject.Models;
using System.ComponentModel.DataAnnotations;

namespace SimpleProject.Data.Models
{
    public class Category
    {
            [Key]
            public int Id { get; set; }

            public string? Name { get; set; }
            
            public ICollection<Item> Items { get; set; } = new List<Item>();
        
    }
}
