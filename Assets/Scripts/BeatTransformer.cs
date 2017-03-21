using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

public class BeatTransformer : MonoBehaviour
{
    public Color _attackPlatformColor = Color.white;

    private int _beat = 0;

    private Metronome _metronome;

    private Metronome _fastMetronome;

    private Ring<Pair<GameObject>> _scaleSequence = new Ring<Pair<GameObject>>();

    private List<GameObject> _platforms = new List<GameObject>();

    private List<GameObject> _attackablePlatforms = new List<GameObject>();

    private const string _platformTag = "Platform";

    private void Start()
    {
        // get the metronomes
        _metronome = GetComponent<GameController>().MainMetronome;
        _fastMetronome = GetComponent<GameController>().FastMetronome;

        // add the platforms to the list
        foreach (var platform in GameObject.FindGameObjectsWithTag(_platformTag))
        {
            _platforms.Add(platform);
        }

        _scaleSequence.Add(new Pair<GameObject>(GameObject.Find("R1"), GameObject.Find("L4")));
        _scaleSequence.Add(new Pair<GameObject>(GameObject.Find("R2"), GameObject.Find("L3")));
        _scaleSequence.Add(new Pair<GameObject>(GameObject.Find("R3"), GameObject.Find("L2")));
        _scaleSequence.Add(new Pair<GameObject>(GameObject.Find("R4"), GameObject.Find("L1")));
        _scaleSequence.Add(new Pair<GameObject>(GameObject.Find("R3"), GameObject.Find("L2")));
        _scaleSequence.Add(new Pair<GameObject>(GameObject.Find("R2"), GameObject.Find("L3")));

        _scaleSequence.MoveToFirst();
    }

    private void Update()
    {
        if (_metronome.Started && !_metronome.Paused)
        {
            // modulate colors of platforms
            foreach (var platform in _platforms)
            {
                if (!_attackablePlatforms.Contains(platform))
                {
                    platform.GetComponent<MeshRenderer>().material.color = new Color(GetBeatFraction(), 0f, 1f);
                }
                else
                {
                    platform.GetComponent<MeshRenderer>().material.color = _attackPlatformColor;
                }
            }

            if (_beat < _fastMetronome.BeatsElapsed)
            {
                var platformPair = _scaleSequence.GetValue();
                platformPair.Left.GetComponent<Scaler>().StartScaling();
                platformPair.Right.GetComponent<Scaler>().StartScaling();
                _scaleSequence.MoveNext();
            }

            _beat = _fastMetronome.BeatsElapsed;
        }
    }

    public void SetAttackable(Platform platform)
    {
        if (!_attackablePlatforms.Contains(Utils.GetPlatformObject(platform)))
        {
            _attackablePlatforms.Add(Utils.GetPlatformObject(platform));
        }
    }

    public void SetAttackable(GameObject platform)
    {
        if (!_attackablePlatforms.Contains(platform))
        {
            _attackablePlatforms.Add(platform);
        }
    }

    public void SetNotAttackable(Platform platform)
    {
        if (_attackablePlatforms.Contains(Utils.GetPlatformObject(platform)))
        {
            _attackablePlatforms.Remove(Utils.GetPlatformObject(platform));
        }
    }

    public void SetNotAttackable(GameObject platform)
    {
        if (_attackablePlatforms.Contains(platform))
        {
            _attackablePlatforms.Remove(platform);
        }
    }

    private float GetBeatFraction()
    {
        var fBeat = _metronome.TimeElapsed / _metronome.SecondsPerBeat;
        return (float)fBeat - Mathf.Floor((float)fBeat);
    }

    private float ComputeFractionFromEndpoints(float startVal, float endVal)
    {
        return startVal + (GetBeatFraction() * (endVal - startVal));
    }
}