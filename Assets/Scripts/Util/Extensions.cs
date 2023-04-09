using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public static class Extensions
{
    public static bool Contains<Vector2Int> (this IEnumerable<Vector2Int> enumerable, Vector2Int targetValue){
        foreach (var item in enumerable){
            if (item.Equals(targetValue))
                return true;
        }
        return false;
    }
    
    public static string AsString<Vector2Int>(this IEnumerable<Vector2Int> enumerable) {
        var strinBuilder = new StringBuilder();
        strinBuilder.Append("( ");
        foreach (var item in enumerable) {
            strinBuilder.Append($"{item} ");
        }
        strinBuilder.Append(")");
        return strinBuilder.ToString();
    }
}
