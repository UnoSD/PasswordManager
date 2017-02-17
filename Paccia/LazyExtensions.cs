using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Paccia
{
    static class LazyExtensions
    {
        public static TaskAwaiter<T> GetAwaiter<T>(this Lazy<Task<T>> lazy) => lazy.Value.GetAwaiter();
    }
}