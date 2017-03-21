using System;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerAction
{
    MoveRight,
    MoveLeft,
    AttackLeft,
    AttackRight,
}

public enum Player
{
    One,
    Two,
}

public enum PlatformPosition
{
    One,
    Two,
    Three,
    Four
}

public class PlayerController : MonoBehaviour
{
    // controls whether this player is controlled by a human or ai
    public bool _playerControlled = true;
   
    // which player this controls
    public Player _player;

    // the position the player is on
    public PlatformPosition _currentPosition;

    // offset from ground
    public float yOffset = 1.5f;

    // keys for input
    public KeyCode _leftKey = KeyCode.A;

    public KeyCode _rightKey = KeyCode.D;

    public KeyCode _attackLeftKey = KeyCode.Q;

    public KeyCode _attackRightKey = KeyCode.E;

    public GameObject _attackObjectPrefab;

    public BeatTransformer _beatTransformer;

    public GameController _gameController;

    public GameObject _leftAttackIndicator;

    public GameObject _rightAttackIndicator;

    public Vector3 _attackIndicatorOffset = new Vector3(0, 0, -0.55f);

    public bool IndicatorsActive
    {
        get
        {
            return _leftAttackIndicator.activeInHierarchy && _rightAttackIndicator.activeInHierarchy; 
        }
    }

    // movement functions for log n function lookup
    private Dictionary<PlayerAction, Action> _actionMap = new Dictionary<PlayerAction, Action>();

    // logical structure to deal with ring-like movement
    private Ring<PlatformPosition> _positionRing = new Ring<PlatformPosition>();

    // maps positions to possible positions the player may attack
    private Dictionary<PlatformPosition, Pair<PlatformPosition>> _attackMap = new Dictionary<PlatformPosition, Pair<PlatformPosition>>();

    // to get the game objects from the platform positions
    private Dictionary<Pair<PlatformPosition>, Pair<GameObject>> _positionToGameObjectMap = new Dictionary<Pair<PlatformPosition>, Pair<GameObject>>();

    private Metronome _metronome;

    // make sure the player may not make multiple actions within a beat
    private bool _actionTakenThisBeat = false;

    // used in conjunction with _actionTakenThisBeat
    private int _beat = 0;

    private static bool MapCreated = false;

    private static Dictionary<PlayerAction, string> ActionToStringMap = new Dictionary<PlayerAction, string>();

    public static PlayerAction[] ActionList = { PlayerAction.MoveRight, PlayerAction.MoveLeft, PlayerAction.AttackLeft, PlayerAction.AttackRight };

