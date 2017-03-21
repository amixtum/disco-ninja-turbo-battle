using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class AIController : MonoBehaviour
{
    public float _levelOneMistakeChance = 0.5f;

    public float _levelTwoMistakeChance = 0.25f;

    public float _levelThreeMistakeChance = 0f;

    public Text _levelText;

    public GameController _gameController;

    public int CurrentLevel { get; private set; }

    public List<PlayerAction> _seq1 = new List<PlayerAction>();

    public List<PlayerAction> _seq2 = new List<PlayerAction>();

    public List<PlayerAction> _seq3 = new List<PlayerAction>();

    public List<Color> _levelColors = new List<Color>();

    public GameObject _enemyHead;

    public Metronome _metronome;

    private GameObject _otherPlayer;

    private Ring<Color> _headColorRing = new Ring<Color>();

    private PlayerController _controller;

    private GameState _gameState;

    private int _localBeat = 0;

    private Queue<PlayerAction> _actionQueue = new Queue<PlayerAction>();

    private Ring<PlayerAction> _sequence1 = new Ring<PlayerAction>();

    private Ring<PlayerAction> _sequence2 = new Ring<PlayerAction>();

    private Ring<PlayerAction> _sequence3 = new Ring<PlayerAction>();

    private float MistakeChance
    {
        get
        {
            switch (CurrentLevel)
            {
                case 4:
                    return _levelOneMistakeChance;
                case 5:
                    return _levelTwoMistakeChance;
                case 6:
                    return _levelThreeMistakeChance;
                default:
                    return 0;
            }
        }
    }

    private void Start()
    {
        foreach (var p in GameObject.FindGameObjectsWithTag("Player"))
        {
            if (p != gameObject)
            {
                _otherPlayer = p;
            }
        }

        foreach (var color in _levelColors)
        {
            _headColorRing.Add(color);
        }
        _headColorRing.MoveToFirst();
        
        _levelText.text = "Level 1";

        CurrentLevel = 1;

        _enemyHead.GetComponent<SkinnedMeshRenderer>().material.color = _headColorRing.GetValue();

        // set up the sequence rings for levels 1-3
        foreach (var action in _seq1)
        {
            _sequence1.Add(action);
        }
        foreach (var action in _seq2)
        {
            _sequence2.Add(action);
        }
        foreach (var action in _seq3)
        {
            _sequence3.Add(action);
        }

        _sequence1.MoveToFirst();
        _sequence2.MoveToFirst();
        _sequence3.MoveToFirst();

        _controller = GetComponent<PlayerController>();
        _gameState = GetComponent<GameState>();
        _actionQueue.Enqueue(GetNextAction());
    }

    private void Update()
    {
        if (_metronome.Started && !_metronome.Paused)
        {
            if (_actionQueue.Count != 0)
            {
                _controller.PerformAction(_actionQueue.Dequeue());
            }
            if (_localBeat < _metronome.BeatsElapsed)
            {
                _localBeat = _metronome.BeatsElapsed;
                _actionQueue.Enqueue(GetNextAction());
                //_actionQueue.Enqueue(GetDecisionTreeAction());
            }

            if (_localBeat > _metronome.BeatsElapsed)
            {
                _localBeat = _metronome.BeatsElapsed;
            }
        }
    }

    public void Resync()
    {
        _localBeat = 0;
    }

    public void SetBeat(int beat)
    {
        _localBeat = beat;
    }

    public void IncreaseLevel()
    {
        CurrentLevel += 1;

        if (CurrentLevel == 7)
        {
            if (_metronome.CurrentTrack + 1 != _metronome.TrackCount)
            {
                _metronome.SetTrack(_metronome.CurrentTrack + 1);
                _otherPlayer.GetComponent<PlayerProperties>().RemoveAllCheerleaders();
            }
            else
            {
                SceneManager.LoadSceneAsync(2);
                Utils.GetThemePlayer().GetComponent<ThemePlayer>().Play();
            }
            _gameController.StartGame();
            _gameController.RestartGame();
            CurrentLevel = 1;
        }

        _levelText.text = "Level " + CurrentLevel;
        _headColorRing.MoveNext();
        _enemyHead.GetComponent<SkinnedMeshRenderer>().material.color = _headColorRing.GetValue();
    }

    public void DecreaseLevel()
    {
        CurrentLevel = (CurrentLevel == 1) ? 1 : CurrentLevel - 1;
        _levelText.text = "Level " + CurrentLevel;
        _headColorRing.MovePrev();
        _enemyHead.GetComponent<SkinnedMeshRenderer>().material.color = _headColorRing.GetValue();
    }

    public void ResetLevel()
    {
        CurrentLevel = 1;
    }

    private PlayerAction MakeWeightedChoice(PlayerAction a1, PlayerAction a2, float a1Prob)
    {
        if (a1Prob < 0 || a1Prob > 1f)
        {
            Debug.Log("AI Controller Warning: Calling MakeWeightedChoice() with probability outside [0,1]");
        }
        var randomNumber = UnityEngine.Random.Range(0f, 1f);
        return (randomNumber < a1Prob) ? a1 : a2;
    }

    private PlayerAction GenRandomActionUniform()
    {
        return PlayerController.ActionList[UnityEngine.Random.Range(0, PlayerController.ActionList.Length)];
    }

    private PlayerAction GetNextAction()
    {
        PlayerAction toReturn = PlayerAction.MoveLeft;
        switch (CurrentLevel)
        {
            case 1:
                toReturn = _sequence1.GetValue();
                _sequence1.MoveNext();
                return toReturn;
            case 2:
                toReturn = _sequence2.GetValue();
                _sequence2.MoveNext();
                return toReturn;
            case 3:
                toReturn = _sequence3.GetValue();
                _sequence3.MoveNext();
                return toReturn;
            case 4:
                return GetDecisionTreeAction();
            case 5:
                return GetDecisionTreeAction();
            case 6:
                return GetDecisionTreeAction();
            default:
                return PlayerAction.MoveLeft;
        }
    }

    private PlayerAction GetDecisionTreeAction()
    {
        if (CanAttackPlayer())
        {
            if (InAttackableSpace())
            {
                if (_gameState.AIAttackLeftSuccess())
                {
                    return MakeWeightedChoice(GenRandomActionUniform(), PlayerAction.AttackLeft, MistakeChance);
                }
                else
                {
                    return MakeWeightedChoice(GenRandomActionUniform(), PlayerAction.AttackRight, MistakeChance);
                }
            }
            else
            {
                if (_gameState.AIAttackLeftSuccess())
                {
                    return MakeWeightedChoice(GenRandomActionUniform(), PlayerAction.AttackLeft, MistakeChance);
                }
                else
                {
                    return MakeWeightedChoice(GenRandomActionUniform(), PlayerAction.AttackRight, MistakeChance);
                }
            }
        }
        else
        {
            if (InAttackableSpace())
            {
                if (_gameState.AICanMoveLeftSafe())
                {
                    return MakeWeightedChoice(GenRandomActionUniform(), PlayerAction.MoveLeft, MistakeChance);
                }
                else
                {
                    return MakeWeightedChoice(GenRandomActionUniform(), PlayerAction.MoveRight, MistakeChance);
                }
            }
            else
            {
                return MakeWeightedChoice(PlayerAction.MoveLeft, PlayerAction.MoveRight, 0.5f);
            }
        }
    }

    private bool InAttackableSpace()
    {
        return _gameState.AIPlayerCanBeAttacked();
    }

    private bool InSafeSpace()
    {
        return !InAttackableSpace();
    }

    private bool CanAttackPlayer()
    {
        return _gameState.HumanPlayerCanBeAttacked();
    }

    private bool CannotAttackPlayer()
    {
        return !CanAttackPlayer();
    }
}