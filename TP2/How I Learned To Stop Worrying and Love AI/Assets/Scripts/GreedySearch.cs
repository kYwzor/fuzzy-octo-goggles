using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreedySearch : SearchAlgorithm {

	public float div = 1;
	private PriorityHolder priorityHolder;
	public PriorityChoice priority;
	public HeuristicChoice heuristic;

    protected override void Begin () {
		startNode = GridMap.instance.NodeFromWorldPoint (startPos);
		targetNode = GridMap.instance.NodeFromWorldPoint (targetPos);

		SearchState start = new SearchState (startNode, 0);
		if (priority == PriorityChoice.PriorityQueue)
			priorityHolder = new PriorityQueue ();
		else
			priorityHolder = new PriorityStack ();
		priorityHolder.Add (start, 0);

	}

	protected override void Step () {

		if (priorityHolder.Count > 0)
		{
			currentState = priorityHolder.PopFirst ();
			VisitNode (currentState);
			if (currentState.node == targetNode) {
				solution = currentState;
				finished = true;
				running = false;
				foundPath = true;
			} else {
				foreach (Node suc in GetNodeSucessors(currentState.node)) {
					SearchState new_node = new SearchState(suc, suc.gCost + currentState.g, GetHeuristic(suc, div, heuristic), currentState);
					priorityHolder.Add (new_node, (int)new_node.h);
				}
				// for energy
				if ((ulong) priorityHolder.Count > maxListSize) {
					maxListSize = (ulong) priorityHolder.Count;
				}
			}
		}
		else
		{
			finished = true;
			running = false;
		}

	}

    protected override string getName()
    {
		if(priority == PriorityChoice.PriorityQueue)
			return "GreedyPQ";
		return "GreedyPS";
    }
}
