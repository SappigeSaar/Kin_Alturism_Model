// See https://aka.ms/new-console-template for more information

using Kin_Alturism_Model;
using System.Linq;
using System.Net.Security;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Diagnostics;


for(int allele = 0; allele < 11; allele++)
{
    Console.WriteLine("Started work on allele " +  allele.ToString());
    //1000 times population at 10 points of 1 allele
    int[][] hundos = new int[1000][];
    int[] seeds = new int[1000];

    Random seedgenerator = new Random();
    for (int i = 0; i < 1000; i++)
    {
        seeds[i] = seedgenerator.Next();
        hundos[i] = new int[10];
    }

    MainButPopulationOutput main = new MainButPopulationOutput(seeds, hundos, allele);


    string path = "..\\..\\..\\output\\populationsForAllele" + allele.ToString() + ".txt";
    Stream file = File.Open(path, FileMode.OpenOrCreate);
    StreamWriter writer = new StreamWriter(file);

    writer.WriteLine("Population for allele" + allele.ToString() + " with seed _ at time increment 0, 100, 200, ..., 1000");
    writer.Write("\n");
    for (int i = 0; i < 1000; i++)
    {
        //seed at start of line
        writer.Write("Seed " + seeds[i]);
        //write population at start
        writer.Write(", 4");
        //write population for each increment
        for (int j = 0; j < 10; j++)
        {
            writer.Write(", " + hundos[i][j].ToString());
        }
        //start new line
        writer.Write('\n');
    }

    writer.Close();

    Console.WriteLine("I finished allele " + allele.ToString() + ">w<");
}


public class Main
{
    Parameters parameters;
    List<Creature> population = new List<Creature>();
    List<SimpleCreature> simplepopulation = new List<SimpleCreature>();
    List<Creature> males = new List<Creature>();
    List<SimpleCreature> simplemales = new List<SimpleCreature>();
    List<Creature> females = new List<Creature>();
    List<SimpleCreature> simplefemales = new List<SimpleCreature>();

    Random random;
    bool xlinked;
    int halfwayPoint;
    bool firstHalf;

    public Main()
    {
        xlinked = true;

        Stopwatch stopwatch = new();
        stopwatch.Start();

        Console.WriteLine("Hello, World!");

        Console.WriteLine("Storm here");

        string me = "EmmaReal?";
        Console.WriteLine(me);

        //set up seed and timestamp for output file
        DateTime now = DateTime.Now;
        string timestamp = now.ToString().Replace('/', '-').Replace(' ', '_').Replace(':', '-');
        int timeseed = 10000 * now.Hour + 100 * now.Minute + now.Second;

        //set up random with seed, set up file 
        random = new Random(timeseed);
        string path = "..\\..\\..\\output\\outputAt-" + timestamp + ".txt";
        Stream file = File.Open(path, FileMode.OpenOrCreate);
        StreamWriter writer = new StreamWriter(file);
        

        Initialise();

        PrintSetup(writer, timeseed);

        RunLoop(writer);

        stopwatch.Stop();
        Console.WriteLine("done :3");
        Console.WriteLine("Time spent: " + stopwatch.ElapsedMilliseconds + "ms");
    }
    public Main(int seed)
    {
        xlinked = true;

        Stopwatch stopwatch = new();
        stopwatch.Start();

        Console.WriteLine("Hello, World!");

        Console.WriteLine("Storm here");

        string me = "EmmaReal?";
        Console.WriteLine(me);

        //set up seed and timestamp for output file

        //set up random with seed, set up file 
        random = new Random(seed);
        string path = "..\\..\\..\\output\\outputWSeed-" + seed.ToString() + ".txt";
        Stream file = File.Open(path, FileMode.OpenOrCreate);
        StreamWriter writer = new StreamWriter(file);   


        Initialise();

        PrintSetup(writer, seed);

        RunLoop(writer);

        stopwatch.Stop();
        Console.WriteLine("done :3");
        Console.WriteLine("Time spent: " + stopwatch.ElapsedMilliseconds + "ms");
    }

