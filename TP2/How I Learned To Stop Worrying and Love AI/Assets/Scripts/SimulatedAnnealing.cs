using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SimulatedAnnealing : SearchAlgorithm {

	public double temperature;
    public double initialTemperature = 1;
	public double scalingAmount = 0.0001;
	public int seed = 0;
    private int k = 0;
	public float div = 1;
	public HeuristicChoice heuristic;


	protected override void Begin () {
		startNode = GridMap.instance.NodeFromWorldPoint (startPos);
		targetNode = GridMap.instance.NodeFromWorldPoint (targetPos);

		currentState = new SearchState (startNode, 0, GetHeuristic(startNode,div, heuristic));
        UnityEngine.Random.InitState (seed);
	}

	protected override void Step () {
        k++;
		VisitNode (currentState);
		temperature = scalingPolicy ();
		if (temperature > 0) {
            if (currentState.node == targetNode)
            {
                solution = currentState;
                finished = true;
                running = false;
                foundPath = true;
                k = 0;
            }
            else
            {
                List<Node> suc_list = GetNodeSucessors(currentState.node);
                Node aux_node = suc_list[UnityEngine.Random.Range(0, suc_list.Count)];
                SearchState new_node = new SearchState(aux_node, aux_node.gCost + currentState.g, GetHeuristic(aux_node, div, heuristic), currentState);

                if (new_node.h < currentState.h)
                    currentState = new_node;
                else if (UnityEngine.Random.value <= Mathf.Exp((float)((currentState.h - new_node.h) / temperature)))
					currentState = new_node;
            }
		}
		else
		{
			finished = true;
			running = false;
            declareDeath = true;
        }
	}

	private double scalingPolicy(){
		return initialTemperature - k * scalingAmount;
	}

	public override string getName()
    {
        return "SimulatedAnnealingT-" + initialTemperature.ToString() + "S-" + scalingAmount.ToString() ;
    }

	public override string getExtra()
    {
        return seed.ToString();
    }
}

