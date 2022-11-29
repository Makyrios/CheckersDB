using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour
{
    float totalSeconds;
    float elapsedSeconds;
    bool started = false;
    bool running = false;
    public float Duration
    {
        set
        {
            if (value > 0)
            {
                totalSeconds = value;
            }
        }
    }

    public bool Finished
    {
        get
        {
            return (started && !running);
        }
    }

    /// <summary>
    /// Start timer
    /// </summary>
    public void Run()
    {
        if (!running)
        {
            started = true;
            running = true;
            elapsedSeconds = 0;
        }
    }

    public void Stop()
    {
        if (started && !running)
        {
            started = false;
        }
    }


    // Update is called once per frame
    void Update()
    {
        if (running)
        {
            elapsedSeconds += Time.deltaTime;
            if (elapsedSeconds >= totalSeconds)
            {
                running = false;
            }
        }
    }

}
