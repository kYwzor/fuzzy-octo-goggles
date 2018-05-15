using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneticAlgorithm : MetaHeuristic {
	public float mutationProbability;
	public float crossoverProbability;

	public float k;
	public int tournamentSize;
	public bool elitist;

	public override void InitPopulation () {
		//You should implement the code to initialize the population here
		population = new List<Individual> ();
        // jncor 
        selection = new TournamentSelection();
        while (population.Count < populationSize) {
			GeneticIndividual new_ind= new GeneticIndividual(topology);
			new_ind.Initialize ();
			population.Add (new_ind);
		}
	}

	//The Step function assumes that the fitness values of all the individuals in the population have been calculated.
	public override void Step() {
		//You should implement the code runs in each generation here
        Debug.Log("Population:");
        foreach(var i in population)
        {
            Debug.Log(i.getGenotypeString());
        }
		List<Individual> progenitors = selection.selectIndividuals(population, tournamentSize);
        Debug.Log("Progenitors:");
        foreach(var i in progenitors)
        {
            Debug.Log(i.getGenotypeString());
        }
		List<Individual> newPop = new List<Individual>();
		for(int i = 0; i < populationSize; i+=2)	{
			Individual individual1 = progenitors[i].Clone();
			Individual individual2 = progenitors[i + 1].Clone();
            individual1.Crossover(progenitors[i + 1], crossoverProbability);
			individual2.Crossover(progenitors[i], crossoverProbability);
			individual1.Mutate(mutationProbability);
			individual2.Mutate(mutationProbability);
			newPop.Add(individual1);
			newPop.Add(individual2);
		}
        Debug.Log("New population:");
        foreach(var i in newPop)
        {
            Debug.Log(i.getGenotypeString());
        }
		population = newPop;
		generation++;
	}

}
