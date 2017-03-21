using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class GameController : MonoBehaviour
{
    public GameObject _leftPlayer;

    public GameObject _rightPlayer;

    public Color _defaultPlatformColor = Color.white;

    // key used to start and stop music
    public KeyCode _startStopKey = KeyCode.Return;

    // key used to pause and resume music
    public KeyCode _pauseResumeKey = KeyCode.Space;

    // how much faster the fast metronome is
    public int _fastMetronomeFactor = 4;

    public GameObject _canvas;

    public GameObject _counter;

    public GameObject _title;

    public GameObject _startButton;

    public GameObject _quitButton;

    public GameObject _menuButton;

    public GameObject _creditsButton;

    public GameObject _creditsPanel;

    public GameObject _instructionsButton;

    public GameObject _instructionsPanel;

    public GameObject _aiController;

    public GameObject _quitPanel;

    public GameObject _quitConfirmButton;

    public GameObject _quitDeconfirmButton;

    public SoundEffectPlayer _soundEffectPlayer;

    public PlayerController _humanPlayerController; 

    // reference to the object which controls the music and timing
    public Metronome MainMetronome { get; private set; }

    // metronome used for visual effects
    public Metronome FastMetronome { get; private set; }

    private ThemePlayer _themePlayer;

    private string[] _countdownNumbers = { "4", "3", "2", "1" };

    private int _currentBeat = 0;

    private int _countDownIndex = 0;

    public bool CountdownFinished { get; private set; }

    private bool _gameStarted = false;

    private bool _audioStarted = false;

    private bool _creditsActive = false;

    private bool _instructionsActive = false;

    private bool _soundPlayed = false;

    private bool _shouldQuit = false;

    private void Awake()
    {
        MainMetronome = GameObject.Find("Metronome").GetComponent<Metronome>();
        FastMetronome = gameObject.AddComponent<Metronome>();
        FastMetronome._beatsPerMinute = MainMetronome._beatsPerMinute * _fastMetronomeFactor;
        FastMetronome._acceptableBeatRadius = 0.2f;

        _themePlayer = Utils.GetThemePlayer().GetComponent<ThemePlayer>();
    }

    private void Start()
    {
        _counter.SetActive(false);
        _startButton.GetComponent<Button>().onClick.AddListener(new UnityAction(() =>
        {
            _soundEffectPlayer.PlayFight();
            _startButton.SetActive(false);
            _quitButton.SetActive(false);
            _creditsButton.SetActive(false);
            _creditsPanel.SetActive(false);
            _instructionsButton.SetActive(false);
            _instructionsPanel.SetActive(false);
            _quitPanel.SetActive(false);

            _creditsActive = false;
            _instructionsActive = false;

            _themePlayer.Stop();

            StartGame();
        }));

        _menuButton.GetComponent<Button>().onClick.AddListener(new UnityAction(() =>
        {
            _soundEffectPlayer.PlayMenu();
            _quitPanel.SetActive(false);
            _creditsPanel.SetActive(false);
            _instructionsPanel.SetActive(false);
            EndGame();
            _humanPlayerController.DeactivateArrowIndicators();
            _themePlayer.Play();
        }));

        _creditsButton.GetComponent<Button>().onClick.AddListener(new UnityAction(() =>
        {
            _soundEffectPlayer.PlayCredits();
            _quitPanel.SetActive(false);
            _instructionsPanel.SetActive(false);
            _creditsPanel.SetActive(!_creditsActive);
            _creditsActive = !_creditsActive;
        }));

        _instructionsButton.GetComponent<Button>().onClick.AddListener(new UnityAction(() =>
        {
            _soundEffectPlayer.PlayInstructions();
            _quitPanel.SetActive(false);
            _creditsPanel.SetActive(false);
            _instructionsPanel.SetActive(!_instructionsActive);
            _instructionsActive = !_instructionsActive;
        }));

        _quitButton.GetComponent<Button>().onClick.AddListener(new UnityAction(() =>
        {
            _quitPanel.SetActive(true);
            _instructionsPanel.SetActive(false);
            _creditsPanel.SetActive(false);
            _soundEffectPlayer.PlayQuitQuestion();
        }));

        _quitConfirmButton.GetComponent<Button>().onClick.AddListener(new UnityAction(() =>
        {
            _soundEffectPlayer.PlayQuitConfirm();
            _shouldQuit = true;
        }));

        _quitDeconfirmButton.GetComponent<Button>().onClick.AddListener(new UnityAction(() =>
        {
            _quitPanel.SetActive(false);
        }));


        ReturnGameToDefaultState();
    }

    private void Update()
    {
        if (Input.GetKeyDown(_pauseResumeKey))
        {
            if (MainMetronome.Paused)
            {
                UnpauseGame();
            }
            else
            {
                PauseGame();
            }
        }

        DoCountDown();

        if (_shouldQuit)
        {
            if (!_soundEffectPlayer.gameObject.GetComponent<AudioSource>().isPlaying)
            {
                Application.Quit();
            }
        }
    }

    private void DoCountDown()
    {
        if (MainMetronome.Started && !_gameStarted)
        {
            if (!CountdownFinished)
            {
                _counter.GetComponent<Text>().text = _countdownNumbers[_countDownIndex];

                if (!_soundPlayed && _countDownIndex != 0)
                {
                    _soundEffectPlayer.PlayCountdownSound(4 - _countDownIndex);
                    _soundPlayed = true;
                }
            }
            else if (CountdownFinished && !_gameStarted)
            {
                if (!_audioStarted)
                {
                    MainMetronome.StartAudio();
                    MainMetronome.StartMetronome();
                    _audioStarted = true;
                }

                _gameStarted = true;
                MainMetronome.ResyncAudio();
            }

            if (_currentBeat < MainMetronome.BeatsElapsed)
            {
                _soundPlayed = false;
                _currentBeat += 1;
                _countDownIndex += 1;

                if (_countDownIndex == 4)
                {
                    CountdownFinished = true;
                    _counter.SetActive(false);
                }
            }
        }
    }

    private void ReturnGameToDefaultState()
    {
        _counter.SetActive(false);
        _startButton.SetActive(true);
        _quitButton.SetActive(true);
        _creditsButton.SetActive(true);
        _instructionsButton.SetActive(true);
        _quitPanel.SetActive(false);

        CountdownFinished = false;
        _countDownIndex = 0;
        _currentBeat = 0;
        _gameStarted = false;

        _creditsActive = false;
        _instructionsActive = false;

        _leftPlayer.GetComponent<PlayerProperties>().ResetPoints();
        _rightPlayer.GetComponent<PlayerProperties>().ResetPoints();

        if (!_leftPlayer.GetComponent<PlayerController>()._playerControlled)
        {
            _leftPlayer.GetComponent<AIController>().Resync();
        }

        if (!_rightPlayer.GetComponent<PlayerController>()._playerControlled)
        {
            _rightPlayer.GetComponent<AIController>().Resync();
        }

        foreach (var platform in GameObject.FindGameObjectsWithTag("Platform"))
        {
            platform.GetComponent<MeshRenderer>().material.color = _defaultPlatformColor;
        }
    }

    public void StartGame()
    {
        foreach (var platform in GameObject.FindGameObjectsWithTag("Platform"))
        {
            platform.GetComponent<MeshRenderer>().material.color = _defaultPlatformColor;
        }

        _audioStarted = false;

        _counter.SetActive(true);

        MainMetronome.StartMetronome();
        FastMetronome.StartMetronome();
    }

    public void EndGame()
    {
        ReturnGameToDefaultState();

        _aiController.GetComponent<AIController>().ResetLevel();

        MainMetronome.StopMetronome();
        MainMetronome.StopAudio();
        FastMetronome.StopMetronome();

        _audioStarted = false;
    }

    public void RestartGame()
    {
        ReturnGameToDefaultState();

        _startButton.SetActive(false);
        _quitButton.SetActive(false);
        _creditsButton.SetActive(false);
        _instructionsButton.SetActive(false);

        _counter.SetActive(true);

        _currentBeat = MainMetronome.BeatsElapsed;
    }

    public void PauseGame()
    {
        MainMetronome.PauseMetronome();
        MainMetronome.PauseAudio();
        FastMetronome.PauseMetronome();
        foreach (var platform in GameObject.FindGameObjectsWithTag("Platform"))
        {
            platform.GetComponent<MeshRenderer>().material.color = _defaultPlatformColor;
        }
    }

    public void UnpauseGame()
    {
        MainMetronome.UnpauseMetronome();
        MainMetronome.UnpauseAudio();
        FastMetronome.UnpauseMetronome();
    }
}