﻿// See https://aka.ms/new-console-template for more information

using Kin_Alturism_Model;
using System.Linq;
using System.Net.Security;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System;
using System.Reflection.Metadata.Ecma335;

Main main = new Main();
public class Main
{
    List<Creature> population = new List<Creature>();
    List<Creature> males = new List<Creature>();
    List<Creature> females = new List<Creature>();

    Random random = new Random();

    public Main()
    {
        Console.WriteLine("Hello, World!");

        Console.WriteLine("Storm here");

        string me = "EmmaReal?";
        Console.WriteLine(me);

        Initialise();

        RunLoop();

        PrintResults();

        Console.WriteLine(":3");
    }

    /// <summary>
    /// sets up everything the program needs
    /// </summary>
    public void Initialise()
    {
        //set up all the creatures
        
        //read the initiattion Parameters from a file
    }

    /// <summary>
    /// runs the program itself
    /// </summary>
    public void RunLoop()
    {
        int phaseCount = 1000;

        while (phaseCount > 0)
        {
            //assign food
            AssignFood();

            //run the creature iteration
           foreach (Creature creature in population)
            {
                //creature phase\
                creature.RunIteration();//foodupdate

                //kills the creatures 
                if (creature.Dies())
                {
                    population.Remove(creature);
                }
            }

            //make new creatures
            foreach (Creature female in females)
            {
                if (female.fertile)
                    foreach (Creature male in males)
                        if (male.fertile)
                        {
                            Creature creature = new Creature(female, male);
                            
                            //add the creature to all the right lists
                            population.Add(creature);
                            if (creature.sex == physicalsex.female)
                                females.Add(creature);
                            else
                                males.Add(creature);

                            //creatures cant make multiple babies in the same iteration
                            female.fertile = false;
                            male.fertile = false;
                        }
            }

            foreach (Creature creature in population)
            {
                creature.RelationsUpdate();
            }

            phaseCount--;
        }

        
    }

    /// <summary>
    /// put the results og the run into a file
    /// </summary>
    public void PrintResults()
    {

    }

    public void AssignFood()
    {
        List<int> indexes = new List<int>();
        //create list of random integers(representing the indexes
        for (int i = 0; i < Parameters.numOfBundles; i++)
        {
            int randomnumber = random.Next(population.Count());
            while (indexes.Contains(randomnumber))
            {
                randomnumber = random.Next(population.Count());
            }

            indexes.Add(randomnumber);

            Creature creature = population[randomnumber];
            creature.gotfood = true;
        }
        //foreach index getfood to true
    }
}

public class Creature
{
    //parents and list of childern
    Creature ?parent1;
    Creature ?parent2;
    List<Creature> children;

    //amount of food it has and if it got food this loop
    private int food;
    public bool gotfood;

    //method to update food value without going over bounds
    int foodupdate
    {
        get { return food; }

        set { if(value > Parameters.maxfood)
            {
                food = Parameters.maxfood;
            } else { food = value; }
        }
    }

    //phenotype
    public physicalsex sex;
    private bool doubledominance;
    private int ?phenoaltruism;
    Random rand = new Random();
    public bool fertile;
    public bool disability;
    int altruism
    {
        get
        {
            gene[] genes = new gene[] { gene1, gene2 };
            if (doubledominance) return (int)genes[rand.Next(1, 3)].altruism;
            else return (int)phenoaltruism;
        }
    }

    //genotype
    gene gene1;
    gene gene2;

