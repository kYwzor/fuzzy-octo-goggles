using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TournamentSelection : SelectionMethod{
    public override List<Individual> selectIndividuals(List<Individual> oldpop, int tournamentSize){
		List<Individual> newPop = new List<Individual>();
		for(int i = 0; i < oldpop.Count; i++){
			Individual bestCandidate = oldpop[Random.Range(0,oldpop.Count)];
			for(int j = 1; j < tournamentSize; j++){
				Individual currentCandidate = oldpop[Random.Range(0,oldpop.Count)];
				if(currentCandidate.Fitness > bestCandidate.Fitness){
					bestCandidate = currentCandidate;
				}
			}
			newPop.Add(bestCandidate);
		}
		return newPop;
    }
}
