using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GeneticIndividual : Individual
{
    public int numPoints = 1;

    public GeneticIndividual(int[] topology, int n) : base(topology)
    {
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
			HashSet<int> points = new HashSet<int>();
			while(points.Count < numPoints)
            {
                points.Add(Random.Range(0, totalSize));
            }
			List<int> aux = points.ToList ();
            aux.Sort();
            bool crossing = false;
            for (int i = 0, j = 0; i < totalSize; i++)
            {
                if (j < aux.Count && aux[j] == i)
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

