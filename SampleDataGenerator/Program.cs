using System;
using System.IO;
using System.Text;

class Program
{
    static Random random = new Random();

    static void Main(string[] args)
    {
        StringBuilder csvContent = new StringBuilder();
        csvContent.AppendLine("durationInMonths,isMarried,bsDegree,msDegree,yearsExperience,ageAtHire,hasKids,withinMonthOfVesting,deskDecorations,longCommute");

        for (int i = 0; i < 100000; i++)
        {
            csvContent.AppendLine($"{RandomNumber(0, 180)},{RandomNumber(0, 2)},{RandomNumber(0, 2)},{RandomNumber(0, 2)},{RandomNumber(0, 30)},{RandomNumber(20, 60)},{RandomNumber(0, 2)},{RandomNumber(0, 2)},{RandomNumber(0, 2)},{RandomNumber(0, 2)}");
        }

        File.WriteAllText("sampledata.csv", csvContent.ToString());
        Console.WriteLine("Dataset saved to sampledata.csv with 100000 rows.");
    }

    static int RandomNumber(int min, int max)
    {
        return random.Next(min, max);
    }
}