    //returns whether the creature starves
    public bool Dies()
    {
        if (food > 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    //removes dead direct relatives
    public void RelationsUpdate()
    {
        if (parent1 != null) if (parent1.Dies()) parent1 = null;
        if (parent2 != null) if (parent2.Dies()) parent2 = null;
        foreach(Creature child in children)
        {
            if(child.Dies()) children.Remove(child);
        }
    }

    /// <summary>
    /// is just a phase
    /// </summary>
    public void RunIteration()
    {
        food = food - Parameters.hunger;

        if (gotfood)
            Handout();

        //see if the creature can be fertile this phase
        if (food > Parameters.mateBound)
        {
            fertile = true;
        }
        else fertile = false;
    }

    //deals with obtained food
    public void Handout()
    {
        //looks at family distance 1
        List<Creature> currentdist = new List<Creature> { parent1, parent2 };
        foreach(Creature child in children) currentdist.Add(child);
        bool found = false;

        //loops 
        for(int dist = 1; dist <= altruism; dist++)
        {
            //only hands out once
            if (found) break;
            HashSet<Creature> nexthash = new HashSet<Creature>();
            foreach (Creature fam in currentdist)
            {
                //if fam considered hungry, food handed out
                int x = fam.food;
                if (x < Parameters.hungrybound)
                {
                    fam.foodupdate = x + Parameters.foodPerBundle;
                    found = true; 
                    break;
                }
                nexthash.UnionWith(fam.related);
            }
            //sets list of relatives' relatives to current parsing list
            currentdist = nexthash.ToList();

        }
        //if no hungry fam found, eat food itself
        if(!found) foodupdate = food + Parameters.foodPerBundle;
        gotfood = false;
    }

    //returns hashset of related creatures
    public HashSet<Creature> related
    {
        get
        {
            HashSet<Creature> found = new HashSet<Creature>();
            if (parent1 != null) found.Add(parent1);
            if (parent2 != null) found.Add(parent2);
            foreach(Creature child in children) found.Add(child);
            return found;
        }
    }

    //creates a new creature based on two parents
    public Creature(Creature mommy, Creature daddy)
    {
        //random number to see if mutated, mutation + (1) or - (2), mom (1) or dad (2) gene mutation, disabled, mommygene, daddygene,
        bool mutated = rand.Next(1, 101) < Parameters.mutationChance;
        bool disabled = rand.Next(1, 101) < Parameters.disabilityChance;
        int mutationDirection = rand.Next(1, 3);
        int momOrDad = rand.Next(1, 3);

        this.disability = disabled;
        this.food = 100;

        //sets parents
        this.parent1 = mommy;
        this.parent2 = daddy;
        int mommygene = rand.Next(1, 3);
        int daddygene = rand.Next(1, 3);
        //copy genes
        if (mommygene == 1)
        {
            this.gene1 = mommy.gene1;
        }
        else
        {
            this.gene1 = mommy.gene2;
        }
        if (daddygene == 1)
        {
            this.gene2 = daddy.gene1;
        }
        else
        {
            this.gene2 = daddy.gene2;
        }

        //mutate
        if (mutated)
        {
            if (gene1.type == gene2.type)
            {
                if (momOrDad == 1)
                {
                    if (mutationDirection == 1)
                    {
                        if (gene1.altruism >= Parameters.altruismHalfwayPoint && gene1.altruism + 1 >= Parameters.altruismHalfwayPoint)
                        {
                            gene1.altruism += 1;
                        }
                    }
                    else
                    {
                        if (gene1.altruism >= Parameters.altruismHalfwayPoint && gene1.altruism - 1 >= Parameters.altruismHalfwayPoint)
                        {
                            gene1.altruism += 1;
                        }
                    }
                }
                else
                {
                    if (mutationDirection == 1)
                    {
                        if (gene2.altruism >= Parameters.altruismHalfwayPoint && gene2.altruism + 1 >= Parameters.altruismHalfwayPoint)
                        {
                            gene2.altruism += 1;
                        }
                    }
                    else
                    {
                        if (gene2.altruism >= Parameters.altruismHalfwayPoint && gene2.altruism - 1 >= Parameters.altruismHalfwayPoint)
                        {
                            gene2.altruism += 1;
                        }
                    }
                }
            }
        }
        

        //decide sex and altruism phenotype
        if(gene1.type == sexgene.Y)
        {
            this.sex = physicalsex.male;
            this.doubledominance = false;
            this.phenoaltruism = gene2.altruism;
        }
        else if(gene2.type == sexgene.Y)
        {
            this.sex = physicalsex.male;
            this.doubledominance = false;
            this.phenoaltruism = gene1.altruism;
        }
        else
        {
            if (gene1.dom == gene2.dom)
            {
                this.doubledominance = true;
            }
            else
            {
                this.doubledominance = false;
                if (gene1.dom == dominance.dominant)
                {
                    this.phenoaltruism = gene1.altruism;
                }
                else this.phenoaltruism = gene2.altruism;
            }

            this.sex = physicalsex.female;
        }
        



        this.children = new List<Creature>();
        this.fertile = false;
        this.gotfood = false;
    }

    //creates a creature based on simple parameters
    public Creature(sexgene sex1, int gen1, dominance dom1, sexgene sex2, int gen2, dominance dom2)
    {
        this.food = 100;
        this.disability = false;

        this.gene1 = new gene(gen1, dom1, sex1);
        this.gene2 = new gene(gen2, dom2, sex2);
        this.children = new List<Creature>();
        this.fertile = false;
        this.gotfood = false;
    }

}
public class gene
{
    public int ?altruism;
    public dominance ?dom;
    public sexgene type;

    public gene(int alt, dominance dommy, sexgene sex)
    {
        if(sex == sexgene.X)
        {
            this.altruism = alt;
            this.dom = dommy;
        }
        this.type = sex;
    }
}
public enum dominance
{
    dominant,
    recessive
}

public enum sexgene
{
    X,
    Y
}

public enum physicalsex
{
    female,
    male
}
