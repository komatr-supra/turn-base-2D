using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Common
{
    public static T RandomElement<T>(this IList<T> collection)
    {
        return collection[Random.Range(0, collection.Count)];
    }
}
