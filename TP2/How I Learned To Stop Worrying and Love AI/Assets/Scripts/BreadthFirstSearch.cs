using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreadthFirstSearch : SearchAlgorithm {

	private Queue<SearchState> openQueue;


	protected override void Begin () {
		startNode = GridMap.instance.NodeFromWorldPoint (startPos);
		targetNode = GridMap.instance.NodeFromWorldPoint (targetPos);

		SearchState start = new SearchState (startNode, 0);
		openQueue = new Queue<SearchState> ();
		openQueue.Enqueue(start);
		
	}
	
	protected override void Step () {
		
		if (openQueue.Count > 0)
		{
			SearchState currentState = openQueue.Dequeue();
			VisitNode (currentState);
			if (currentState.node == targetNode) {
				solution = currentState;
				finished = true;
				running = false;
				foundPath = true;
			} else {
				foreach (Node suc in GetNodeSucessors(currentState.node)) {
					SearchState new_node = new SearchState(suc, suc.gCost + currentState.g, currentState);
					openQueue.Enqueue (new_node);
				}
				// for energy
				if ((ulong) openQueue.Count > maxListSize) {
					maxListSize = (ulong) openQueue.Count;
				}
			}
		}
		else
		{
			finished = true;
			running = false;
			foundPath = true;
		}

	}
}
