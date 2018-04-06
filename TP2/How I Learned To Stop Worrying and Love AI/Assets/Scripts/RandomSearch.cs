using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSearch : SearchAlgorithm {

	private 	List<SearchState> openList;


	protected override void Begin () {
		startNode = GridMap.instance.NodeFromWorldPoint (startPos);
		targetNode = GridMap.instance.NodeFromWorldPoint (targetPos);

		SearchState start = new SearchState (startNode, 0);
		openList = new List<SearchState> ();
		openList.Add(start);

	}

	protected override void Step () {
		if (openList.Count > 0)
		{
			SearchState currentState = openList[Random.Range (0, openList.Count - 1)];
			openList.Remove (currentState);
			VisitNode (currentState);
			if (currentState.node == targetNode) {
				solution = currentState;
				finished = true;
				running = false;
				foundPath = true;
			} 
			else {
				foreach (Node suc in GetNodeSucessors(currentState.node)) {
					SearchState new_node = new SearchState(suc, suc.gCost + currentState.g, currentState);
					openList.Add(new_node);
				}
				// for energy
				if ((ulong) openList.Count > maxListSize) {
					maxListSize = (ulong) openList.Count;
				}
			}
		}
		else
		{
			finished = true;
			running = false;
		}
	}
}
