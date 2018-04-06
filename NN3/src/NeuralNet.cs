using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
TRYING OUT OBJECTTIVE #3: MNIST HANDWRITING CLASSIFICATION!!!
*/

namespace NN3.src
    {
    class NeuralNet
        {
        public int NUMBER_OF_LAYERS;
        public List<int> NUMBER_OF_NODES = new List<int>();

        //THE MOST IMPORTANT LIST!! - Mat# - Row# - Col# Order.
        public List<List<List<double>>> MATRICES = new List<List<List<double>>>(); //A.K.A. WEIGHTS

        public void SetWeight(int mat, int row, int col, double val) //DON'T BE CONFUSED!!
            {
            MATRICES[mat][row][col] = val;
            }

        public double GetWeight(int mat, int row, int col)
            {
            return MATRICES[mat][row][col];
            }

        public double FITNESS;

        public bool ACTIVATION = false;

        public NeuralNet(int[] nodes)
            {
            NUMBER_OF_LAYERS = nodes.Count();
            //NUMBER_OF_NODES.Add(IDENTITY); //Since the first node's (INPUT) row is always 1 and is not provided as an argument in the constructor!
            //Therefore, if there are n number of layers, NUMBER_OF_LAYERS = n, while NUMBER_OF_NODES will have n+1 elements
            for (int i = 0; i < NUMBER_OF_LAYERS; i++)
                {
                NUMBER_OF_NODES.Add(nodes[i]);
                }
            }

        public void GenerateWeights(Random rand)
            {
            //Console.WriteLine("\n\n");
            //The input matrix is assigned some random values as well. These will be overwritten later. - NOPE.
            //FIXED THE CODE, [MATRICES] NOW DOES NOT CONTAIN THE INPUT MATRIX. JUST DIDN'T NEED IT IN THE FIRST PLACE1
            for (int i = 0; i < NUMBER_OF_LAYERS-1; i++)
                {
                List<List<double>> jList = new List<List<double>>();
                for (int j = 0; j < NUMBER_OF_NODES[i]; j++)
                    {
                    List<double> kList = new List<double>();
                    for (int k = 0; k < NUMBER_OF_NODES[i + 1]; k++)
                        {
                        //double w = (rand.NextDouble()-0.5)*2; //For tanh
                        double w = rand.NextDouble();   //For sigmoid
                        kList.Add(w);
#if DEBUG
Console.WriteLine("Weight config: " + w);
#endif
                        }
                    jList.Add(kList);
                    }
                MATRICES.Add(jList);
                }
            }

        public List<double> Run(List<double> inputs)
            {
            //ALL MATRIX ARRAYS ARE [ROW/Y][COL/X]!!

            List<double> ret = inputs;

            for (int i = 0; i < NUMBER_OF_LAYERS - 1; i++)
                {
                ret = Multi(ref ret, MATRICES[i]);
                if (ACTIVATION) { Activate(ref ret); }
                }
            return ret;
            }
        /*
        public List<double> Encode() //NO REAL NEED FOR ENCODING; JUST ACCESS WEIGHTS DIRECTLY VIA LISTS!!
        */

        private List<double> Multi(ref List<double> res, List<List<double>> weight)
            {
            List<double> ret = new List<double>();
            int shared = res.Count();
            if (shared != weight.Count())
                { throw new System.ArgumentException("Matrix 1 col and Matrix 2 row do not match!");}
            int weightcol = weight[0].Count();

            for (int i = 0; i < weightcol; i++)
                {
                double element = 0;
                for (int j = 0; j < shared; j++)
                    {
                    element += res[j] * weight[j][i];
                    }
                ret.Add(element);
                }
            return ret;
            }

        private void Activate(ref List<double> mat)
            {
            for (int i = 0; i < mat.Count(); i++)
                {
                mat[i] = 1/(1 + Math.Exp(mat[i]*-1));   //Sigmoid, 0 ~ 1
                //mat[i] = Math.Tanh(mat[i]);   //Tanh, -1 ~ 1
                }
            }
        }
    }

