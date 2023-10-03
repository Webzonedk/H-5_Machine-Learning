using Microsoft.ML.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkforceTeardown.Interfaces.Models
{
    internal class EmploymentHistory
    {
        [LoadColumn(0)]
        public float DurationInMonths { get; set; }
        [LoadColumn(1)]
        public float IsMarried { get; set; }
        [LoadColumn(2)]
        public float BsDegree { get; set; }
        [LoadColumn(3)]
        public float MsDegree { get; set; }
        [LoadColumn(4)]
        public float YearsExperience { get; set; }
        [LoadColumn(5)]
        public float AeAtHire { get; set; }
        [LoadColumn(6)]
        public float HasKids { get; set; }
        [LoadColumn(7)]
        public float WithinMonthOfVesting { get; set; }
        [LoadColumn(8)]
        public float DeskDecorations { get; set; }
        [LoadColumn(9)]
        public float LongCommute { get; set; }
    }
}
