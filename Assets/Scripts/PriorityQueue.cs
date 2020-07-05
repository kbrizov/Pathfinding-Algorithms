using System;
using System.Collections.Generic;
using Wintellect.PowerCollections;

/// <summary>
/// Stable priority queue.
/// </summary>
public class PriorityQueue<T> where T : IComparable<T>
{
    private OrderedBag<T> m_multiset;

    public PriorityQueue()
    {
        m_multiset = new OrderedBag<T>();
    }

    public PriorityQueue(Comparison<T> comparison)
        : this(Comparer<T>.Create(comparison))
    {
    }

    public PriorityQueue(IComparer<T> comparer)
    {
        m_multiset = new OrderedBag<T>(comparer);
    }

    public PriorityQueue(IEnumerable<T> collection)
    {
        m_multiset = new OrderedBag<T>(collection);
    }

    public PriorityQueue(IEnumerable<T> collection, Comparison<T> comparison)
        : this(collection, Comparer<T>.Create(comparison))
    {
    }

    public PriorityQueue(IEnumerable<T> collection, IComparer<T> comparer)
    {
        m_multiset = new OrderedBag<T>(collection, comparer);
    }

    public int Count
    {
        get
        {
            return m_multiset.Count;
        }
    }

    public void Enqueue(T element)
    {
        m_multiset.Add(element);
    }

    public T Dequeue()
    {
        T element = m_multiset.RemoveFirst();

        return element;
    }

    public override string ToString()
    {
        return m_multiset.ToString();
    }
}