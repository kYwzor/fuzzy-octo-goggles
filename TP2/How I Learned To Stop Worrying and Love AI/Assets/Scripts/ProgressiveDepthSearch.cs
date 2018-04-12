using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressiveDepthSearch : SearchAlgorithm {

	private int depthLimit;
	private Stack<SearchState> openStack;


	protected override void Begin () {
		startNode = GridMap.instance.NodeFromWorldPoint (startPos);
		targetNode = GridMap.instance.NodeFromWorldPoint (targetPos);

		SearchState start = new SearchState (startNode, 0);
		openStack = new Stack<SearchState> ();
		openStack.Push(start);
		depthLimit = 0;
	}

	protected override void Step () {
		if (openStack.Count > 0)
		{
			SearchState currentState = openStack.Pop();
			VisitNode (currentState);
			if (currentState.node == targetNode) {
				solution = currentState;
				finished = true;
				running = false;
				foundPath = true;
			} 
			else {
				if (currentState.depth <= depthLimit) {
					foreach (Node suc in GetNodeSucessors(currentState.node)) {
						SearchState new_node = new SearchState (suc, suc.gCost + currentState.g, currentState);
						openStack.Push (new_node);
					}
					// for energy
					if ((ulong)openStack.Count > maxListSize) {
						maxListSize = (ulong)openStack.Count;
					}
				}
			}
		}
		else
		{
			openStack.Push(new SearchState (startNode, 0));
			depthLimit++;
		}
	}
}
