using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

public class AudioScaler : MonoBehaviour
{
    public float _scaleFactor = 1f;

    public float _minScale = 1f;

    public float _scaleVelocity = 1;

    public AudioSource _waveSource;

    private float[] _audioSamples;

    private int _lastSample = 0;

    private void Start()
    {
        _audioSamples = new float[_waveSource.clip.samples];
        _waveSource.clip.GetData(_audioSamples, 0);
    }

    private void Update()
    {
        int deltaSamples = _waveSource.timeSamples - _lastSample;
        _lastSample = _waveSource.timeSamples;

        int currentIndex = _waveSource.timeSamples % _audioSamples.Length;
        int nextIndex = ((_waveSource.timeSamples + deltaSamples) % _audioSamples.Length);

        float currentSample = _audioSamples[currentIndex];
        float nextSample = _audioSamples[nextIndex];

        float diff = nextSample - currentSample;

        float nextScale = Mathf.Lerp(transform.localScale.x, transform.localScale.x + (diff * _scaleFactor), Time.deltaTime * _scaleVelocity);

        transform.localScale = new Vector3(nextScale, transform.localScale.y, nextScale);
    }
}