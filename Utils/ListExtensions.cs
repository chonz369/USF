using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;

namespace USF.Utils
{
    public static class ListExtensions
    {
        public static async UniTask ForEachAsync<T>(this List<T> list, Func<T, UniTask> func) {
            foreach (var value in list) {
                await func(value);
            }
        }
    }
}