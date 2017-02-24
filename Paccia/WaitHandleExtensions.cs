using System;
using System.Threading;
using System.Threading.Tasks;

namespace Paccia
{
    public static class WaitHandleExtensions
    {
        public static Task WaitOneAsync(this WaitHandle waitHandle) => WaitOneAsync(waitHandle, Timeout.InfiniteTimeSpan);

        public static async Task WaitOneAsync(this WaitHandle waitHandle, TimeSpan timeout)
        {
            var taskCompletionSource = new TaskCompletionSource<bool>();

            var registeredWaitHandle = ThreadPool.RegisterWaitForSingleObject
                                                  (
                                                      waitHandle,
                                                      (_, timedOut) => _ = timedOut ? 
                                                                           taskCompletionSource.TrySetCanceled() : 
                                                                           taskCompletionSource.TrySetResult(true),
                                                      null,
                                                      timeout, 
                                                      true
                                                  );

            await taskCompletionSource.Task;

            registeredWaitHandle.Unregister(null);
        }
    }
}