    private void Awake()
    {
        _leftAttackIndicator.SetActive(false);
        _rightAttackIndicator.SetActive(false);

        if (!MapCreated)
        {
            ActionToStringMap.Add(PlayerAction.MoveLeft, "Moving Left!");
            ActionToStringMap.Add(PlayerAction.MoveRight, "Moving Right!");
            ActionToStringMap.Add(PlayerAction.AttackLeft, "Attacking Left!");
            ActionToStringMap.Add(PlayerAction.AttackRight, "Attacking Right!");
            MapCreated = true;
        }

        // create the ring
        _positionRing.Add(PlatformPosition.One);
        _positionRing.Add(PlatformPosition.Two);
        _positionRing.Add(PlatformPosition.Three);
        _positionRing.Add(PlatformPosition.Four);

        // rotate the ring until the iterator is in the same position as the player
        _positionRing.MoveToFirst();
        while (_positionRing.GetValue() != _currentPosition)
        {
            _positionRing.MoveNext();
        }

        // add the movement functions to the dictionary
        _actionMap.Add(PlayerAction.MoveRight, () =>
        {
            _positionRing.MoveNext();
            MoveToPosition(_positionRing.GetValue());
            _actionTakenThisBeat = true;
            _beat = _metronome.BeatsElapsed;
        });

        _actionMap.Add(PlayerAction.MoveLeft, () =>
        {
            _positionRing.MovePrev();
            MoveToPosition(_positionRing.GetValue());
            _actionTakenThisBeat = true;
            _beat = _metronome.BeatsElapsed;
        });

        // setup the attack positions
        if (_player == Player.One)
        {
            var onePair = new Pair<PlatformPosition>(PlatformPosition.One, PlatformPosition.Four);
            _attackMap.Add(PlatformPosition.One, onePair);
            _positionToGameObjectMap.Add(onePair, new Pair<GameObject>(GameObject.Find("R1"), GameObject.Find("R4")));

            var twoPair = new Pair<PlatformPosition>(PlatformPosition.Three, PlatformPosition.Four);
            _attackMap.Add(PlatformPosition.Two, twoPair);
            _positionToGameObjectMap.Add(twoPair, new Pair<GameObject>(GameObject.Find("R3"), GameObject.Find("R4")));

            var threePair = new Pair<PlatformPosition>(PlatformPosition.Two, PlatformPosition.Three);
            _attackMap.Add(PlatformPosition.Three, threePair);
            _positionToGameObjectMap.Add(threePair, new Pair<GameObject>(GameObject.Find("R2"), GameObject.Find("R3")));

            var fourPair = new Pair<PlatformPosition>(PlatformPosition.One, PlatformPosition.Two);
            _attackMap.Add(PlatformPosition.Four, fourPair);
            _positionToGameObjectMap.Add(fourPair, new Pair<GameObject>(GameObject.Find("R1"), GameObject.Find("R2")));
        }
        if (_player == Player.Two)
        {
            var onePair = new Pair<PlatformPosition>(PlatformPosition.Three, PlatformPosition.Four);
            _attackMap.Add(PlatformPosition.One, onePair);
            _positionToGameObjectMap.Add(onePair, new Pair<GameObject>(GameObject.Find("L3"), GameObject.Find("L4")));

            var twoPair = new Pair<PlatformPosition>(PlatformPosition.Two, PlatformPosition.Three);
            _attackMap.Add(PlatformPosition.Two, twoPair);
            _positionToGameObjectMap.Add(twoPair, new Pair<GameObject>(GameObject.Find("L2"), GameObject.Find("L3")));

            var threePair = new Pair<PlatformPosition>(PlatformPosition.One, PlatformPosition.Two);
            _attackMap.Add(PlatformPosition.Three, threePair);
            _positionToGameObjectMap.Add(threePair, new Pair<GameObject>(GameObject.Find("L1"), GameObject.Find("L2")));

            var fourPair = new Pair<PlatformPosition>(PlatformPosition.Four, PlatformPosition.One);
            _attackMap.Add(PlatformPosition.Four, fourPair);
            _positionToGameObjectMap.Add(fourPair, new Pair<GameObject>(GameObject.Find("L4"), GameObject.Find("L1")));
        }

        _actionMap.Add(PlayerAction.AttackLeft, () =>
        {
            var position = GetPositionFromUtils((_player == Player.One) ? Player.Two : Player.One, _attackMap[_positionRing.GetValue()].Left);
            Instantiate<GameObject>(_attackObjectPrefab, new Vector3(position.x, position.y + 2, position.z), Quaternion.identity);
            _actionTakenThisBeat = true;
        });

        _actionMap.Add(PlayerAction.AttackRight, () =>
        {
            var position = GetPositionFromUtils((_player == Player.One) ? Player.Two : Player.One, _attackMap[_positionRing.GetValue()].Right);
            Instantiate<GameObject>(_attackObjectPrefab, new Vector3(position.x, position.y + 2, position.z), Quaternion.identity);
            _actionTakenThisBeat = true;
        });
    }

    private void Start()
    {
        // put the player in their initial position
        MoveToPosition(_currentPosition);

        // get a reference to the metronome component in the scene
        _metronome = GameObject.Find("Metronome").GetComponent<Metronome>();
    }

    private void Update()
    {
        if (_metronome.Started && !_metronome.Paused)
        {
            if (!IndicatorsActive)
            {
                _leftAttackIndicator.SetActive(true);
                _rightAttackIndicator.SetActive(true);
            }

            ColorPlatforms();
            if (!_actionTakenThisBeat && _playerControlled)
            {
                if (Input.GetKeyDown(_leftKey) && _metronome.WasCalledOnBeat())
                {
                    PerformAction(PlayerAction.MoveLeft);
                }

                else if (Input.GetKeyDown(_rightKey) && _metronome.WasCalledOnBeat())
                {
                    PerformAction(PlayerAction.MoveRight);
                }

                else if (Input.GetKeyDown(_attackLeftKey) && _metronome.WasCalledOnBeat())
                {
                    PerformAction(PlayerAction.AttackLeft);
                }

                else if (Input.GetKeyDown(_attackRightKey) && _metronome.WasCalledOnBeat())
                {
                    PerformAction(PlayerAction.AttackRight);
                }
            }

            if (_metronome.BeatsElapsed > _beat)
            {
                _actionTakenThisBeat = false;
                _beat = _metronome.BeatsElapsed;
            }

            if (_beat > _metronome.BeatsElapsed)
            {
                _beat = _metronome.BeatsElapsed;
            }
        }
    }

