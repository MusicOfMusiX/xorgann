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
        }
    class MnistTrainLabels
        {
        private byte[] RAW = System.IO.File.ReadAllBytes(@"C:\Users\MusicOfMusiX\Desktop\CSPROJECTS\C\NN3\NN3\test\train-labels.idx1-ubyte");
        private int OFFSET = 0x8; //4 bytes * 2

        public double GetLabel(int index) //Return values from 0 to 9 
            {
            return Convert.ToDouble((int)RAW[OFFSET + index]);
            }
        }
    }
