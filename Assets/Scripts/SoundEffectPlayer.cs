using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

public class SoundEffectPlayer : MonoBehaviour
{
    public AudioClip _fourVoice;
    public AudioClip _threeVoice;
    public AudioClip _twoVoice;
    public AudioClip _oneVoice;

    public AudioClip _fightVoice;
    public AudioClip _instructionsVoice;
    public AudioClip _creditsVoice;
    public AudioClip _quitVoice;
    public AudioClip _quitConfirmVoice;
    public AudioClip _menuVoice;

    public AudioClip _playerPointSound;
    public AudioClip _enemyPointSound;
    public AudioClip _perfectWinSound;
    public AudioClip _nearPerfectWinSound;
    public AudioClip _closeWinSound;
    public AudioClip _tieSound;
    public AudioClip _nearLossSound;
    public AudioClip _badLossSound;
    public AudioClip _perfectLossSound;

    public void PlayCountdownSound(int n)
    {
        if (n <= 0 || n > 4)
        {
            Debug.Log("SoundEffectPlayer Warning: Calling PlayCountdownSound with invalid number");
        }
        AudioClip clip;
        switch (n)
        {
            case 1:
                clip = _oneVoice;
                break;
            case 2:
                clip = _twoVoice;
                break;
            case 3:
                clip = _threeVoice;
                break;
            case 4:
                clip = _fourVoice;
                break;
            default:
                clip = _oneVoice;
                break;
        }

        GetComponent<AudioSource>().clip = clip;
        GetComponent<AudioSource>().Play();
    }

    public void PlayFight()
    {
        GetComponent<AudioSource>().clip = _fightVoice;
        GetComponent<AudioSource>().Play();
    }

    public void PlayInstructions()
    {
        GetComponent<AudioSource>().clip = _instructionsVoice;
        GetComponent<AudioSource>().Play();
    }

    public void PlayCredits()
    {
        GetComponent<AudioSource>().clip = _creditsVoice;
        GetComponent<AudioSource>().Play();
    }

    public void PlayQuitQuestion()
    {
        GetComponent<AudioSource>().clip = _quitVoice;
        GetComponent<AudioSource>().Play();
    }

    public void PlayQuitConfirm()
    {
        GetComponent<AudioSource>().clip = _quitConfirmVoice;
        GetComponent<AudioSource>().Play();
    }

    public void PlayMenu()
    {
        GetComponent<AudioSource>().clip = _menuVoice;
        GetComponent<AudioSource>().Play();
    }

    public void PlayPointSound(Player player)
    {
        //switch (player)
        //{
        //    case Player.One:
        //        GetComponent<AudioSource>().clip = _enemyPointSound;
        //        GetComponent<AudioSource>().Play();
        //        break;
        //    case Player.Two:
        //        GetComponent<AudioSource>().clip = _playerPointSound;
        //        GetComponent<AudioSource>().Play();
        //        break;
        //    default:
        //        break;
        //}
    }

    public void PlayWinSound(int enemyPoints)
    {
        switch (enemyPoints)
        {
            case 0:
                GetComponent<AudioSource>().clip = _perfectWinSound;
                GetComponent<AudioSource>().Play();
                break;
            case 1:
                GetComponent<AudioSource>().clip = _nearPerfectWinSound;
                GetComponent<AudioSource>().Play();
                break;
            case 2:
                GetComponent<AudioSource>().clip = _closeWinSound;
                GetComponent<AudioSource>().Play();
                break;
            default:
                break;
        }
    }

    public void PlayLossSound(int playerPoints)
    {
        switch (playerPoints)
        {
            case 0:
                GetComponent<AudioSource>().clip = _perfectLossSound;
                GetComponent<AudioSource>().Play();
                break;
            case 1:
                GetComponent<AudioSource>().clip = _badLossSound;
                GetComponent<AudioSource>().Play();
                break;
            case 2:
                GetComponent<AudioSource>().clip = _nearLossSound;
                GetComponent<AudioSource>().Play();
                break;
            default:
                break;
        }
    }

    public void PlayTieSound()
    {
        GetComponent<AudioSource>().clip = _tieSound;
        GetComponent<AudioSource>().Play();
    }
}