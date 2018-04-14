public abstract class PriorityHolder {
	protected int size;
	public int Count { get { return size; } }

	abstract public void Add (SearchState s, int n);
	abstract public SearchState PopFirst ();
}