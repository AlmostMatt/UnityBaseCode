using System.Collections;
using System.Collections.Generic;

public class Neighbour<T>
{
	// The square of the distance
	public float dd { get; private set; }
	public T obj { get; private set; }
	internal Neighbour(float dd, T obj)
	{
		this.dd = dd;
		this.obj = obj;
	}
}

public class Neighbours<T> : IEnumerable<Neighbour<T>>
{
	private SortedList<float, List<T>> _neighbours;
	public int Count { get {return _neighbours.Count;} }

	public Neighbours ()
	{
		_neighbours = new SortedList<float, List<T>>();
	}

	public void Clear() {
		_neighbours.Clear();
	}

	public void Add(T obj, float dd) {
		if (_neighbours.ContainsKey(dd)) {
			_neighbours[dd].Add(obj);
		} else {
			_neighbours.Add(dd, new List<T>() {obj});
		}
	}

	public IEnumerator<Neighbour<T>> GetEnumerator() {
		foreach (KeyValuePair<float, List<T>> kvp in _neighbours) {
			foreach (T obj in kvp.Value) {
				yield return new Neighbour<T>(kvp.Key, obj);
			}
		}
	}

	// this is not generic
	IEnumerator IEnumerable.GetEnumerator() {
		return this.GetEnumerator();
	}
}

