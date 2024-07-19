using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace RussiaLossesApp.Models
{
    public class EquipCategory
    {
        
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public List<EquipType> EquipTypes { get; set; } = new List<EquipType>();
        
    }
}