    /// <summary>
    /// sets up everything the program needs
    /// </summary>
    public void Initialise()
    {
        dominance firstHalf;
        dominance secondHalf;
        //set up all the creatures

        //read the initiattion Parameters from a file
        StreamReader reader = new StreamReader("..\\..\\..\\initParameters.txt");

        string line = reader.ReadLine();
        if (line == "none")
        {
            xlinked = false;
        }
        else
        {
            this.halfwayPoint = int.Parse(line);
        }

        line = reader.ReadLine();
        string[] p = line.Split(", ");

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
        string[] distribution = line.Split(',');
        if (xlinked)
        {
            for (int altruism = 0; altruism < distribution.Length; altruism++)
            {
                dominance dominance;
                if (altruism < halfwayPoint)
                    dominance = firstHalf;
                else
                    dominance = secondHalf;

                int total = int.Parse(distribution[altruism]);

                //make the fems
                for (int j = 0; j < (total / 2); j++)
                {
                    Creature creature = new Creature(sexgene.X, altruism, dominance, sexgene.X, altruism, dominance, parameters, random);
                    population.Add(creature);
                    females.Add(creature);
                }

                //make the mans
                for (int j = (total / 2); j < total; j++)
                {
                    Creature creature = new Creature(sexgene.X, altruism, dominance, sexgene.Y, altruism, dominance, parameters, random);
                    population.Add(creature);
                    males.Add(creature);
                }
            }
        }
        else
        {
            for (int altruism = 0; altruism < distribution.Length; altruism++)
            {
                int total = int.Parse(distribution[altruism]);
                dominance dom;
                if (altruism < halfwayPoint) dom = firstHalf; else dom = secondHalf;

                //make the fems
                for (int j = 0; j < (total / 2); j++)
                {
                    SimpleCreature creature = new SimpleCreature(physicalsex.female, altruism, dom, parameters, random);
                    simplepopulation.Add(creature);
                    simplefemales.Add(creature);
                }

                //make the mans
                for (int j = (total / 2); j < total; j++)
                {
                    SimpleCreature creature = new SimpleCreature(physicalsex.male, altruism, dom, parameters, random);
                    simplepopulation.Add(creature);
                    simplemales.Add(creature);
                }
            }
        }
        reader.Close();

        Console.WriteLine("initialise complete");
    }


    public void PrintSetup(StreamWriter streamWriter, int seed)
    {
        string first;
        if (firstHalf)
        {
            first = "dominant";
        }
        else
        {
            first = "recessive";
        }
        //writes the setup requirements
        streamWriter.Write("Seed: " + seed.ToString() + "\n");
        streamWriter.Write("Parameters: ");
        streamWriter.Write("number of bundles = " + parameters.numOfBundles);
        streamWriter.Write(", food per bundle = " + parameters.foodPerBundle);
        streamWriter.Write(", max food bar = " + parameters.maxfood);
        streamWriter.Write(", hungry bound = " + parameters.hungrybound);
        streamWriter.Write(", hunger = " + parameters.hunger);
        streamWriter.Write(", mating food lower bound = " + parameters.mateBound);
        streamWriter.Write(", mutation chance = " + parameters.mutationChance);
        streamWriter.Write(", disability chacne = " + parameters.disabilityChance);
        streamWriter.Write(", first half = " + first);
        streamWriter.Write(", halfway point = " + halfwayPoint);
        streamWriter.Write(", selfish bonus = " + parameters.selfishBonus);
        streamWriter.Write(", gain from bonus = " + parameters.bonusGain + "\n");
    }
    /// <summary>
    /// runs the program itself
    /// </summary>
    public void RunLoop(StreamWriter file)
    {
        int phaseCount = 1000;

        if (xlinked)
        {
            while (phaseCount > 0)
            {
                //assign food
                AssignFood();

                //run the creature iteration
                foreach (Creature creature in population)
                {
                    //creature phase\
                    creature.RunIteration();//foodupdate


                }
                //only after all food is handed out, check population. otherwise a creature might die and get handed food afterwards
                List<Creature> removedM = new List<Creature>();
                List<Creature> removedF = new List<Creature>();

                foreach (Creature creature in population)
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
                        foreach(Creature related in creature.related.ToList()) { related.RelationsUpdate(); }
                    }
                }
                foreach (Creature creature in removedM)
                {
                    population.Remove(creature);
                    males.Remove(creature);
                }
                foreach (Creature creature in removedF)
                {
                    population.Remove(creature);
                    females.Remove(creature);
                }


                List<Creature> newM = new List<Creature>();
                List<Creature> newF = new List<Creature>();

                //make new creatures
                foreach (Creature female in females)
                {
                    if (female.fertile)
                        foreach (Creature male in males)
                            if (male.fertile)
                            {
                                Creature creature = new Creature(female, male, parameters, random);

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
                foreach (Creature gal in newF)
                {
                    females.Add(gal);
                    population.Add(gal);
                }
                foreach (Creature ew in newM)
                {
                    males.Add(ew);
                    population.Add(ew);
                }


                phaseCount--;
                //print state to file youre working on
                PrintResults(file);
            }
        }
        else 
        {
            while (phaseCount > 0)
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

                phaseCount--;
                //print state to file youre working on
                PrintResults(file);
            }
        }
    }

    /// <summary>
    /// put the results og the run into a file
    /// </summary>
    public void PrintResults(StreamWriter writer)
    {
        if (xlinked)
        {
            foreach (Creature creature in population)
            {
                writer.Write(creature.ToString());
                writer.Write(", ");
            }
        }
        else
        {
            foreach (SimpleCreature creature in simplepopulation)
            {
                writer.Write(creature.ToString());
                writer.Write(", ");
            }
        }
        writer.Write('\n');
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



public class gene
{
    public int altruism;
    public dominance dom;
    public sexgene type;

    public override string ToString()
    {
        string output;
        if (type == sexgene.X)
        {
            output = "X " + dom.ToString() + " " + altruism.ToString();
        }
        else
        {
            output = "Y null null";
        }
        return output;
    }

    public gene(int alt, dominance dommy, sexgene sex)
    {
        if(sex == sexgene.X)
        {
            this.altruism = alt;
            this.dom = dommy;
        }
        else
        {
            this.altruism = 69;
            this.dom = dominance.dominant;
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
