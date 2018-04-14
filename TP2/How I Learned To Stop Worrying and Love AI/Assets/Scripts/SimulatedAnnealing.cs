using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SimulatedAnnealing : SearchAlgorithm {

	SearchState currentState;
	public double temperature;
	public int seed = 0;
    private int k = 0;
	public float div = 1;
	public HeuristicChoice heuristic;

	// Use this for initialization
	protected override void Begin () {
		startNode = GridMap.instance.NodeFromWorldPoint (startPos);
		targetNode = GridMap.instance.NodeFromWorldPoint (targetPos);
		currentState = new SearchState (startNode, 0, GetHeuristic(startNode,div, heuristic));
        UnityEngine.Random.InitState (seed);
		//Debug.Log (targetNode.gridX + " " + targetNode.gridY);
	}
	
	// Update is called once per frame
	protected override void Step () {
        k++;
		VisitNode (currentState);
		temperature = scalingPolicy (k);
		if (temperature > 0) {
            if (currentState.node == targetNode)
            {
                solution = currentState;
                finished = true;
                running = false;
                foundPath = true;
            }
            else
            {
                List<Node> suc_list = GetNodeSucessors(currentState.node);
                //Debug.Log ("H: " + GetHeuristic (currentState.node,divisor) + " " + currentState.node.gridX + " " + currentState.node.gridY);
                Node aux_node = suc_list[UnityEngine.Random.Range(0, suc_list.Count)];
                //Debug.Log ("H: " + GetHeuristic (aux_node,divisor) + " " + aux_node.gridX + " " + aux_node.gridY);
                // for energy
                SearchState new_node = new SearchState(aux_node, aux_node.gCost + currentState.g, GetHeuristic(aux_node, div, heuristic), currentState);
                if (new_node.h < currentState.h)
                {
                    currentState = new_node;
                    //Debug.Log ("Accepted - H");
                }
                else
                {
                    /*
					Debug.Log ("Diff " + (currentState.h - new_node.h));
					Debug.Log ("Temp " + temperature);
					Debug.Log((currentState.h - new_node.h) / temperature);
					Debug.Log (Mathf.Exp ((float)((currentState.h - new_node.h) / temperature)));
					*/
                    if (UnityEngine.Random.value <= Mathf.Exp((float)((currentState.h - new_node.h) / temperature)))
                    {
                        //Debug.Log ("Accepted - T " + Mathf.Exp ((float)((currentState.h - new_node.h) / temperature)));
                        currentState = new_node;
                    }
                    else
                    {
                        //Debug.Log ("Rejected - T " + Mathf.Exp ((float)((currentState.h - new_node.h) / temperature)));

                    }
                }
                //Debug.Log ("-----");

            }
		}
		else
		{
			finished = true;
			running = false;
		}
	}

	private double scalingPolicy(int k){
		return 1 + k * 0.0001;
	}

    protected override string getName()
    {
        return "SimulatedAnnealing";
    }

    protected override string getExtra()
    {
        return seed.ToString();
    }
}

