using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public static class Extensions
{
    public static bool Contains<Vector2> (this IEnumerable<Vector2> enumerable, Vector2 targetValue){
        foreach (var item in enumerable){
            if (item.Equals(targetValue))
                return true;
        }
        return false;
    }
    
    public static string AsString<Vector2>(this IEnumerable<Vector2> enumerable) {
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
    public static T[] DuplicateAndRemove<T>(this T[] arr, T value){
        var duplicate = new T[arr.Length-1];
        for (int arrIndex=0, duplicateIndex=0; arrIndex<arr.Length; arrIndex++, duplicateIndex++){
            if (value.Equals(arr[arrIndex])){
                duplicateIndex--;
                continue;
            }
            duplicate[duplicateIndex] = arr[arrIndex];
        }
        return duplicate;
    }

    public static int GetDifferenceXYSum(this Vector2 origin, Vector2 endPos){
        int diffX = Mathf.Abs((int)origin.x - (int)endPos.x);
        int diffY = Mathf.Abs((int)origin.y - (int)endPos.y);
        return diffX + diffY;
    }

    public static void Print<Vector2>(this IEnumerable<Vector2> arr, string prefix = "", string postfix = "") {
        var strinBuilder = new StringBuilder();
        strinBuilder.Append("( ");
        foreach (Vector2 item in arr) {
            strinBuilder.Append($"{(Vector2)item} ");
        }
        strinBuilder.Append(")");
        Debug.Log(prefix + strinBuilder.ToString() + postfix);
    }
}
