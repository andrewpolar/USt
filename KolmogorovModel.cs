using System;
using System.Collections.Generic;
using System.Text;

namespace UStandard
{
    class KolmogorovModel
    {
        private Dataholder _dh = null;
        private List<U> _ulist = new List<U>();
        private U _bigU = null;
        private Random _rnd = new Random();
 
        public KolmogorovModel(Dataholder DH)
        {
            _dh = DH;
        }

        public void GenerateInitialOperators(int Nleaves, int[] linearBlocksPerRootInput)
        {
            _ulist.Clear();
            int points = _dh._inputs[0].Length;
            int[] inputIndexesLeaves = new int[points];
            for (int i = 0; i < points; ++i)
            {
                inputIndexesLeaves[i] = i;
            }

            for (int counter = 0; counter < Nleaves; ++counter)
            {
                U uc = new U(inputIndexesLeaves);
                uc.Initialize(points, _dh._LINEAR_BLOCKS_PER_INPUT, _dh._xmin, _dh._xmax);
                uc.SetRandom(_dh._targetMin, _dh._targetMax);
                _ulist.Add(uc);
            }

            if (null != _bigU)
            {
                _bigU.Clear();
                _bigU = null;
            }

            int[] inputIndexesRoot = new int[Nleaves];
            for (int k = 0; k < Nleaves; ++k)
            {
                inputIndexesRoot[k] = k;
            }

            double[] min = new double[Nleaves];
            double[] max = new double[Nleaves];
            for (int i = 0; i < Nleaves; ++i)
            {
                min[i] = _dh._targetMin;
                max[i] = _dh._targetMax;
            }

            _bigU = new U(inputIndexesRoot);
            _bigU.Initialize(Nleaves, linearBlocksPerRootInput, min, max);
            _bigU.SetRandom(_dh._targetMin, _dh._targetMax);
        }

        private double[] GetVector(double[] data)
        {
            int size = _ulist.Count;
            double[] vector = new double[size];
            for (int i = 0; i < size; ++i)
            {
                vector[i] = _ulist[i].GetU(data);
            }
            return vector;
        }

        public void BuildRepresentation(int steps, double muRoot, double muLeaves)
        { 
            for (int step = 0; step < steps; ++step)
            {
                double RMSE = 0.0;
                int cnt = 0;
                for (int i = 0; i < _dh._inputs.Count; ++i)
                {
                    if (DataType.TRAIN != _dh._dt[i]) continue;
                    double[] data = _dh._inputs[i];
                    double[] v = GetVector(data);
                    double diff = _dh._target[i] - _bigU.GetU(v);

                    for (int k = 0; k < _ulist.Count; ++k)
                    {
                        if (v[k] > _dh._targetMin && v[k] < _dh._targetMax)
                        {
                            double derrivative = _bigU.GetDerrivative(k, v[k]);
                            _ulist[k].Update(diff / v.Length, data, muLeaves * derrivative);
                        }
                    }

                    _bigU.Update(diff, v, muRoot);

                    RMSE += diff * diff;
                    ++cnt;
                }

                RMSE /= cnt;
                RMSE = Math.Sqrt(RMSE);
                if (step == steps - 1)
                {
                    Console.WriteLine("Training is completed, relative error {0:0.0000}", RMSE);
                }
            }        

            _bigU.ShowOperatorLimits();
        }

        public double DoTest()
        {
            double RMSE = 0.0;
            int cnt = 0;
            int N = _dh._inputs.Count;
            for (int i = 0; i < N; ++i)
            {
                if (DataType.TEST != _dh._dt[i]) continue;
                double[] data = _dh._inputs[i];
                double[] v = GetVector(data);
                double diff = _dh._target[i] - _bigU.GetU(v);
                RMSE += diff * diff;
                ++cnt;
            }
            RMSE /= cnt;
            RMSE = Math.Sqrt(RMSE);
            //RMSE /= (_targetMax - _targetMin);
            return RMSE;
        }

        public double ComputeCorrelationCoeff()
        {
            int N = _dh._inputs.Count;
            double[] targetEstimate = new double[N];
            int count = 0;
            for (int i = 0; i < N; ++i)
            {
                double[] data = _dh._inputs[i];
                double[] v = GetVector(data);
                targetEstimate[i] = _bigU.GetU(v);
                if (DataType.TEST == _dh._dt[i]) ++count;
            }
            double[] x = new double[count];
            double[] y = new double[count];
            count = 0;
            for (int i = 0; i < N; ++i)
            {
                if (DataType.TEST == _dh._dt[i])
                {
                    x[count] = targetEstimate[i];
                    y[count] = _dh._target[i];
                    ++count;
                }
            }
            return Static.PearsonCorrelation(x, y);
        }
    }
}
