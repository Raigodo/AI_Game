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

    public static T[] Duplicate<T>(this T[] arr){
        var duplicate = new T[arr.Length];
        for (int i=0; i<arr.Length; i++){
            duplicate[i] = arr[i];
        }
        return arr;
    }

    public static T[] DuplicateAndAdd<T>(this T[] arr, T extraItem){
        var duplicate = new T[arr.Length+1];
        for (int i=0; i<arr.Length; i++){
            duplicate[i] = arr[i];
        }
        duplicate[arr.Length] = extraItem;
        return duplicate;
    }
    public static T[] DuplicateAndRemove<T>(this T[] arr, T value)
    {
        var duplicate = new T[arr.Length-1];
        for (int i=0; i<arr.Length; i++){
            if (value.Equals(arr[i]))
                continue;
            duplicate[i] = arr[i];
        }
        return duplicate;
    }
}
