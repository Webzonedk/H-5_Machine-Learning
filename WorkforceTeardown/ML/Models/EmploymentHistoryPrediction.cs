using Microsoft.ML.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkforceTeardown.Interfaces.Models
{
    internal class EmploymentHistoryPrediction
    {
        [ColumnName("Score")]
        public float PredictedMonths { get; set; }
    }
}
