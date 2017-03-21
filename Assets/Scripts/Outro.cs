using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

public class Outro : MonoBehaviour
{
    private AudioSource _source;

    private void Awake()
    {
        _source = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (!_source.isPlaying)
        {
            Application.Quit();
        }
    }
}