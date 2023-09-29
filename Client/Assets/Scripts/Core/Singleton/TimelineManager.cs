using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimelineManager : Singleton<TimelineManager>
{
    public IEnumerator LoadAsyncTimeline()
    {
        yield return null;
    }

    public IEnumerator UnloadAsyncTimeline()
    {
        yield return null;
    }
}
