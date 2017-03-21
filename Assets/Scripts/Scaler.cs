using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

public class Scaler : MonoBehaviour
{
    public float _startScale = 1;

    public float _apexScale = 1.2f;

    public float _scaleSpeed = 2f;

    private bool _reachedApex = false;

    private bool _started = false;

    private void Start()
    {
    }

    private void Update()
    {
        if (_started)
        {
            if (!_reachedApex)
            {
                transform.localScale += (_scaleSpeed * Time.deltaTime) * Vector3.one;
            }
            else 
            {
                transform.localScale -= (_scaleSpeed * Time.deltaTime) * Vector3.one;
            }

            if (transform.localScale.x >= _apexScale)
            {
                _reachedApex = true;
            }

            if (transform.localScale.x <= _startScale)
            {
                _started = false;
            }
        }
    }

    public void StartScaling()
    {
        _started = true;
        _reachedApex = false;
    }
}