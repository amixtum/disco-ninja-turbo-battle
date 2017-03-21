using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

public struct StateObject
{

}

public class GameState : MonoBehaviour
{
    public GameObject _humanPlayer;
    public GameObject _aiPlayer;

    public PlayerController _humanController;
    public PlayerController _aiController;

    public PlayerProperties _humanProps;
    public PlayerProperties _aiProps;

    public Platform HumanPlayerPlatform { get; private set; }
    public Platform AIPlayerPlatform { get; private set; }

    public Pair<Platform> HumanAttackablePlatforms { get; private set; }
    public Pair<Platform> AIAttackablePlatforms { get; private set; }

    public int HumanPlayerPoints { get; private set; }
    public int AIPlayerPoints { get; private set; }

    private void Start()
    {
        HumanPlayerPlatform = _humanController.GetThisPlatform();
        AIPlayerPlatform = _aiController.GetThisPlatform();

        HumanPlayerPoints = 0;
        AIPlayerPoints = 0;

        HumanAttackablePlatforms = _humanController.GetAttackablePlatforms();
        AIAttackablePlatforms = _aiController.GetAttackablePlatforms();
    }

    private void Update()
    {
        HumanPlayerPoints = _humanProps.GetPointsScored();
        AIPlayerPoints = _aiProps.GetPointsScored();

        HumanPlayerPlatform = _humanController.GetThisPlatform();
        AIPlayerPlatform = _aiController.GetThisPlatform();

        HumanAttackablePlatforms = _humanController.GetAttackablePlatforms();
        AIAttackablePlatforms = _aiController.GetAttackablePlatforms();
    }

    public bool HumanPlayerCanBeAttacked()
    {
        return AIAttackablePlatforms.EitherSatisfy((platform) => { return platform == HumanPlayerPlatform; });
    }

    public bool AIPlayerCanBeAttacked()
    {
        return HumanAttackablePlatforms.EitherSatisfy((platform) => { return platform == AIPlayerPlatform; });
    }

    public bool AICanMoveLeftSafe()
    {
        return HumanAttackablePlatforms.Left != Utils.GetPlatformLeftOf(AIPlayerPlatform);
    }

    public bool AICanMoveRightSafe()
    {
        return HumanAttackablePlatforms.Right != Utils.GetPlatformRightOf(AIPlayerPlatform);
    }

    public bool HumanCanMoveLeftSafe()
    {
        return HumanAttackablePlatforms.Left != Utils.GetPlatformLeftOf(HumanPlayerPlatform);
    }

    public bool HumanCanMoveRightSafe()
    {
        return HumanAttackablePlatforms.Right != Utils.GetPlatformRightOf(HumanPlayerPlatform);
    }

    public bool AIAttackLeftSuccess()
    {
        return AIAttackablePlatforms.Left == HumanPlayerPlatform;
    }

    public bool AIAttackRightSuccess()
    {
        return AIAttackablePlatforms.Right == HumanPlayerPlatform; 
    }

    public bool HumanAttackLeftSuccess()
    {
        return HumanAttackablePlatforms.Left == AIPlayerPlatform;
    }

    public bool HumanAttackRightSuccess()
    {
        return HumanAttackablePlatforms.Right == AIPlayerPlatform;
    }

    public bool AIIsInLead()
    {
        return AIPlayerPoints > HumanPlayerPoints;
    }

    public bool PlayersAreTied()
    {
        return AIPlayerPoints == HumanPlayerPoints;
    }
}