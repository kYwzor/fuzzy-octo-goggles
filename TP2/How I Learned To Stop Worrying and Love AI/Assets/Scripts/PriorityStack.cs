using System.Collections.Generic;

public class PriorityStack : PriorityHolder {
	private SortedDictionary<int, Stack<SearchState>> data;

	public PriorityStack()
	{
		size = 0;
		data = new SortedDictionary<int, Stack<SearchState>> ();
	}

	override public void Add(SearchState s, int n)
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

	override public SearchState PopFirst()
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
