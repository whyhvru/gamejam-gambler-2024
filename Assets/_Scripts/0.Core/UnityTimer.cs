using System;
using System.Collections;
using UnityEngine;

namespace Module.Core
{
    public sealed class UnityTimer : MonoBehaviour, ITimer
    {
        public void Schedule(float delaySeconds, Action action)
        {
            if (action == null) return;
            StartCoroutine(Run(delaySeconds, action));
        }

        private static IEnumerator Run(float delaySeconds, Action action)
        {
            if (delaySeconds > 0f)
                yield return new WaitForSeconds(delaySeconds);

            action.Invoke();
        }
    }
}
