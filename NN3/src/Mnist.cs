using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NN3.src
    {
    class MnistTrainImages
        {
        private byte[] RAW = System.IO.File.ReadAllBytes(@"C:\Users\MusicOfMusiX\Desktop\CSPROJECTS\C\NN3\NN3\test\train-images.idx3-ubyte");
        private int OFFSET = 0x10; //4 bytes * 4

        public double[] GetImage(int index) //Return an 28*28 array of 0s and 1s
            {
            double[] ret = new double[28 * 28];
            for (int i = 0; i < 28 * 28; i++)
                {
                if (RAW[OFFSET + index * 28 * 28 + i] > 0)
                    {
                    ret[i] = 1;
                    }
                else
                    {
                    ret[i] = 0;
                    }
                }
            return ret;
            }

        public double[] GetZscores(int index)
            {
            double[] ret = new double[28 * 28];
            for (int i = 0; i < 28 * 28; i++)
                {
                if (RAW[OFFSET + index * 28 * 28 + i] > 0)
                    {
                    ret[i] = 1;
                    }
                else
                    {
                    ret[i] = 0;
                    }
                }
            double stdev = CalculateStdDev(ret.ToList());
            double mean = Enumerable.Average(ret.ToList());
            for (int i = 0; i < 28 * 28; i++)
                {
                ret[i] = (ret[i]-mean)/stdev;
                }
            return ret;
            }



        private double CalculateStdDev(IEnumerable<double> values)
            {
            double ret = 0;
            if (values.Count() > 0)
                {
                //Compute the Average      
                double avg = values.Average();
                //Perform the Sum of (value-avg)_2_2      
                double sum = values.Sum(d => Math.Pow(d - avg, 2));
                //Put it all together      
                ret = Math.Sqrt((sum) / (values.Count() - 1));
                }
            return ret;
            }
        }
    class MnistTrainLabels
        {
        private byte[] RAW = System.IO.File.ReadAllBytes(@"C:\Users\MusicOfMusiX\Desktop\CSPROJECTS\C\NN3\NN3\test\train-labels.idx1-ubyte");
        private int OFFSET = 0x8; //4 bytes * 2

        public int GetLabel(int index) //Return values from 0 to 9 
            {
            //return Convert.ToDouble((int)RAW[OFFSET + index]);
            return (int)RAW[OFFSET + index];
            }
        }
    }
