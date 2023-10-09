using Microsoft.ML.Data;

namespace ForexAnalyzer.MachineLearning.Models
{
    public class ForexData
    {
        [LoadColumn(0), ColumnName("Date")]
        public string Date { get; set; }

        [LoadColumn(1), ColumnName("Open")]
        public float Open { get; set; }

        [LoadColumn(2), ColumnName("High")]
        public float High { get; set; }

        [LoadColumn(3), ColumnName("Low")]
        public float Low { get; set; }

        [LoadColumn(4), ColumnName("Close")]
        public float Close { get; set; }

        [LoadColumn(5), ColumnName("Volume")]
        public float Volume { get; set; }
    }
}
