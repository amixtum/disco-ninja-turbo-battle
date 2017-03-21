using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Platform
{
    L1,
    L2,
    L3,
    L4,
    R1,
    R2,
    R3,
    R4,
}

public class Utils : MonoBehaviour
{
    private static Dictionary<Platform, GameObject> _platformObjects = new Dictionary<Platform, GameObject>();

    private static Dictionary<Platform, Vector3> _platformPositions = new Dictionary<Platform, Vector3>();

    private static Metronome _metronome;

    private static bool _initialized = false;

    private void Awake()
    {
        if (!_initialized)
        {
            _platformPositions.Add(Platform.L1, GameObject.Find("L1").transform.position);
            _platformPositions.Add(Platform.L2, GameObject.Find("L2").transform.position);
            _platformPositions.Add(Platform.L3, GameObject.Find("L3").transform.position);
            _platformPositions.Add(Platform.L4, GameObject.Find("L4").transform.position);

            _platformPositions.Add(Platform.R1, GameObject.Find("R1").transform.position);
            _platformPositions.Add(Platform.R2, GameObject.Find("R2").transform.position);
            _platformPositions.Add(Platform.R3, GameObject.Find("R3").transform.position);
            _platformPositions.Add(Platform.R4, GameObject.Find("R4").transform.position);

            _platformObjects.Add(Platform.L1, GameObject.Find("L1"));
            _platformObjects.Add(Platform.L2, GameObject.Find("L2"));
            _platformObjects.Add(Platform.L3, GameObject.Find("L3"));
            _platformObjects.Add(Platform.L4, GameObject.Find("L4"));

            _platformObjects.Add(Platform.R1, GameObject.Find("R1"));
            _platformObjects.Add(Platform.R2, GameObject.Find("R2"));
            _platformObjects.Add(Platform.R3, GameObject.Find("R3"));
            _platformObjects.Add(Platform.R4, GameObject.Find("R4"));

            _metronome = GameObject.Find("Metronome").GetComponent<Metronome>();

            _initialized = true;
        }
    }

    public static Vector3 GetPlatformPosition(Platform platform)
    {
        return _platformPositions[platform];
    }

    public static GameObject GetPlatformObject(Platform platform)
    {
        return _platformObjects[platform];
    }

    public static Platform GetPlatformLeftOf(Platform platform)
    {
        switch (platform)
        {
            case Platform.L1:
                return Platform.L4;
            case Platform.L2:
                return Platform.L1;
            case Platform.L3:
                return Platform.L2;
            case Platform.L4:
                return Platform.L3;
            case Platform.R1:
                return Platform.R4;
            case Platform.R2:
                return Platform.R1;
            case Platform.R3:
                return Platform.R2;
            case Platform.R4:
                return Platform.R3;
            default:
                return Platform.R1;
        }
    }

    public static Platform GetPlatformRightOf(Platform platform)
    {
        switch (platform)
        {
            case Platform.L1:
                return Platform.L2;
            case Platform.L2:
                return Platform.L3;
            case Platform.L3:
                return Platform.L4;
            case Platform.L4:
                return Platform.L1;
            case Platform.R1:
                return Platform.R2;
            case Platform.R2:
                return Platform.R3;
            case Platform.R3:
                return Platform.R4;
            case Platform.R4:
                return Platform.R1;
            default:
                return Platform.R1;
        }
    }

    public static GameObject GetThemePlayer()
    {
        return GameObject.Find("ThemePlayer");
    }

    public static float GetBeatFraction()
    {
        var fBeat = _metronome.TimeElapsed / _metronome.SecondsPerBeat;
        return (float)fBeat - Mathf.Floor((float)fBeat);
    }
}
