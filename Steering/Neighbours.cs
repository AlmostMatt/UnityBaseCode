using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public interface ObjectWithPosition {
	Vector2 getPosition();
}
 	
public class Neighbour<T> : IComparable<Neighbour<T>> where T : ObjectWithPosition
{
	// The square of the distance
	public float dd { get; set; }
	public T obj { get; private set; }
	internal Neighbour(float dd, T obj)
	{
		this.dd = dd;
		this.obj = obj;
	}

	public int CompareTo(Neighbour<T> other) {
		return dd.CompareTo(other.dd);
	}
}

/**
 * Maintains a list of objects ordered by how far away from the current object that they are.
 * 
 * T1 is the type of the current object and T2 is the type of the neighbour objects.
 */
public class Neighbours<T1, T2> : IEnumerable<Neighbour<T2>> where T1 : ObjectWithPosition where T2 : ObjectWithPosition
{
	/**
	 * SortedList is low-overhead but inefficient add and remove (unless adding at the end)
	 * SortedDictionary has more overhead but has many log n operations.
	 * SortedList/Dict have no easy way to update (basically remove all and then add all)
	 * List has methods for Sort() and BinarySearch and can mutate items.
	 * 
	 * TODO: try using List and get some sense of how many 'swaps' are necessary during update.
	 * TODO: compare efficiencies of different implementations.
	 * Note: SortedList/Dict also require me to handle items of identical distance and to convert from KeyValuePair to Neighbour.
	 */
	// SortedDictionary is more effecient than SortedList for unsorted addition and removal operations.
	// Note that it's possible for multiple neighbours to have the same distance.
	private T1 currentObject;
	private List<Neighbour<T2>> _neighbours;
	public int Count { get {return _neighbours.Count;} }

	public Neighbours (T1 currentObject)
	{
		this.currentObject = currentObject;
		_neighbours = new List<Neighbour<T2>>();
	}

	public void Clear() {
		_neighbours.Clear();
	}

	/**
	 * Recomputes the distances between objects and reorders them.
	 * 
	 * This updates all values and then sorts the list.
	 *  O(n log n)
	 */
	// TODO: use clock-time to guarantee update is not called too often, and call it during GetEnumerator
	// TODO: add a feature to ignore every 2nd update (to save time)
	// TODO: automatically remove 'dead' objects (gameObject == null).
	// TODO: try swapping neighbour with prev neighbours as they are updated
	// TODO: use a custom-sort algorithm that is efficient for mostly-sorted data
	public void Update() {
		Vector2 currentObjPos = currentObject.getPosition();
		foreach (Neighbour<T2> neighbour in _neighbours) {
			neighbour.dd = (neighbour.obj.getPosition() - currentObjPos).sqrMagnitude;
		}
		_neighbours.Sort();
	}

	/**
	 * Adds an item to the end of the list. It will be sorted in the next call to Update().
	 *  O(1)
	 */
	// TODO: consider updating the order immediately, and using AddRange if multiple objects are added simultaneously
	public void Add(T2 obj) {
		_neighbours.Add(new Neighbour<T2>((obj.getPosition() - currentObject.getPosition()).sqrMagnitude, obj));
	}

	/**
	 * Adds all objects in a collection, and then sorts the list.
	 * 
	 *  O(n log n) because of the sort.
	 */
	public void AddRange(IEnumerable<T2> objects) {
		foreach(T2 obj in objects) {
			// Distance will be recomputed during update
			_neighbours.Add(new Neighbour<T2>(0f, obj));
		}
		Update();
	}

	/**
	 * Linear search to find  an item and then shift the remaining elements
	 *  O(n)
	 */
	public void Remove(T2 objToRemove) {
		// Since the distance might be slightly out of date, lookup index by key
		// And then check the nearby indices for this object, and call RemoveAt(index)
		// Note: the remaining elements are shifted, so this will be O(n) regardless ...
		// TODO: consider using SortedDictionary
		for (int i=0; i<_neighbours.Count; i++) {
			if (_neighbours[i].obj.Equals(objToRemove)) {
				_neighbours.RemoveAt(i);
				break;
			}
		}
	}

	// This is Generic
	public IEnumerator<Neighbour<T2>> GetEnumerator() {
		return _neighbours.GetEnumerator();
	}

	// This is not Generic
	IEnumerator IEnumerable.GetEnumerator() {
		return _neighbours.GetEnumerator();
	}
}

