using GenericDataStore.Services;

namespace GenericDataStore.InputModels
{
    public class ClusteringResult
    {
        public List<ClusteringDataset> Datasets { get; set; } = new List<ClusteringDataset>();
    }

    public class ClusteringDataset
    {
        public string Label { get; set; }
        public List<List<float>> Data { get; set; }

        public string BorderColor { get; set; }
        public string BackgroundColor { get; set; }

    }

}
