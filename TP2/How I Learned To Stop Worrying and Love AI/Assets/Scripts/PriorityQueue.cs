using System.Collections.Generic;

public class PriorityQueue : PriorityHolder {
	private SortedDictionary<int, Queue<SearchState>> data;

	public PriorityQueue()
	{
		size = 0;
		data = new SortedDictionary<int, Queue<SearchState>> ();
	}

	override public void Add(SearchState s, int n)
	{
		if (data.ContainsKey (n)) {
			data [n].Enqueue (s);
		} else {
			Queue<SearchState> queue = new Queue<SearchState> ();
			queue.Enqueue (s);
			data [n] = queue;
		}

		size++;
	}

	override public SearchState PopFirst()
	{
		foreach (Queue<SearchState> queue in data.Values) {
			if (queue.Count > 0) {
				size--;
				return queue.Dequeue ();
			}
		}

		return null;
	}
}
