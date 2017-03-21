using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

public class DiscoSphere : MonoBehaviour
{
    public GameObject _tile;

    public float _radius = 10f;

    public float _zIncrement = 1f;

    public float _thetaIncrement = 0.1f;

    public float _maxRotationVelocity = 50f;

    public float _rotationDecayFactor = 0.5f;

    public Metronome _metronome;

    public List<Color> _colorSequence = new List<Color>();

    public List<GameObject> Tiles { get; private set; }

    private Ring<Color> _colorRing = new Ring<Color>();

    private int _localBeat = 0;

    private float _rotationVelocity;

    private void Start()
    {
        _rotationVelocity = _maxRotationVelocity;
        Initialize();
    }

    private void Update()
    {
        if (_metronome.Started && !_metronome.Paused && _metronome.WasCalledOnBeat())
        {
            if (_localBeat < _metronome.BeatsElapsed)
            {
                Step();
                _localBeat = _metronome.BeatsElapsed;
            }

            if (_localBeat > _metronome.BeatsElapsed)
            {
                _localBeat = _metronome.BeatsElapsed;
            }
        }

        foreach (var tile in Tiles)
        {
            tile.transform.RotateAround(Vector3.zero, Vector3.forward, _rotationVelocity * Time.deltaTime);
        }

        _rotationVelocity *= _rotationDecayFactor;
    }

    private void Initialize()
    {
        Tiles = new List<GameObject>();
        foreach (var color in _colorSequence)
        {
            _colorRing.Add(color);
        }
        _colorRing.MoveToFirst();

        float zLevel = _radius;
        float theta = 0f;
        while (zLevel >= -_radius / 2f)
        {
            while (theta <= Mathf.PI * 2 - _thetaIncrement)
            {
                float x = Mathf.Sqrt(_radius * _radius - zLevel * zLevel) * Mathf.Cos(theta);
                float y = Mathf.Sqrt(_radius * _radius - zLevel * zLevel) * Mathf.Sin(theta);
                float z = zLevel;

                Tiles.Add(Instantiate<GameObject>(_tile, new Vector3(x, y, z), Quaternion.identity));

                theta += _thetaIncrement;
            }
            theta = 0;
            zLevel -= _zIncrement;
        }
    }

    private void Step()
    {
        _colorRing.MoveNext();
        foreach (var tile in Tiles)
        {
            _rotationVelocity = _maxRotationVelocity;
            tile.GetComponent<Scaler>().StartScaling();
        }
    }
}