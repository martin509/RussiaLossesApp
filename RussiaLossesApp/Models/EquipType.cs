using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace RussiaLossesApp.Models
{
    public class EquipType
    {
        public static string[] allClasses =
        {
            "Airplanes",
            "Anti-aircraft systems",
            "Command posts, communication",
            "Drones",
            "Engineering",
            "Helicopters",
            "Infantry fighting vehicles",
            "Infantry mobility vehicles",
            "Multiple rocket launchers",
            "Other",
            "Radars, jammers",
            "Self-propelled artillery",
            "Tanks",
            "Towed artillery",
            "Transport",
            "Vessels"
        };

        //entry for a given loss type (e.g. "BMP-1", etc)
        [Key]
        //[DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public string category { get; set; }
        public string name {  get; set; }

        [JsonIgnore]  
        public List<EquipCategory> equipCategories { get; set; } = new List<EquipCategory>();

        public string ToString()
        {
            return $"{name}, ({category})";
        }
    }


}
