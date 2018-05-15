using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TournamentSelection : SelectionMethod{
	public float k = 1;
    public override List<Individual> selectIndividuals(List<Individual> oldpop, int tournamentSize){
		float[] probs = new float[tournamentSize];
		List<Individual> newPop = new List<Individual>();
		for(int i = 0; i < tournamentSize; i++){
			probs[i] = k * 	Mathf.Pow((1 - k), i);
			if(i != 0) probs[i] += probs[i - 1];
			Debug.Log("Prob " + i + " " + probs[i]);
		}
		for(int i = 0; i < oldpop.Count; i++){
			List<Individual> candidates = new List<Individual>();
			for(int j = 0; j < tournamentSize; j++){
				candidates.Add(oldpop[Random.Range(0,oldpop.Count)]);
			}
			candidates.Sort(delegate(Individual x, Individual y){
				if(x.Fitness > y.Fitness){
					return -1;
				}
				else if(y.Fitness > x.Fitness){
					return 1;
				}
				else{
					return 0;
				}
			});
			int h = 0;
			float r = Random.value;
			while(r > probs[h] && h < tournamentSize - 1){
				h++;
			}
			newPop.Add(candidates[h]);
		}
		return newPop;
    }
}
