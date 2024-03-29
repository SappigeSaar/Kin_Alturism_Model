﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kin_Alturism_Model
{
    public class MainButPopulationOutput
    {
        Parameters parameters;
        List<XlinkedCreature> population = new List<XlinkedCreature>();
        List<SimpleCreature> simplepopulation = new List<SimpleCreature>();
        List<XlinkedCreature> males = new List<XlinkedCreature>();
        List<SimpleCreature> simplemales = new List<SimpleCreature>();
        List<XlinkedCreature> females = new List<XlinkedCreature>();
        List<SimpleCreature> simplefemales = new List<SimpleCreature>();

        Random random;
        bool xlinked;
        int halfwayPoint;
        bool firstHalf;

        public MainButPopulationOutput(int[]seeds, int[][] hundos, int testedAllele)
        {
            this.xlinked = false;

            //do it fucken like, 1000 times

            for (int i = 0; i < 1000; i++)
            {
                this.population = new List<XlinkedCreature>();
                this.simplepopulation = new List<SimpleCreature>();
                this.males = new List<XlinkedCreature>();
                this.simplemales = new List<SimpleCreature>();
                this.females = new List<XlinkedCreature>();
                this.simplefemales = new List<SimpleCreature>();

                int seed = seeds[i];
                random = new Random(seed);
                Initialise(testedAllele);
                RunLoop(hundos[i]);
            }
        }

        /// <summary>
        /// sets up everything the program needs
        /// </summary>
        public void Initialise(int allele)
        {
            dominance firstHalf;
            dominance secondHalf;
            //set up all the creatures

            //read the initiattion Parameters from a file
            StreamReader reader = new StreamReader("..\\..\\..\\initParameters.txt");

            string line = reader.ReadLine();
            this.halfwayPoint = int.Parse(line);

            line = reader.ReadLine();   
            string[] p = line.Split(",");

            this.parameters = new(halfwayPoint, p);


            line = reader.ReadLine();

            if (line == "first")
            {
                firstHalf = dominance.dominant;
                secondHalf = dominance.recessive;
                this.firstHalf = true;
            }
            else
            {
                firstHalf = dominance.recessive;
                secondHalf = dominance.dominant;
                this.firstHalf = false;
            }




            line = reader.ReadLine();
            if (xlinked)
            {
                dominance dominance;
                if (allele < halfwayPoint)
                    dominance = firstHalf;
                else
                    dominance = secondHalf;

                int total = 4;

                //make the fems
                for (int j = 0; j < (total / 2); j++)
                {
                    XlinkedCreature creature = new XlinkedCreature(sexgene.X, allele, dominance, sexgene.X, allele, dominance, parameters, random);
                    population.Add(creature);
                    females.Add(creature);
                }

                //make the mans
                for (int j = (total / 2); j < total; j++)
                {
                    XlinkedCreature creature = new XlinkedCreature(sexgene.X, allele, dominance, sexgene.Y, allele, dominance, parameters, random);
                    population.Add(creature);
                    males.Add(creature);
                }
            }
            else
            {
                dominance dominance;
                if (allele < halfwayPoint)
                    dominance = firstHalf;
                else
                    dominance = secondHalf;

                int total = 4;

                //make the fems
                for (int j = 0; j < (total / 2); j++)
                {
                    SimpleCreature creature = new SimpleCreature(physicalsex.female, allele, dominance, parameters, random);
                    simplepopulation.Add(creature);
                    simplefemales.Add(creature);
                }

                //make the mans
                for (int j = (total / 2); j < total; j++)
                {
                    SimpleCreature creature = new SimpleCreature(physicalsex.male, allele, dominance, parameters, random);
                    simplepopulation.Add(creature);
                    simplemales.Add(creature);
                }
            }
            reader.Close();
        }

        
        /// <summary>
        /// runs the program itself
        /// </summary>
        public void RunLoop(int[] hundos)
        {
            if (xlinked)
            {
                for (int i = 0; i < 10; i++)
                {
                    for (int j = 0; j < 100; j++)
                    {
                        //assign food
                        AssignFood();

                        //run the creature iteration
                        foreach (XlinkedCreature creature in population)
                        {
                            //creature phase\
                            creature.RunIteration();//foodupdate


                        }
                        //only after all food is handed out, check population. otherwise a creature might die and get handed food afterwards
                        List<XlinkedCreature> removedM = new List<XlinkedCreature>();
                        List<XlinkedCreature> removedF = new List<XlinkedCreature>();

                        foreach (XlinkedCreature creature in population)
                        {
                            //kills the creatures 
                            if (creature.Dies())
                            {
                                //also remove from list of sex
                                if (creature.sex == physicalsex.male)
                                {
                                    removedM.Add(creature);
                                }
                                else
                                {
                                    removedF.Add(creature);
                                }
                                foreach (XlinkedCreature related in creature.related.ToList()) { related.RelationsUpdate(); }
                            }
                        }
                        foreach (XlinkedCreature creature in removedM)
                        {
                            population.Remove(creature);
                            males.Remove(creature);
                        }
                        foreach (XlinkedCreature creature in removedF)
                        {
                            population.Remove(creature);
                            females.Remove(creature);
                        }


                        List<XlinkedCreature> newM = new List<XlinkedCreature>();
                        List<XlinkedCreature> newF = new List<XlinkedCreature>();

                        //make new creatures
                        foreach (XlinkedCreature female in females)
                        {
                            if (female.fertile)
                                foreach (XlinkedCreature male in males)
                                    if (male.fertile)
                                    {
                                        XlinkedCreature creature = new XlinkedCreature(female, male, parameters, random);

                                        //add the creature to all the right lists
                                        if (creature.sex == physicalsex.female)
                                            newF.Add(creature);
                                        else
                                            newM.Add(creature);
                                        male.children.Add(creature);
                                        female.children.Add(creature);

                                        //creatures cant make multiple babies in the same iteration
                                        female.fertile = false;
                                        male.fertile = false;
                                    }
                        }
                        foreach (XlinkedCreature gal in newF)
                        {
                            females.Add(gal);
                            population.Add(gal);
                        }
                        foreach (XlinkedCreature ew in newM)
                        {
                            males.Add(ew);
                            population.Add(ew);
                        }
                    }
                    hundos[i] = population.Count;
                }
            }
            else
            {
                for (int i = 0; i < 10; i++)
                {
                    for (int j = 0; j < 100; j++)
                    {
                        //assign food
                        SimpleAssignFood();

                        //run the creature iteration
                        foreach (SimpleCreature creature in simplepopulation)
                        {
                            //creature phase\
                            creature.RunIteration();//foodupdate


                        }
                        //only after all food is handed out, check population. otherwise a creature might die and get handed food afterwards
                        List<SimpleCreature> removedM = new List<SimpleCreature>();
                        List<SimpleCreature> removedF = new List<SimpleCreature>();
                        foreach (SimpleCreature creature in simplepopulation)
                        {
                            //kills the creatures 
                            if (creature.Dies())
                            {
                                //also remove from list of sex
                                if (creature.sex == physicalsex.male)
                                {
                                    removedM.Add(creature);
                                }
                                else
                                {
                                    removedF.Add(creature);
                                }
                                foreach (SimpleCreature related in creature.related.ToList()) { related.RelationsUpdate(); }
                            }
                        }
                        foreach (SimpleCreature creature in removedM)
                        {
                            simplepopulation.Remove(creature);
                            simplemales.Remove(creature);
                        }
                        foreach (SimpleCreature creature in removedF)
                        {
                            simplepopulation.Remove(creature);
                            simplefemales.Remove(creature);
                        }

                        List<SimpleCreature> newM = new List<SimpleCreature>();
                        List<SimpleCreature> newF = new List<SimpleCreature>();

                        //make new creatures
                        foreach (SimpleCreature female in simplefemales)
                        {
                            if (female.fertile)
                                foreach (SimpleCreature male in simplemales)
                                    if (male.fertile)
                                    {
                                        SimpleCreature creature = new SimpleCreature(female, male, parameters, random);

                                        //add the creature to all the right lists
                                        if (creature.sex == physicalsex.female)
                                            newF.Add(creature);
                                        else newM.Add(creature);

                                        male.children.Add(creature);
                                        female.children.Add(creature);

                                        //creatures cant make multiple babies in the same iteration
                                        female.fertile = false;
                                        male.fertile = false;
                                    }
                        }
                        foreach (SimpleCreature gal in newF)
                        {
                            simplefemales.Add(gal);
                            simplepopulation.Add(gal);
                        }
                        foreach (SimpleCreature ew in newM)
                        {
                            simplemales.Add(ew);
                            simplepopulation.Add(ew);
                        }


                        foreach (SimpleCreature creature in simplepopulation)
                        {
                            creature.RelationsUpdate();
                        }
                    }
                    hundos[i] = simplepopulation.Count;
                }
            }
        }

        public void AssignFood()
        {
            List<int> indexes = new List<int>();
            //create list of random integers(representing the indexes
            for (int i = 0; i < Math.Min(parameters.numOfBundles, population.Count()); i++)
            {
                int randomnumber = random.Next(population.Count());
                while (indexes.Contains(randomnumber))
                {
                    randomnumber = random.Next(population.Count());
                }

                indexes.Add(randomnumber);

                population[randomnumber].gotfood = true;
            }
            //foreach index getfood to true
        }

        public void SimpleAssignFood()
        {
            List<int> indexes = new List<int>();
            //create list of random integers(representing the indexes
            for (int i = 0; i < Math.Min(parameters.numOfBundles, simplepopulation.Count()); i++)
            {
                int randomnumber = random.Next(simplepopulation.Count());
                while (indexes.Contains(randomnumber))
                {
                    randomnumber = random.Next(simplepopulation.Count());
                }

                indexes.Add(randomnumber);

                simplepopulation[randomnumber].gotfood = true;
            }
            //foreach index getfood to true
        }
    }
}
