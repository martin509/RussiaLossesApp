using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RussiaLossesApp.Models
{
    public class Coordinate
    {
        double lat, lng;
        public Coordinate(string coord)
        {
            string[] latlng = coord.Split(',');
            lat = double.Parse(latlng[0]);
            lng = double.Parse(latlng[1]);
        }
    }
    public class JsonLoss
    {
        public int Id { get; set; }
        public DateTime date { get; set; }
        public string status { get; set; }
        public string? lost_by { get; set; }
        public string? nearest_location { get; set; }
        public string? geo { get; set; }
        public string? unit { get; set; }
        public string? tags { get; set; }
        public string type { get; set; }
        public string model { get; set; }

    }

        public class LossObject
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        //public int EquipTypeId {  get; set; }
        
        [DataType(DataType.Date)]
        public DateTime date { get; set; }
        public string status { get; set; }
        public string? lost_by { get; set; }
        public string? nearest_location { get; set; }
        public string? geo { get; set; }
        public string? unit { get; set; }
        public string? tags { get; set; }

        
        //public int EquipTypeId { get; set; }
        //[ForeignKey("EquipTypeId")]
        public virtual EquipType EquipType { get; set; }

        public void copyAttributes(LossObject obj)
        {
            EquipType = obj.EquipType;
            date = obj.date;
            status = obj.status;
            lost_by = obj.lost_by;
            nearest_location = obj.nearest_location;
            geo = obj.geo;
            unit = obj.unit;
            tags = obj.tags;
            

        }
        public void copyAttributes(JsonLoss obj)
        {
            Id = obj.Id;
            date = obj.date;
            status = obj.status;
            lost_by = obj.lost_by;
            nearest_location = obj.nearest_location;
            geo = obj.geo;
            unit = obj.unit;
            tags = obj.tags;
            
        }

        public string getType()
        {
            return EquipType.category;
        }
        public string getModel()
        {
            return EquipType.name;
        }
        public override string ToString()
        {
            string s = $"ID: {Id}\n";
            s += $"Model: {EquipType.name}\n";
            s += $"Type: {EquipType.category}\n";
            s += $"Date: {date}\n";
            if(nearest_location != null) {
                s += $"Nearest location: {nearest_location}\n";
            }
            if (geo != null)
            {
                s += $"Geo: {geo}\n";
            }
            if (unit != null)
            {
                s += $"Unit: {unit}\n";
            }
            if(tags != null)
            {
                s += $"Tags: {tags}";
            }
            return s;
        }

        public double getLong()
        {
            string[] latlng = geo.Split(',');
            return double.Parse(latlng[0]);
        }
        public double getLat()
        {
            string[] latlng = geo.Split(',');
            return double.Parse(latlng[1]);
        }

       

    }

}
