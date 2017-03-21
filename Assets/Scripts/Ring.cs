using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

public class RingNode<T>
{
    public T Value { get; set; }

    public RingNode<T> Next { get; set; }

    public RingNode<T> Previous { get; set; }

    public RingNode(T value)
    {
        Value = value;
    }

    public RingNode(T value, RingNode<T> next, RingNode<T> previous)
    {
        Value = value;
        Next = next;
        Previous = previous;
    }
}

public class Ring<T>
{
    public RingNode<T> First { get; private set; }
    public RingNode<T> Last { get; private set; }

    public RingNode<T> Iter { get; private set; }

    public int Count { get; private set; }

    public Ring()
    {
        Count = 0;
        First = null;
        Last = null;
        Iter = null;
    }

    public bool Empty()
    {
        return Count == 0;
    }

    public void Add(T value)
    {
        // unit ring
        if (Count == 0)
        {
            First = new RingNode<T>(value);
            First.Previous = First;
            First.Next = First;
            Last = First;
            Count += 1;
        }

        else if (Count == 1)
        {
            Last = new RingNode<T>(value);
            First.Next = Last;
            First.Previous = Last;
            Last.Next = First;
            Last.Previous = First;
            Count += 1;
        }

        else
        {
            var newNode = new RingNode<T>(value);
            Last.Next = newNode;
            First.Previous = newNode;
            newNode.Next = First;
            newNode.Previous = Last;
            Last = newNode;
            Count += 1;
        }
    }

    public void RemoveFirst()
    {
        if (Count == 0)
        {
            Debug.Log("Ring Warning: cannot not remove first, as it is null");
            return;
        }

        else if (Count == 1)
        {
            First = null;
            Last = null;
            Count -= 1;
        }

        else
        {
            Last.Next = First.Next;
            First.Next.Previous = Last;
            First = First.Next;
            Count -= 1;
        }
    }

    public void RemoveLast()
    {
        if (Count == 0)
        {
            Debug.Log("Ring Warning: cannot remove last, as it is null");
            return;
        }

        else if (Count == 1)
        {
            First = null;
            Last = null;
            Count -= 1;
        }

        else
        {
            Last.Previous.Next = First;
            First.Previous = Last.Previous;
            Last = Last.Previous;
            Count -= 1;
        }
    }

    public void Clear()
    {
        while (Count != 0)
        {
            RemoveFirst();
        }
    }

    public void MoveToFirst()
    {
        Iter = First;
    }

    public void MoveToLast()
    {
        Iter = Last;
    }

    public void MoveNext()
    {
        if (Iter == null)
        {
            Debug.Log("Ring warning: cannot move iterator next, as it is null");
            return;
        }

        Iter = Iter.Next;
    }

    public void MovePrev()
    {
        if (Iter == null)
        {
            Debug.Log("Ring warning: cannot move iterator prev, as it is null");
            return;
        }

        Iter = Iter.Previous;
    }

    // alias for Iter.Value
    public T GetValue()
    {
        return Iter.Value;
    }
}
