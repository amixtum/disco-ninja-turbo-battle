using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

[System.Serializable]
public struct AudioClipWithBPM
{
    public AudioClip _clip;
    public int _bpm;

    public AudioClipWithBPM(AudioClip clip, int bpm)
    {
        _clip = clip;
        _bpm = bpm;
    }
}

public class Metronome : MonoBehaviour
{
    // bpm of the audio track
    public int _beatsPerMinute = 120;

    // acceptable radius for input to be entered around beat
    public double _acceptableBeatRadius = 0.2f;

    public List<AudioClipWithBPM> _clips = new List<AudioClipWithBPM>();

    public float _resyncInterval = 30f;

    public int CurrentTrack { get; private set; }

    public int TrackCount
    {
        get
        {
            return _clips.Count;
        }
    }

    // the time the audio was started
    public double StartTime { get; private set; }

    // the number of seconds per each beat
    public double SecondsPerBeat
    {
        get
        {
            return 60.0 / (_beatsPerMinute);
        }
    }

    // the time elapsed since the audio track was started
    public double TimeElapsed
    {
        get
        {
            return (AudioSettings.dspTime - StartTime) + _pauseOffset;
        }
    }

    // number of beats that have passed
    public int BeatsElapsed
    {
        get
        {
            return (int)Math.Floor(TimeElapsed / SecondsPerBeat);
        }
    }

    // seconds since the last beat was triggered
    private double UpperBound
    {
        get
        {
            return (BeatsElapsed * SecondsPerBeat) + _acceptableBeatRadius;
        }
    }

    // seconds before the next beat will be triggered
    private double LowerBound
    {
        get
        {
            return ((BeatsElapsed + 1) * SecondsPerBeat) - _acceptableBeatRadius;
        }
    }

    // whether the audio is started or not
    public bool Started { get; private set; }

    // whether the audio is paused or not
    public bool Paused { get; private set; }

    // the time that the audio track was paused
    private double _pauseStart = 0.0;

    // the total time the audio has been paused, but not stopped
    private double _pauseOffset = 0.0;

    // the audio source with which music is played
    private AudioSource _audioSource;

    private float _timeSinceResync = 0f;

    // setup default values
    private void Awake()
    {
        Started = false;
        Paused = false;
    }

    // get the audio source
    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        if (_clips.Count > 0)
        {
            SetTrack(0);
        }
    }

    // keep track of which beat we are on
    private void Update()
    {
        if (Started && !Paused && GetComponent<AudioSource>() != null)
        {
            _timeSinceResync += Time.deltaTime;
            if (_timeSinceResync >= _resyncInterval)
            {
                ResyncAudio();
            }
        }
    }

    // use this to determine if input is in an acceptable radius of the current beat
    public bool WasCalledOnBeat()
    {
        return TimeElapsed - UpperBound <= _acceptableBeatRadius;
    }

    // use this to start the audio from the beginning
    public void StartMetronome()
    {
        Started = true;
        Paused = false;
        StartTime = AudioSettings.dspTime;
        _pauseOffset = 0.0;
    }

    // use this to stop the audio
    public void StopMetronome()
    {
        Started = false;
    }

    // use this to pause the audio e.g. pause menu
    public void PauseMetronome()
    {
        Paused = true;
        _pauseStart = AudioSettings.dspTime;
    }

    // use this to unpause the audio e.g. resume playing the game from some menu state
    public void UnpauseMetronome()
    {
        Paused = false;
        _pauseOffset += _pauseStart - AudioSettings.dspTime;
    }

    public void StartAudio()
    {
        if (_audioSource.isPlaying)
        {
            _audioSource.Stop();
        }

        _audioSource.Play();

        ResyncAudio();
    }

    public void StopAudio()
    {
        _audioSource.Stop();
    }

    public void PauseAudio()
    {
        _audioSource.Pause();
    }

    public void UnpauseAudio()
    {
        _audioSource.UnPause();
    }

    // side-effects: stops currently playing audio and metronome
    // Metronome must be started again after calling this function
    public void SetTrack(int trackIndex)
    {
        if (trackIndex >= _clips.Count)
        {
            Debug.Log("Metronome Warning: Calling SetTrack() with invalid index");
            return;
        }

        if (GetComponent<AudioSource>() != null)
        {
            StopAudio();
        }
        StopMetronome();

        _audioSource.clip = _clips[trackIndex]._clip;
        _beatsPerMinute = _clips[trackIndex]._bpm;
        CurrentTrack = trackIndex;
    }

    public void ResyncAudio()
    {
        float factor = Mathf.Floor((float)TimeElapsed / _audioSource.clip.length);
        float seekTo = (float)TimeElapsed - (factor * _audioSource.clip.length); 
        _audioSource.time = seekTo; 
    }
}