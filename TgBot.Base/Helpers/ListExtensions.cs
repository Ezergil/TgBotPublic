using System;
using System.Collections.Generic;

namespace TgBot.Base.Helpers
{
    public static class ListExtensions
    {
        public static IEnumerable<List<T>> Split<T>(this List<T> list, int size)  
        {        
            for (int i = 0; i < list.Count; i += size) 
            { 
                yield return list.GetRange(i, Math.Min(size, list.Count - i)); 
            }  
        } 
    }
}