using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace RussiaLossesApp.Models
{
    public class EquipType
    {
        //entry for a given loss type (e.g. "BMP-1", etc)
        [Key]
        //[DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public string category { get; set; }
        public string name {  get; set; }  

        public List<EquipCategory> equipCategories { get; set; } = new List<EquipCategory>();

        public string ToString()
        {
            return $"{name}, ({category})";
        }
    }
}
