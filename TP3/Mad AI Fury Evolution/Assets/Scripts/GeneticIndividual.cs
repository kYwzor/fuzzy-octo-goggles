using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneticIndividual : Individual
{
    public int numPoints = 1;
    public List<int> points;

    public GeneticIndividual(int[] topology, int n) : base(topology)
    {
        points = new List<int>();
        numPoints = n;
    }

    public override void Initialize()
    {
        for (int i = 0; i < totalSize; i++)
        {
            genotype[i] = Random.Range(-1.0f, 1.0f);
        }
    }

    public override void Crossover(Individual partner, float probability)
    {
        //Debug.Log("Trying for crossover\n");
        if (Random.Range(0.0f, 1.0f) < probability)
        {
            /*
            string aux = "";
            foreach(var i in genotype)
            {
                aux += i.ToString();
            }
			Debug.Log("Before crossover: " + getGenotypeString());
            Debug.Log("Crossing with: " + ((GeneticIndividual)partner).getGenotypeString());
            */
            for (int i = 0; i < numPoints; i++)
            {
                points.Add(Random.Range(0, totalSize));
            }
            points.Sort();
            /*
            for (int i = 0; i < numPoints; i++)
            {
                Debug.Log("At point " + points[i]);
            }
            */
            bool crossing = false;
            for (int i = 0, j = 0; i < totalSize; i++)
            {
                if (j < points.Count && points[j] == i)
                {
                    crossing = !crossing;
                    j++;
                }
                if (crossing)
                    genotype[i] = ((GeneticIndividual)partner).genotype[i];
            }
            //Debug.Log("After crossover: " + getGenotypeString());
        }
    }

    public override void Mutate(float probability)
    {
        for (int i = 0; i < totalSize; i++)
        {
            if (Random.Range(0.0f, 1.0f) < probability)
            {
                genotype[i] = Random.Range(-1.0f, 1.0f);
            }
        }
    }

    public override Individual Clone()
    {
        GeneticIndividual new_ind = new GeneticIndividual(this.topology, this.numPoints);

        genotype.CopyTo(new_ind.genotype, 0);

        new_ind.fitness = this.Fitness;
        new_ind.evaluated = false;

        return new_ind;
    }



}

