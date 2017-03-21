using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

public class PlayerProperties : MonoBehaviour
{
    public Player _player = Player.One;

    public GameObject _otherPlayer;

    public GameController _gameController;

    public GameObject _canvas;

    public GameObject _pointOne;
    public GameObject _pointTwo;
    public GameObject _pointThree;

    public Metronome _metronome;

    public GameObject _cheerleader1;
    public GameObject _cheerleader2;
    public GameObject _cheerleader3;
    public GameObject _cheerleader4;
    public GameObject _cheerleader5;

    public SoundEffectPlayer _sfx;

    public bool HasWon { get; private set; }

    private PointObject _p1;
    private PointObject _p2;
    private PointObject _p3;

    private int _winBeat = 9000;

    private void Start()
    {
        HasWon = false;

        _p1 = _pointOne.GetComponent<PointObject>();
        _p2 = _pointTwo.GetComponent<PointObject>();
        _p3 = _pointThree.GetComponent<PointObject>();
    }

    private void Update()
    {
        if (HasWon && _winBeat < _metronome.BeatsElapsed)
        {
            // no tie
            if (!_otherPlayer.GetComponent<PlayerProperties>().HasWon)
            {
                // the human player wins
                if (_player == Player.One)
                {
                    AddCheerleader();
                    _otherPlayer.GetComponent<AIController>().IncreaseLevel();
                    _sfx.PlayWinSound(_otherPlayer.GetComponent<PlayerProperties>().GetPointsScored());
                }
                else
                {
                    RemoveCheerleader();
                    GetComponent<AIController>().DecreaseLevel();
                    _sfx.PlayLossSound(_otherPlayer.GetComponent<PlayerProperties>().GetPointsScored());
                }
            }

            // tie
            else
            {
                _sfx.PlayTieSound();
            }

            _gameController.RestartGame();
        }
    }

    public void ScorePoint()
    {
        if (!_p1.Scored)
        {
            _p1.ScorePoint();
        }

        else if (!_p2.Scored)
        {
            _p2.ScorePoint();
        }

        else if (!_p3.Scored)
        {
            _p3.ScorePoint();
            HasWon = true;
            _winBeat = _metronome.BeatsElapsed;
        }

        PlayPointSound();
    }

    public void ResetPoints()
    {
        _p1.ResetPoint();
        _p2.ResetPoint();
        _p3.ResetPoint();

        HasWon = false;
    }

    public int GetPointsScored()
    {
        if (_p1.Scored)
        {
            return 1;
        }
        else if (_p2.Scored)
        {
            return 2;
        }
        else if (_p3.Scored)
        {
            return 3;
        }

        return 0;
    }

    public void RemoveAllCheerleaders()
    {
        _cheerleader1.SetActive(false);
        _cheerleader2.SetActive(false);
        _cheerleader3.SetActive(false);
        _cheerleader4.SetActive(false); 
        _cheerleader5.SetActive(false);
    }

    private void PlayPointSound()
    {
        if (!_p3.Scored)
        {
            _sfx.PlayPointSound(_player);
        }
    }

    private void AddCheerleader()
    {
        if (!_cheerleader1.activeInHierarchy)
        {
            _cheerleader1.SetActive(true);
        }
        else if (!_cheerleader2.activeInHierarchy)
        {
            _cheerleader2.SetActive(true);
        }
        else if (!_cheerleader3.activeInHierarchy)
        {
            _cheerleader3.SetActive(true);
        }
        else if (!_cheerleader4.activeInHierarchy)
        {
            _cheerleader4.SetActive(true);
        }
        else if (!_cheerleader5.activeInHierarchy)
        {
            _cheerleader5.SetActive(true);
        }
    }

    private void RemoveCheerleader()
    {
        if (_cheerleader5.activeInHierarchy)
        {
            _cheerleader5.SetActive(false);
        }
        else if (_cheerleader4.activeInHierarchy)
        {
            _cheerleader4.SetActive(false);
        }
        else if (_cheerleader3.activeInHierarchy)
        {
            _cheerleader3.SetActive(false);
        }
        else if (_cheerleader2.activeInHierarchy)
        {
            _cheerleader2.SetActive(false);
        }
        else if (_cheerleader1.activeInHierarchy)
        {
            _cheerleader1.SetActive(false);
        }
    }
}