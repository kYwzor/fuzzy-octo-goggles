using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PriorityQueue2 {
	private int size;
	private SortedDictionary<int, Stack<SearchState>> data;

	public int Count { get { return size; } }

	public PriorityQueue2()
	{
		size = 0;
		data = new SortedDictionary<int, Stack<SearchState>> ();
	}

	public void Add(SearchState s, int n)
	{
		if (data.ContainsKey (n)) {
			data [n].Push (s);
		} else {
			Stack<SearchState> stack = new Stack<SearchState> ();
			stack.Push (s);
			data [n] = stack;
		}

		size++;
	}

	public SearchState PopFirst()
	{
		foreach (Stack<SearchState> stack in data.Values) {
			if (stack.Count > 0) {
				size--;
				return stack.Pop ();
			}
		}

		return null;
	}
}
