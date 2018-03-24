using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
TRYING OUT OBJECTTIVE #2: XOR GATE SIMULATION!!!
*/

namespace NN3
    {
    class GeneticOptimiser
        {
        //XOR DATASETS
        private static int NUMBER_OF_DATASETS = 4;
        private static double[,] XOR_INPUT = { { 0.0, 0.0 }, { 1.0, 0.0 }, { 0.0, 1.0 }, { 1.0, 1.0 } };
        private static double[,] XOR_IDEAL = { { 0.0 }, { 1.0 }, { 1.0 }, { 0.0 } };

        //private Random rand = new Random();

        Random rand = new Random();

        public List<NeuralNet> CreatePopulation(int count)
            {
            List<NeuralNet> ret = new List<NeuralNet>();
            for (int i = 0; i < count; i++)
                {
                NeuralNet net = new NeuralNet(2, 1); //STATIC TOPOLOGY FOR NOW; LET THIS BECOME PART OF THE DNA WHEN TRYING OUT NEAT
                net.GenerateWeights(rand);
                ret.Add(net);
                }
#if DEBUG
            Console.WriteLine("Population of size " + count + " create OK!");
#endif
            return ret;
            }

        private void EvaluateFitness(NeuralNet net) //Probably the 'Main' part of the class; The real 'running & testing happens here'
            {
            //DUE TO SORTING ISSUES, THIS FUNCTION WILL ASSIGN THE VALUE TO THE NETWORK'S PUBLIC VARIABLE - [FITNESS]. 
            
            //XOR - SPECIFIC CODE!!!
            double diff = 0;
#if DEBUG
            Console.WriteLine("Dataset count: " + NUMBER_OF_DATASETS);
#endif
            for (int i = 0; i < NUMBER_OF_DATASETS; i++)
                {
#if DEBUG
                Console.WriteLine("Input count: " + XOR_INPUT.GetRow(i).ToList().Count());
#endif
                List<double> outcome = net.Run(XOR_INPUT.GetRow(i).ToList());

#if DEBUG
                Console.WriteLine("Result difference: " + Math.Abs(XOR_IDEAL[i, 0] - outcome[0]));
#endif

                diff += Math.Abs(XOR_IDEAL[i, 0] - outcome[0]);
                }
            //LARGER THE BETTER; hence the "1 -" part
            net.FITNESS = 1-(diff / NUMBER_OF_DATASETS); //Average difference
#if DEBUG
            Console.WriteLine("Result Average Fitness: " + net.FITNESS);
#endif
            }

        public List<NeuralNet> SortFitness(List<NeuralNet> netlist)
            {
            List<NeuralNet> ret = new List<NeuralNet>(); //Create list of nets to order in fitness

            for (int i = 0; i < netlist.Count(); i++)
                {
#if DEBUG
                Console.WriteLine("\n" + "Testing network #" + i);
#endif
                EvaluateFitness(netlist[i]);
                }

            List<NeuralNet> SortedList = netlist.OrderBy(o => o.FITNESS).ToList();

            return ret;
            }
        
        static void Main(string[] args)
            {
            GeneticOptimiser optimiser = new GeneticOptimiser();
            List<NeuralNet> pop = optimiser.CreatePopulation(5);
            List<NeuralNet> sorted_pop = optimiser.SortFitness(pop);
            for (int i = 0; i < sorted_pop.Count(); i++)
                {
                Console.WriteLine("Fitness of network #" + i + ": " + sorted_pop[i].FITNESS);
                }

            /*
            NeuralNet net = new NeuralNet(2,4,5,3,1);
            net.GenerateWeights();
            List<double> inputs = new List<double> { 1, 2 };
            net.Run(inputs);
            */
            Console.ReadLine();
            }
        }

    class NeuralNet
        {
        private int NUMBER_OF_LAYERS;
        private const int IDENTITY = 1;
        private List<int> NUMBER_OF_NODES = new List<int>();
        private List<List<List<double>>> MATRICIES = new List<List<List<double>>>();

        public double FITNESS;

        

        public bool ACTIVATION = true;

        public NeuralNet(params int[] nodes)
            {
            NUMBER_OF_LAYERS = nodes.Count();
            NUMBER_OF_NODES.Add(IDENTITY); //Since the first node's (INPUT) row is always 1 and is not provided as an argument in the constructor!
            //Therefore, if there are n number of layers, NUMBER_OF_LAYERS = n, while NUMBER_OF_NODES will have n+1 elements
            for (int i = 0; i < NUMBER_OF_LAYERS; i++)
                {
                NUMBER_OF_NODES.Add(nodes[i]);
                }
            }

        public void GenerateWeights(Random rand)
            {
            /*
            Console.Write("Please input a seed: ");
            int seed = Convert.ToInt32(Console.Read());
            Console.WriteLine();
            */

            Console.WriteLine("\n\n");
            //The input matrix is assigned some random values as well. These will be overwritten later.
            for (int i = 0; i < NUMBER_OF_LAYERS; i++)
                {
                List<List<double>> jList = new List<List<double>>();
                for (int j = 0; j < NUMBER_OF_NODES[i]; j++)
                    {
                    List<double> kList = new List<double>();
                    for (int k = 0; k < NUMBER_OF_NODES[i + 1]; k++)
                        {
                        double w = rand.NextDouble();
                        kList.Add(w);
#if DEBUG
                        Console.WriteLine("Weight config: " + w);
#endif

                        }
                    jList.Add(kList);
                    }
                MATRICIES.Add(jList);
                }
            }

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
                mat[i] = 1/(1 + Math.Exp(mat[i]*-1));
                }
            }

        public List<double> Run(List<double> inputs)
            {
            //ALL MATRIX ARRAYS ARE [ROW/Y][COL/X]!!
            List<double> ret = inputs;

            for (int i = 0; i < NUMBER_OF_LAYERS - 1; i++)
                {
                ret = Multi(ref ret, MATRICIES[i + 1]);
                if (ACTIVATION) { Activate(ref ret); }
                }
            //
            return ret;
            }
        }
    }

public static class Extenstions
    {
    public static void Add<T>(this List<T> list, params T[] elements)
        {
        list.AddRange(elements);
        }

    public static T[] GetRow<T>(this T[,] input2DArray, int row) where T : IComparable
        {
        var height = input2DArray.GetLength(0);
        var width = input2DArray.GetLength(1);

        if (row >= height)
            throw new IndexOutOfRangeException("Row Index Out of Range");
        // Ensures the row requested is within the range of the 2-d array


        var returnRow = new T[width];
        for (var i = 0; i < width; i++)
            returnRow[i] = input2DArray[row, i];

        return returnRow;
        }
    }