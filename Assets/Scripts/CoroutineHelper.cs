using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

public static class CoroutineHelper
{
    public static IEnumerator AsCoroutine(this Task task)
    {
        while (!task.IsCompleted)
        {
            yield return null;
        }

        if (task.IsFaulted)
        {
            throw task.Exception;
        }
    }

    public static IEnumerator AsCoroutine<T>(this Task<T> task, System.Action<T> onComplete)
    {
        while (!task.IsCompleted)
        {
            yield return null;
        }

        if (task.IsFaulted)
        {
            throw task.Exception;
        }
        else
        {
            onComplete(task.Result);
        }
    }
}
