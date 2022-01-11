using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace UStandard
{
    enum DataType
    {
        TRAIN, TEST
    }

    class Dataholder
    {
        public List<double[]> _inputs = new List<double[]>();
        public List<double> _target = new List<double>();
        public List<DataType> _dt = new List<DataType>();
        public double[] _xmin = null;
        public double[] _xmax = null;
        public double _targetMin;
        public double _targetMax;
        public int[] _LINEAR_BLOCKS_PER_INPUT = null;
        private Random _rnd = new Random();

        private void FindMinMax()
        {
            int size = _inputs[0].Length;
            _xmin = new double[size];
            _xmax = new double[size];

            for (int i = 0; i < size; ++i)
            {
                _xmin[i] = double.MaxValue;
                _xmax[i] = double.MinValue;
            }

            for (int i = 0; i < _inputs.Count; ++i)
            {
                for (int j = 0; j < _inputs[i].Length; ++j)
                {
                    if (_inputs[i][j] < _xmin[j]) _xmin[j] = _inputs[i][j];
                    if (_inputs[i][j] > _xmax[j]) _xmax[j] = _inputs[i][j];
                }

            }

            _targetMin = double.MaxValue;
            _targetMax = double.MinValue;
            for (int j = 0; j < _target.Count; ++j)
            {
                if (_target[j] < _targetMin) _targetMin = _target[j];
                if (_target[j] > _targetMax) _targetMax = _target[j];
            }
        }

        public void AssignDataTypes()
        {
            int size = _inputs.Count;
            for (int i = 0; i < size; ++i)
            {
                if (_rnd.Next() % 100 <= 80) _dt.Add(DataType.TRAIN);
                else
                {
                    _dt.Add(DataType.TEST);
                }
            }
        }

        private void ShowDataStat(string fileName)
        {
            //Stat of data sample
            int nTrain = 0;
            int nTest = 0;
            int nTotal = 0;
            foreach (DataType dt in _dt)
            {
                if (DataType.TRAIN == dt) ++nTrain;
                else if (DataType.TEST == dt) ++nTest;
                ++nTotal;
            }
            if (null == fileName)
                Console.WriteLine("Data is ready");
            else
                Console.WriteLine("Data is ready for file {0}", fileName);
            Console.WriteLine("Total = {0}, training sample = {1}, test sample = {2}, min target = {3:0.0000}, max target = {4:0.0000}",
                nTotal, nTrain, nTest, _targetMin, _targetMax);
        }

        public void BuildFormula2Data()
        {
            _LINEAR_BLOCKS_PER_INPUT = new int[] { 10, 10, 10, 10, 10 };

            _inputs.Clear();
            _target.Clear();
            _dt.Clear();

            int N = 10000;
            for (int i = 0; i < N; ++i)
            {
                double[] x = new double[5];
                x[0] = (_rnd.Next() % 100) / 100.0;
                x[1] = (_rnd.Next() % 100) / 100.0;
                x[2] = (_rnd.Next() % 100) / 100.0;
                x[3] = (_rnd.Next() % 100) / 100.0;
                x[4] = (_rnd.Next() % 100) / 100.0;
    
                //y = (1/pi)*(2+2*x3)*(1/3)*(atan(20*exp(x5)*(x1-0.5+x2/6))+pi/2) + (1/pi)*(2+2*x4)*(1/3)*(atan(20*exp(x5)*(x1-0.5-x2/6))+pi/2);
                double pi = 3.14159265359;
                if (5 != x.Length)
                {
                    Console.WriteLine("Formala error");
                    Environment.Exit(0);
                }
                double y = (1.0 / pi);
                y *= (2.0 + 2.0 * x[2]);
                y *= (1.0 / 3.0);
                y *= Math.Atan(20.0 * Math.Exp(x[4]) * (x[0] - 0.5 + x[1] / 6.0)) + pi / 2.0;

                double z = (1.0 / pi);
                z *= (2.0 + 2.0 * x[3]);
                z *= (1.0 / 3.0);
                z *= Math.Atan(20.0 * Math.Exp(x[4]) * (x[0] - 0.5 - x[1] / 6.0)) + pi / 2.0;

                double target = y + z;
                _inputs.Add(x);
                _target.Add(y);
            }
            AssignDataTypes();
            FindMinMax();
            ShowDataStat(null);
        }

        public void BuildFormula1Data()
        {
            _LINEAR_BLOCKS_PER_INPUT = new int[] { 10, 10, 10, 10, 10 };

            _inputs.Clear();
            _target.Clear();
            _dt.Clear();

            int N = 10000;
            for (int i = 0; i < N; ++i)
            {
                double[] x = new double[5];
                x[0] = (_rnd.Next() % 100) / 100.0;
                x[1] = (_rnd.Next() % 100) / 100.0 * 3.14 / 2.0;
                x[2] = (_rnd.Next() % 50) / 100.0 + 1;
                x[3] = (_rnd.Next() % 100) / 100.0 + 0.4;
                x[4] = (_rnd.Next() % 100) / 200.0;

                double y = Math.Abs(Math.Pow(Math.Sin(x[1]), x[0]) - 1.0 / Math.Exp(x[2])) / x[3] + x[4] * Math.Cos(x[4]);
                _inputs.Add(x);
                _target.Add(y);
            }
            AssignDataTypes();
            FindMinMax();
            ShowDataStat(null);
        }

        private double GetLength(double x1, double x2, double y1, double y2)
        {
            return Math.Sqrt((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2));
        }

        public void BuildTrianglesData(string filename = null)
        {
            StreamWriter sw = null;
            if (null != filename)
            {
                sw = new StreamWriter(filename);
            }

            _LINEAR_BLOCKS_PER_INPUT = new int[] { 3, 3, 3, 3, 3, 3 };

            _inputs.Clear();
            _target.Clear();
            _dt.Clear();

            double xmin = 0.0;
            double xmax = 100.0;
            double ymin = 0.0;
            double ymax = 100.0;

            if (null != sw)
            {
                sw.WriteLine("x1,x2,x3,x4,x5,x6,y");
            }

            for (int i = 0; i < 10000; ++i)
            {
                double[] x = new double[6];

                //x1
                x[0] = _rnd.Next() % xmax;
                if (x[0] < xmin) x[0] = xmin;

                //x2
                x[1] = _rnd.Next() % xmax;
                if (x[1] < xmin) x[1] = xmin;

                //x3
                x[2] = _rnd.Next() % xmax;
                if (x[2] < xmin) x[2] = xmin;

                //y1
                x[3] = _rnd.Next() % ymax;
                if (x[3] < ymin) x[3] = ymin;

                //y2
                x[4] = _rnd.Next() % ymax;
                if (x[4] < ymin) x[4] = ymin;

                //y3
                x[5] = _rnd.Next() % ymax;
                if (x[5] < ymin) x[5] = ymin;
                _inputs.Add(x);

                double l1 = GetLength(x[0], x[1], x[3], x[4]);
                double l2 = GetLength(x[0], x[2], x[3], x[5]);
                double l3 = GetLength(x[1], x[2], x[4], x[5]);
                double s = (l1 + l2 + l3) / 2.0;
                double target = 0.0;
                if ((s - l2) < 0.0 || (s - l2) < 0.0 || (s - l3) < 0.0) target = 0.0;
                else target = Math.Sqrt(s * (s - l1) * (s - l2) * (s - l3));

                _target.Add(target);

                if (null != sw)
                {
                    String sdata = String.Format("{0:0.00}, {1:0.00}, {2:0.00}, {3:0.00}, {4:0.00}, {5:0.00}, {6:0.00}",
                        x[0], x[1], x[2], x[3], x[4], x[5], target);
                    sw.WriteLine(sdata);
                }
            }

            if (null != sw)
            {
                sw.Flush();
                sw.Close();
            }
            AssignDataTypes();
            FindMinMax();
            ShowDataStat(null);
        }

        public void BuildAirfoilData()
        {
            _LINEAR_BLOCKS_PER_INPUT = new int[] { 11, 11, 11, 11, 11 };

            _inputs.Clear();
            _target.Clear();
            _dt.Clear();

            string fileName = @"..\..\..\data\airfoil_self_noise.csv";
            if (!File.Exists(fileName))
            {
                Console.WriteLine("File not found {0}", fileName);
                Environment.Exit(0);
            }
            string[] lines = System.IO.File.ReadAllLines(fileName);
            for (int m = 1; m < lines.Length; ++m) //we skip first line
            {
                string[] sdata = lines[m].Split(';');
                if (6 != sdata.Length)
                {
                    Console.WriteLine("Misformatted data");
                    Environment.Exit(0);
                }
                double[] data = new double[5];
                for (int i = 0; i < data.Length; ++i)
                {
                    Double.TryParse(sdata[i], out data[i]);
                }
                double target = 0.0;
                Double.TryParse(sdata[5], out target);
                _inputs.Add(data);
                _target.Add(target);
            }

            AssignDataTypes();
            FindMinMax();
            ShowDataStat(fileName);
        }
    }
}


