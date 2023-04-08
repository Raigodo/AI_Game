using System.Collections;
using System.Collections.Generic;

public static class Extensions
{
    public static bool Contains<Vector2> (this IEnumerable<Vector2> enumerable, Vector2 targetValue){
        foreach (var item in enumerable){
            if (item.Equals(targetValue))
                return true;
        }
        return false;
    }
}
