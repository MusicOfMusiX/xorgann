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

        public static int[] NODECONFIG = new int[] { 2, 5, 4, 1 };
        public static int POPULATION_SIZE = 50;
        public static int GENERATIONS = 100;

        public static double MUTATE_CHANCE = 0.3;       //Increase diversity
        public static double CROSSOVER_CHANCE = 0.5;    //Unlike muatate chance, this determines whether an individual node will be crossed;
                                                        //instead of whether crossover itself should happen across the entire population.
        public static double UNDERDOG_SURVIVAL = 0.3;   //Increase diversity
        public static int SURVIVORS = 3;                //Must be larger than 0; but equal or less than the population size.

        /*ONLY WEIGHTS ARE EVOLVED; THE TOPOLOGY IS UNTOUCHED THROUGHOUT THE ENTIRE PROCESS*/

        public Random rand = new Random();

        static void Main(string[] args)
            {
            GeneticOptimiser optimiser = new GeneticOptimiser();
            
            List<NeuralNet> pop = optimiser.CreatePopulation(POPULATION_SIZE, NODECONFIG);

            for (int i = 0; i < GENERATIONS; i++)
                {
                //Console.WriteLine("##########################GENERATION #" + i + "##########################\n\n");
                pop = optimiser.Evolve(pop);
                optimiser.EvaluatePopulation(pop);
                }

            Console.WriteLine("\n\n#####FINAL RESULTS#####");
            for (int i = 0; i < NUMBER_OF_DATASETS; i++)
                {
                optimiser.TestBestNetwork(pop, XOR_INPUT.GetRow(i).ToList());
                }
            Console.ReadLine();
            }

        public void TestBestNetwork(List<NeuralNet> nets, List<double> input)
            {
            //XOR EXCLUSIVE!!!
            double ret;
            List<double> tmp = nets[0].Run(input);
            ret = tmp[0];
            Console.WriteLine("Input: {" + input[0] + ", " + input[1] + "}, result: " + ret);
            }

        public List<NeuralNet> Evolve(List<NeuralNet> nets)
            {
            List<NeuralNet> ret = new List<NeuralNet>();
            ret = SortFitness(nets);
            ret = Cutoff(ret);
            ret = Crossover(ret,POPULATION_SIZE);

            ret = Mutate(ret);
            return ret;
            }

        private List<NeuralNet> Cutoff(List<NeuralNet> nets)
            {
            List<NeuralNet> ret = nets;
            for(int i=nets.Count()-1;i>=SURVIVORS;i--)  //Lists automatically update after a RemoveAt(); Therefore interate from the back!
                {
                if(rand.NextDouble()>UNDERDOG_SURVIVAL)
                    {
                    ret.RemoveAt(i);
                    }          
                }
            return ret;
            }

        private List<NeuralNet> Mutate(List<NeuralNet> nets)
            {
            List<NeuralNet> ret = nets; //THIS DOES NOT CREATE A COPY!!! THIS JUST PASSES A REFERENCE; THE DESTINATION IS THE SAME!!
            for (int i = 0; i < ret.Count(); i++)
                {
                for (int mat = 0; mat < ret[i].MATRICIES.Count(); mat++)
                    {
                    for (int row = 0; row < ret[i].MATRICIES[mat].Count(); row++)
                        {
                        for (int col = 0; col < ret[i].MATRICIES[mat][row].Count(); col++)
                            {
                            if (rand.NextDouble() < MUTATE_CHANCE)
                                {

                                ret[i].SetWeight(mat, row, col, ret[i].GetWeight(mat,row,col)+rand.NextDouble()-0.5);
                                }
                            }
                        }
                    }
                }
            return ret;
            }

        private List<NeuralNet> Crossover(List<NeuralNet> nets, int desiredsize)    //This will also fill the "Cutoffed" population w/ children.
            {
            List<NeuralNet> ret = nets; 
            int size = nets.Count();

            for(int i=0;i<desiredsize-size;i++) //BE CAREFUL WHEN USING nets AND ret!! nets STAYS CONSTANT!!
                {
                NeuralNet father = ret[0];
                NeuralNet mother = ret[1];
                NeuralNet offspring = new NeuralNet(NODECONFIG);
                offspring.GenerateWeights(rand); //I DON'T LIKE THIS. BUT THIS WILL DO FOR NOW...
                
                for(int mat=0;mat<offspring.MATRICIES.Count();mat++)
                    {
                    for(int row=0;row<offspring.MATRICIES[mat].Count();row++)
                        {
                        for(int col=0;col<offspring.MATRICIES[mat][row].Count();col++)
                            {
                            if(rand.NextDouble() < CROSSOVER_CHANCE)
                                {

                                offspring.SetWeight(mat,row,col,mother.GetWeight(mat,row,col));
                                }
                            else
                                {
                                offspring.SetWeight(mat, row, col, father.GetWeight(mat, row, col));
                                }
                            }
                        }
                    }
                ret.Add(offspring);
                }
            return ret;
            }

        
        public List<NeuralNet> CreatePopulation(int count, int[] nodeconfig)
            {
            List<NeuralNet> ret = new List<NeuralNet>();
            for (int i = 0; i < count; i++)
                {
                NeuralNet net = new NeuralNet(nodeconfig); //STATIC TOPOLOGY FOR NOW; LET THIS BECOME PART OF THE DNA WHEN TRYING OUT NEAT
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
                /*
#if DEBUG
                Console.WriteLine("Input count: " + XOR_INPUT.GetRow(i).ToList().Count());
#endif          */
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
        private void EvaluatePopulation(List<NeuralNet> nets)
            {
            double fitness = 0;
            for (int i = 0; i < nets.Count(); i++)
                {
                fitness += nets[i].FITNESS;
                }
            //Console.WriteLine("\n######POPULATION AVG. FITNESS: " + fitness/POPULATION_SIZE + " ######");
            Console.WriteLine(fitness / POPULATION_SIZE);
            }
        }
    }
