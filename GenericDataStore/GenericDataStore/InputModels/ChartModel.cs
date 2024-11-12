namespace GenericDataStore.InputModels
{
    public class ChartModel
    {
        public List<ChartModelType> Options { get; set; }
        public List<ChartModelType> Numbers { get; set; }

        public List<ChartModelType> Booleans { get; set; }

        public ChartModelOrganisation Organisation { get; set; }


    }

    public class ChartModelType
    {
        public bool privateobject { get; set; }
        public string Name { get; set; }
        public List<string> Labels { get; set; } = new List<string>();
        public List<ChartModelDataset> Datasets { get; set; } = new List<ChartModelDataset>();
    }

    public class ChartModelOrganisation
    {
        public string? Name { get; set; }

        public string? StyleClass { get; set; }

        public List<ChartModelOrganisationData?> Data { get; set; } = new List<ChartModelOrganisationData?>();
        public List<ChartModelOrganisationLink> Links { get; set; } = new List<ChartModelOrganisationLink>();
    }

    public class ChartModelOrganisationData
    {
        public string? Name { get; set; }
        public Guid? ObjId { get; set; }

        public int X { get; set; }
        public int Y { get; set; }

        public bool Private { get; set; }

    }

    public class ChartModelOrganisationLink
    {
        public string Source { get; set; }
        public string Target { get; set; }
        public LineStyle LineStyle { get; set; }

    }

    public class LineStyle
    {
        public int Width { get; set; }
        public string Color { get; set; }
        public double Curveness { get; set; }
    }



    public class ChartModelDataset
    {
        public List<double> Data { get; set; } = new List<double>();

        public string? Regression { get; set; }
        public string? Stepped { get; set; }

        public string? Fill { get; set; }

        public bool? Stacked { get; set; }

        public string Label { get; set; }
        public List<string> BackgroundColor { get; set; } = new List<string>();
        public List<string> BorderColor { get; set; } = new List<string>();

        public string? Type { get; set; }

        //public int Radius { get; set; } = 50;

        //public int BorderWidth { get; set; } = 5;


    }
}
