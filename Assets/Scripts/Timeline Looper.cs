using UnityEngine;
using UnityEngine.Playables;

public class LoopTimelineSection : MonoBehaviour
{
    public PlayableDirector director;
    public double loopStart = 0;
    public double loopEnd = 3; // time to loop to

    void Update()
    {
        if (director.time >= loopEnd)
            director.time = loopStart;
    }
}
