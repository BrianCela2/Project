using System.ComponentModel.DataAnnotations;

namespace Project.Models
{
    public class Furnizues
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Emri { get; set; }
        public string?  Shteti { get; set; }
        public List<Produkt>? Produkte { get; set; } 
        
    }
}
