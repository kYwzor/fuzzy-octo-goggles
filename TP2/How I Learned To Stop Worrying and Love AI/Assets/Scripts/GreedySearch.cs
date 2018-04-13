using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreedySearch : SearchAlgorithm {

	public float div = 1;
	private PriorityQueue priorityQueue;
	public SearchAlgorithm.HeuristicChoice heuristic;

	protected override void Begin () {
		startNode = GridMap.instance.NodeFromWorldPoint (startPos);
		targetNode = GridMap.instance.NodeFromWorldPoint (targetPos);

		SearchState start = new SearchState (startNode, 0);
		priorityQueue = new PriorityQueue ();
		priorityQueue.Add (start, 0);

	}

	protected override void Step () {

		if (priorityQueue.Count > 0)
		{
			SearchState currentState = priorityQueue.PopFirst ();
			VisitNode (currentState);
			if (currentState.node == targetNode) {
				solution = currentState;
				finished = true;
				running = false;
				foundPath = true;
			} else {
				foreach (Node suc in GetNodeSucessors(currentState.node)) {
					SearchState new_node = new SearchState(suc, suc.gCost + currentState.g, GetHeuristic(suc, div, heuristic), currentState);
					priorityQueue.Add (new_node, (int)new_node.h);
				}
				// for energy
				if ((ulong) priorityQueue.Count > maxListSize) {
					maxListSize = (ulong) priorityQueue.Count;
				}
			}
		}
		else
		{
			finished = true;
			running = false;
		}

	}

    protected override string getSeed()
    {
        return "N/A";
    }

    protected override string getName()
    {
        return "GreedySearch";
    }
}
