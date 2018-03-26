using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        static void Main(string[] args)
            {
            GeneticOptimiser optimiser = new GeneticOptimiser();
            List<NeuralNet> pop = optimiser.CreatePopulation(50);
            List<NeuralNet> sorted_pop = optimiser.SortFitness(pop);
            Console.WriteLine();
            for (int i = 0; i < sorted_pop.Count(); i++)
                {
                Console.WriteLine("Fitness of network #" + i + ": " + sorted_pop[i].FITNESS);
                }
            Console.ReadLine();
            }

        public List<NeuralNet> Evolve


        public List<NeuralNet> CreatePopulation(int count)
            {
            List<NeuralNet> ret = new List<NeuralNet>();
            for (int i = 0; i < count; i++)
                {
                NeuralNet net = new NeuralNet(2, 4, 7, 1); //STATIC TOPOLOGY FOR NOW; LET THIS BECOME PART OF THE DNA WHEN TRYING OUT NEAT
                net.GenerateWeights(rand);
                ret.Add(net);
                }
#if DEBUG
            Console.WriteLine("Population of size " + count + " create OK!");
#endif
            return ret;
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

            ret = netlist.OrderBy(o => o.FITNESS).ToList();

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
                Console.WriteLine("Result difference: " + (XOR_IDEAL[i, 0] - outcome[0]));
#endif
                //diff += Math.Abs(XOR_IDEAL[i, 0] - outcome[0]);
                //JUST LIKE THE STANDARD DEVIATION PROBLEM, USING ABS. VALUES MAY NOT BE A GOOD CHOICE.
                diff += Math.Pow(XOR_IDEAL[i, 0] - outcome[0], 2);
                }
            //SMALLER THE BETTER!
            net.FITNESS = (diff / NUMBER_OF_DATASETS); //Average difference
#if DEBUG
            Console.WriteLine("Result Average Fitness: " + net.FITNESS);
#endif
            }
        }
    }
