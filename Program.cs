using System;

namespace UStandard
{
    class Program
    {
        static void Main(string[] args)
        {
            //string fileName = @"..\..\..\data\triangle.csv"; 
            Dataholder dh = new Dataholder();

            //dh.BuildFormula1Data();
            //dh.BuildAirfoilData();
            dh.BuildTrianglesData();
            //dh.BuildFormula2Data();
            DateTime start = DateTime.Now;

            KolmogorovModel km = new KolmogorovModel(dh);

            int NLeaves = 50;
            int[] linearBlocksPerRootInput = new int[NLeaves];
            for (int i = 0; i < NLeaves; ++i)
            {
                linearBlocksPerRootInput[i] = 10;
            }

            km.GenerateInitialOperators(NLeaves, linearBlocksPerRootInput);
            km.BuildRepresentation(100, 0.05, 0.2);
            double testdataError = km.DoTest();

            double pearson = km.ComputeCorrelationCoeff();
            Console.WriteLine("Test sample RMSE = {0:0.0000}, pearson correlation coeff. for modelled and actual outputs = {1:0.0000}",
                testdataError, pearson);
            DateTime end = DateTime.Now;
            TimeSpan duration = end - start;
            double time = duration.Minutes * 60.0 + duration.Seconds + duration.Milliseconds / 1000.0;
            Console.WriteLine("Time {0:####.00} seconds", time);
        }
    }
}
