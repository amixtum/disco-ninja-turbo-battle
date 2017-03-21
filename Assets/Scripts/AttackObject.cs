using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

public class AttackObject : MonoBehaviour
{
    public float _timeToDestroy = 1f;

    private float _interactionDelay;

    private float _interactionTime;

    private float _timeAlive = 0f;

    bool _triggerAdded = false;

    private void Start()
    {
        _interactionTime = (float)(GameObject.Find("Metronome").GetComponent<Metronome>().SecondsPerBeat / 2.0);
        _interactionDelay = _interactionTime / 2f;
        GetComponent<BoxCollider>().enabled = false;
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "Player" && _timeAlive < _interactionTime)
        {
            var playerNum = collider.gameObject.GetComponent<PlayerProperties>()._player;

            switch (playerNum)
            {
                case Player.One:
                    GameObject.Find("Right Player").GetComponent<PlayerProperties>().ScorePoint();
                    break;
                case Player.Two:
                    GameObject.Find("Left Player").GetComponent<PlayerProperties>().ScorePoint();
                    break;
            }
        }
    }

    private void Update()
    {
        _timeAlive += Time.deltaTime;
        if (_timeAlive >= _interactionDelay && !_triggerAdded)
        {
            GetComponent<BoxCollider>().enabled = true;
        }

        if (_timeAlive >= _timeToDestroy)
        {
            Destroy(this.gameObject);
        }
    }
}