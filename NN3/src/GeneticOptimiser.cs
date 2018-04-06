using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NN3.src
    {
    class GeneticOptimiser
        {
        public static int NUMBER_OF_DATASETS = 60000;

        public static int[] NODECONFIG = new int[] { 28*28, 10 };
        public static int POPULATION_SIZE = 30;
        public static int GENERATIONS = 300; //This is static; the Test DB has 60000 samples!

        //NOT THE CHANCES OF MUTATION/CROSSOVER HAPPENING ENTIRELY ACROSS A POPULATION; BUT RATHER THE CHANCE OF AN INDIVIDUAL NODE UNDERGOING MUTAION/CROSSOVER.
        public static double MUTATE_CHANCE = 0.2;
        public static double CROSSOVER_CHANCE = 0.5;    
        public static double UNDERDOG_SURVIVAL = 0.05;   //Increase diversity
        public static int SURVIVORS = 3;                //Must be larger than 0; but equal or less than the population size.

        /*ONLY WEIGHTS ARE EVOLVED; THE TOPOLOGY IS UNTOUCHED THROUGHOUT THE ENTIRE PROCESS*/

        public Random rand = new Random();

        static void Main(string[] args)
            {
            GeneticOptimiser optimiser = new GeneticOptimiser();
            
            List<NeuralNet> pop = optimiser.CreatePopulation(POPULATION_SIZE, NODECONFIG);

            //Load Training Data
            MnistTrainImages trainimages = new MnistTrainImages();
            MnistTrainLabels trainlabels = new MnistTrainLabels();

            for (int i = 0; i < GENERATIONS; i++)
                {
#if DEBUG
Console.WriteLine("##########################GENERATION #" + i + "##########################\n\n");
#endif
                pop = optimiser.Evolve(pop, i, trainimages, trainlabels);
                //optimiser.EvaluatePopulation(pop);
                }
            Console.WriteLine("FINISHED!");
            optimiser.TestBestNetwork(pop,trainimages,trainlabels);
            Console.ReadLine();
            }

        public void TestBestNetwork(List<NeuralNet> nets, MnistTrainImages images, MnistTrainLabels labels)
            {
            List<double> tmp = nets[0].Run(images.GetImage(100).ToList());
            List<double> DB = images.GetImage(100).ToList();
            Console.WriteLine("####FINAL TEST####\nInput: " + labels.GetLabel(100));
            for (int i = 0; i < 28; i++)
                {
                for (int j = 0; j < 28; j++)
                    {
                    Console.Write(DB[i*28 + j] + " ");
                    }
                Console.Write("\n");
                }
            for (int i = 0; i < 10; i++)
                {
                Console.WriteLine(i + ": " + tmp[i]/(28*28));
                }
            }

        public List<NeuralNet> Evolve(List<NeuralNet> nets, int generation, MnistTrainImages images, MnistTrainLabels labels)
            {
            nets = SortFitness(nets, generation, images, labels);
            Cutoff(nets);
            Crossover(nets, POPULATION_SIZE);
            Mutate(nets);
            return nets;
            }

        private void Cutoff(List<NeuralNet> nets)
            {
            for(int i=nets.Count()-1;i>=SURVIVORS;i--)  //Lists automatically update after a RemoveAt(); Therefore interate from the back!
                {
                if(rand.NextDouble() > UNDERDOG_SURVIVAL) //We REMOVE when NOT underdog! Don't be confused!
                    {
                    nets.RemoveAt(i);
                    }          
                }
            }

        private void Mutate(List<NeuralNet> nets)
            {
            //List<NeuralNet> ret = nets; //THIS DOES NOT CREATE A COPY!!! THIS JUST PASSES A REFERENCE; THE DESTINATION IS THE SAME!!
            for (int i = 0; i < nets.Count(); i++)
                {
                for (int mat = 0; mat < nets[i].MATRICES.Count(); mat++)
                    {
                    for (int row = 0; row < nets[i].MATRICES[mat].Count(); row++)
                        {
                        for (int col = 0; col < nets[i].MATRICES[mat][row].Count(); col++)
                            {
                            if (rand.NextDouble() < MUTATE_CHANCE)
                                {
                                nets[i].SetWeight(mat, row, col, nets[i].GetWeight(mat,row,col)+rand.NextDouble()-0.5);
                                }
                            }
                        }
                    }
                }
            }

        private void Crossover(List<NeuralNet> nets, int desiredsize)    //This will also fill the "Cutoffed" population w/ children.
            {
            int size = nets.Count();
            for (int i=0;i< desiredsize-size; i++)
                {
                NeuralNet father = nets[0]; //Performance was better when selecting the parents as the top #1 and #2. HOWEVER, this will render the previously-selected underdogs useless; as they will have 0 chance of passing down their gene. (Unless the offspring gen. completely screws up and somehow the underdog makes in into top #2 in the next evolution)
                NeuralNet mother = nets[1];
                NeuralNet offspring = new NeuralNet(NODECONFIG);
                offspring.GenerateWeights(rand); //I DON'T LIKE THIS. BUT THIS WILL DO FOR NOW...
                
                for(int mat=0;mat<offspring.MATRICES.Count();mat++)
                    {
                    for(int row=0;row<offspring.MATRICES[mat].Count();row++)
                        {
                        for(int col=0;col<offspring.MATRICES[mat][row].Count();col++)
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
                nets.Add(offspring);
                }
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

        private List<NeuralNet> SortFitness(List<NeuralNet> nets,int generation, MnistTrainImages images, MnistTrainLabels labels)
            {
            //Create list of nets to order in fitness
            for (int i = 0; i < nets.Count(); i++)
                {
#if DEBUG
Console.WriteLine("\n" + "Testing network #" + i);
#endif
                EvaluateFitness(nets[i], generation, images, labels);
                }

            //nets = nets.OrderBy(o => o.FITNESS).ToList(); //REMEBER THE SUFFERING BECAUSE OF THIS!!!

            return nets.OrderBy(o => o.FITNESS).ToList(); //You know what? a List<T> is also a class. It is also automatically passed by reference to methods.
            }

        private void EvaluateFitness(NeuralNet net, int generation, MnistTrainImages images, MnistTrainLabels labels) //Probably the 'Main' part of the class; The real 'running & testing happens here'
            {
            //DUE TO SORTING ISSUES, THIS FUNCTION WILL ASSIGN THE VALUE TO THE NETWORK'S PUBLIC VARIABLE - [FITNESS]. 
            double diff = 0;
#if DEBUG
Console.WriteLine("Dataset count: " + NUMBER_OF_DATASETS);
#endif
            List<double> outcome = net.Run(images.GetImage(generation).ToList());
            //Console.WriteLine(outcome[2]);
            for (int i = 0; i < 10; i++)
                {
                if (i == labels.GetLabel(generation))
                    {
                    //Console.WriteLine("MATCH!");
                    diff += Math.Pow(1-outcome[i]/(28*28),2);
                    }
                
                else
                    {
                    diff += Math.Pow(outcome[i]/(28*28),2);
                    }
                }
            
            //SMALLER THE BETTER!
            net.FITNESS = (diff); //Average difference
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
            Console.WriteLine("\n######POPULATION AVG. FITNESS: " + fitness/POPULATION_SIZE + " ######");
            //Console.WriteLine(fitness / POPULATION_SIZE);
            }
        }
    }
