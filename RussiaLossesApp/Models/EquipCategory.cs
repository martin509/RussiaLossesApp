using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace RussiaLossesApp.Models
{
    public class EquipCategory
    {
        
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string categoryClass { get; set; } //e.g. 'tanks', 'artillery', etc. Don't see a reason to have categories spanning different classes of vehicle
        public List<EquipType> EquipTypes { get; set; } = new List<EquipType>();
        
    }
}
