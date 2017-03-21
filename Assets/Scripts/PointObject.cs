using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

public class PointObject : MonoBehaviour
{
    public bool Scored { get; private set; }

    public Color _defaultColor = Color.white;

    private void Awake()
    {
        Scored = false;
    }

    public void ScorePoint()
    {
        if (!Scored)
        {
            Scored = true;
            OnScore();
        }
    }

    public void ResetPoint()
    {
        Scored = false;
        GetComponent<MeshRenderer>().material.color = _defaultColor;
    }

    private void OnScore()
    {
        GetComponent<MeshRenderer>().material.color = new Color(1f, 0, 1f);
    }
}