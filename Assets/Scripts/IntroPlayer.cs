using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroPlayer : MonoBehaviour
{
    public List<AudioClip> _clips = new List<AudioClip>();

    private AsyncOperation _sceneLoad;

    public bool Done
    {
        get
        {
            return !GetComponent<AudioSource>().isPlaying;
        }
    }

    private bool _startedLoading = false;

    private void Awake()
    {
        var clip = _clips[UnityEngine.Random.Range(0, _clips.Count)];
        GetComponent<AudioSource>().clip = clip;
        GetComponent<AudioSource>().Play();
    }

    private void Start()
    {

    }

    private void Update()
    {
        if (!_startedLoading)
        {
            _sceneLoad = SceneManager.LoadSceneAsync(1);
            _sceneLoad.allowSceneActivation = false;
            _startedLoading = true;
        }

        if (Done)
        {
            _sceneLoad.allowSceneActivation = true;            
        }
    }
}