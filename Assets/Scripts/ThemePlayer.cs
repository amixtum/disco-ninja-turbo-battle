using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

public class ThemePlayer : MonoBehaviour
{
    private bool _started = false;

    private void Start()
    {
        if (!_started)
        {
            _started = true;
            DontDestroyOnLoad(gameObject);
            Play();
        }
    }

    public void Stop()
    {
        GetComponent<AudioSource>().Stop();
    }

    public void Play()
    {
        GetComponent<AudioSource>().Play();
    }
}