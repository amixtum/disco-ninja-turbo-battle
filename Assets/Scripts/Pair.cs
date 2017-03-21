using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// self explanatory
[System.Serializable]
public class Pair<T>
{
    public T Left { get; set; }
    public T Right { get; set; }

    public Pair(T left, T right)
    {
        Left = left;
        Right = right;
    }

    public bool BothSatisfy(Predicate<T> pred)
    {
        return pred(Left) && pred(Right);
    }

    public bool EitherSatisfy(Predicate<T> pred)
    {
        return pred(Left) || pred(Right);
    }
}