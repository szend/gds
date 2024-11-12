namespace GenericDataStore.Models
{
    public class Field
    {
        public Field() 
        {
            this.Option = new HashSet<Option>();
        }
        public Guid FieldId { get; set; }

        public Guid ObjectTypeId { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }

        public string? CalculationMethod { get; set; }

        public string? ColorMethod { get; set; }


        public string? LabelColorMethod { get; set; }

        public string? SizeMethod { get; set; }


        public int? Position { get; set; }
        public bool? Visible { get; set; }
        public bool? DefaultOrder { get; set; }


        public virtual ICollection<Option> Option { get; set; }


    }
}
