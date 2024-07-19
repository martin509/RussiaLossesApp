namespace RussiaLossesApp.Models
{
    public class LossListObject
    {
        public string? model { get; set; }
        public string? type { get; set; }
        public int nDamaged { get; set; } = 0;
        public int nDestroyed { get; set; } = 0;
        public int nCaptured { get; set; } = 0;
        public int nAbandoned {  get; set; } = 0;

        public int getTotal()
        {
            return nDamaged + nDestroyed + nCaptured + nAbandoned;
        }
        
        public LossListObject(string model)
        {
            this.model = model;
        }
        public void addStatus(string status)
        {
            switch (status)
            {
                case "Destroyed":
                    nDestroyed++;
                    break;
                case "Damaged":
                    nDamaged++;
                    break;
                case "Abandoned":
                    nAbandoned++;
                    break;
                case "Captured":
                    nCaptured++;
                    break;
            }
        }
        public override string ToString()
        {
            List<string> str = [$"{getTotal()}  {model}"];
            if (nDestroyed > 0)
                str.Add($"{nDestroyed} destroyed");
            if (nDamaged > 0)
                str.Add($"{nDamaged} damaged");
            if (nCaptured > 0)
                str.Add($"{nCaptured} captured");
            if (nAbandoned > 0)
                str.Add($"{nAbandoned} abandoned");
            return string.Join(", ", str);
           
        }

    }
}