    public void PerformAction(PlayerAction action)
    {
        if (_gameController.CountdownFinished)
        {
            _actionMap[action]();
        }
    }

    public void DeactivateArrowIndicators()
    {
        _leftAttackIndicator.SetActive(false);
        _rightAttackIndicator.SetActive(false);
    }

    // color platforms that the player may attack
    private void ColorPlatforms()
    {
        var attackablePlatforms = _positionToGameObjectMap[_attackMap[_positionRing.GetValue()]];
        if (_player == Player.One)
        {
            _beatTransformer.SetNotAttackable(Platform.R1);
            _beatTransformer.SetNotAttackable(Platform.R2);
            _beatTransformer.SetNotAttackable(Platform.R3);
            _beatTransformer.SetNotAttackable(Platform.R4);
        }
        else
        {
            _beatTransformer.SetNotAttackable(Platform.L1);
            _beatTransformer.SetNotAttackable(Platform.L2);
            _beatTransformer.SetNotAttackable(Platform.L3);
            _beatTransformer.SetNotAttackable(Platform.L4);
        }
        
        _beatTransformer.SetAttackable(attackablePlatforms.Left);
        _beatTransformer.SetAttackable(attackablePlatforms.Right);

        if (_player == Player.One)
        {
            _leftAttackIndicator.transform.SetParent(attackablePlatforms.Left.transform);
            _rightAttackIndicator.transform.SetParent(attackablePlatforms.Right.transform);
            
            _leftAttackIndicator.transform.localPosition = _attackIndicatorOffset;
            _rightAttackIndicator.transform.localPosition = _attackIndicatorOffset;

            _leftAttackIndicator.transform.localScale = Vector3.one * (1 / attackablePlatforms.Left.transform.localScale.x);
            _rightAttackIndicator.transform.localScale = Vector3.one * (1 / attackablePlatforms.Right.transform.localScale.x);
        }
    }

    private void MoveTo(Vector3 position, float yOffset)
    {
        transform.position = new Vector3(position.x, position.y + yOffset, position.z);
    }

    private Platform GetAbsolutePlatform(Player player, PlatformPosition position)
    {
        switch (player)
        {
            case Player.One:
                switch (position)
                {
                    case PlatformPosition.One:
                        return Platform.L1;
                    case PlatformPosition.Two:
                        return Platform.L2;
                    case PlatformPosition.Three:
                        return Platform.L3;
                    case PlatformPosition.Four:
                        return Platform.L4;
                }
                break;
            case Player.Two:
                switch (position)
                {
                    case PlatformPosition.One:
                        return Platform.R1;
                    case PlatformPosition.Two:
                        return Platform.R2;
                    case PlatformPosition.Three:
                        return Platform.R3;
                    case PlatformPosition.Four:
                        return Platform.R4;
                }
                break;
        }

        return Platform.L1;
    }

    private void MoveToPosition(PlatformPosition position)
    {
        var vecPosition = Utils.GetPlatformPosition(GetAbsolutePlatform(_player, position));
        MoveTo(vecPosition, yOffset);
    }

    // alias for this long statement
    private Vector3 GetPositionFromUtils(Player player, PlatformPosition position)
    {
        return Utils.GetPlatformPosition(GetAbsolutePlatform(player, position));
    }

    public static string GetActionString(PlayerAction action)
    {
        return ActionToStringMap[action];
    }

    public Platform GetThisPlatform()
    {
        return GetAbsolutePlatform(_player, _positionRing.GetValue());
    }

    public Pair<Platform> GetAttackablePlatforms()
    {
        var player = (_player == Player.One) ? Player.Two : Player.One;
        return new Pair<Platform>(GetAbsolutePlatform(player, _attackMap[_positionRing.GetValue()].Left), GetAbsolutePlatform(player, _attackMap[_positionRing.GetValue()].Right));
    }
}
