using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AuraTween;
using CrossyRoad.Audio;
using CrossyRoad.Behaviour;
using CrossyRoad.Input;
using CrossyRoad.Score;
using CrossyRoad.World;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace CrossyRoad.Player
{
    public class PlayerController : MonoBehaviour, CrossyRoadInput.IPlayerInputActions
    {
        // kinda gross but it's gonna take too long to do this nice lol
        public static Action? KillPlayer;
        
        [SerializeField]
        private Animator _playerAnimator = null!;
        
        [SerializeField]
        private Animator _rendererAnimator = null!;

        [SerializeField]
        private Transform _playerTransform = null!;
        
        [SerializeField]
        private Transform _cameraTransform = null!;
        
        [SerializeField]
        private GameObject _explosion = null!;

        [SerializeField]
        private TweenManager _tweenManager = null!;

        [SerializeField]
        private ScoreController _scoreController = null!;

        [SerializeField]
        private LogCollisionController _logCollisionController = null!;

        [SerializeField]
        private AudioPool _audioPool = null!;

        [SerializeField]
        private List<AudioClip> _jumpAudioClips = new();
        
        [SerializeField]
        private List<AudioClip> _notAllowedAudioClips = new();

        private float _jumpDuration = 0.333f / 1.4f;

        private CrossyRoadInput _input = null!;
        private bool _jumping = false;
        private bool _dead = false;
        private bool _onWater = false;

        private int _maxReachedTile = 0;
        private int _currentTile = 0;
        private int _currentZTile = 0;

        [CanBeNull] 
        private Transform _activeLog = null;
        private float _logZOffset = 0;
        
        [SerializeField]
        private int _maxBackwardsJumps = 3;
        
        [SerializeField]
        private int _maxLeftJumps = 3;
        
        [SerializeField]
        private int _maxRightJumps = 3;
        
        private int _jumpTriggerId = Animator.StringToHash("Jump");
        private int _deathTriggerId = Animator.StringToHash("Death");

        private int _finishJumpTriggerId = Animator.StringToHash("FinishMoving");
        private int _jumpAnimationId = Animator.StringToHash("Move");
        // Start is called before the first frame update
        void Start()
        {
            (_input = new CrossyRoadInput()).PlayerInput.Enable();
            _input.PlayerInput.AddCallbacks(this);
            KillPlayer += OnPlayerKilled;
        }

        private async void OnPlayerKilled()
        {
            if (_dead) return;
            Debug.Log("Heavy is dead?");
            _dead = true;
            // move it out of the player so if the player still has velocity it won't move
            _explosion.transform.SetParent(null);
            _explosion.SetActive(true);
            
            await UniTask.Delay(500);
            _rendererAnimator.SetTrigger(_deathTriggerId);
            await UniTask.Delay(1500);
            
            _scoreController.SaveHighScoreIfNeeded();
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        // Update is called once per frame
        void Update()
        {
            /*if (!_dead && _onWater && !_jumping)
            {
                if (_logCollisionController.PlayerIsOnWaterWithoutLog(_currentTile))
                {
                    Debug.Log("Killed on a log!");
                    OnPlayerKilled();
                }
            }*/
            if(!_dead && _onWater && !_jumping && _activeLog != null && _activeLog.gameObject.activeSelf)
            {
                // follow the log!
                _playerTransform.position = new Vector3(_playerTransform.position.x, _playerTransform.position.y, _activeLog.position.z - _logZOffset);
            }
        }

        public void OnJump(InputAction.CallbackContext ctx)
        {
            if (!ctx.performed || _dead) return;
            Move(1, 0).Forget();
        }

        public void OnLeft(InputAction.CallbackContext ctx)
        {
            if (!ctx.performed || _dead) return;
            Move(0, 1).Forget();
        }

        public void OnRight(InputAction.CallbackContext ctx)
        {
            if (!ctx.performed || _dead) return;
            Move(0, -1).Forget();
        }

        public void OnDown(InputAction.CallbackContext ctx)
        {
            if (!ctx.performed || _dead) return;
            Move(-1, 0).Forget();
        }
        
        private void PlayNotAllowedSound()
        {
            _audioPool.Play(_notAllowedAudioClips[Random.Range(0, _jumpAudioClips.Count)]);
        }
        
        private async UniTask Move(int xAdd, int zAdd)
        {
            if (_jumping) return;

            if (_currentZTile + zAdd > _maxLeftJumps || _currentZTile + zAdd < -_maxRightJumps)
            {
                PlayNotAllowedSound();
                return;
            }
            if (_currentTile + xAdd < _maxReachedTile - _maxBackwardsJumps)
            {
                PlayNotAllowedSound();
                return;
            }
            if (ObstacleCoordinatesController.ExistingCoordinates.Contains((_currentTile + xAdd, _currentZTile + zAdd)))
            {
                Debug.Log("FOLLOWING HAS OBJECT:");
                Debug.Log((_currentTile + xAdd, _currentZTile + zAdd));
                PlayNotAllowedSound();
                return;
            }
            
            _jumping = true;
            
            _playerAnimator.SetTrigger(_jumpTriggerId);
            
            _audioPool.Play(_jumpAudioClips[Random.Range(0, _jumpAudioClips.Count)]);
            
            // await UniTask.NextFrame();

            if (_currentTile + xAdd > _maxReachedTile)
            {
                // move camera forward!!
                _tweenManager.Run(_currentTile, _currentTile + 1, _jumpDuration, value => _cameraTransform.localPosition = new Vector3(value, _cameraTransform.localPosition.y, _cameraTransform.localPosition.z), Easer.FastLinear, this);
            }

            var tempIsOnWater = _logCollisionController.PlayerIsOnWater(_currentTile + 1);

            if (!tempIsOnWater && _onWater)
            {
                // transitioning from water to land, need to do some currentTile math to regain our bearings
                _currentZTile = Mathf.Clamp((int)Math.Round(_playerTransform.localPosition.z, 0), -_maxLeftJumps, _maxRightJumps);
                await _tweenManager.Run(
                    new Vector3(_currentTile, 1, _currentZTile), 
                    new Vector3(_currentTile + xAdd, 1, _currentZTile + zAdd), 
                    _jumpDuration, 
                    value => _playerTransform.localPosition = value, 
                    Easer.FastLinear, 
                    this);
            }
            else if (tempIsOnWater && !_onWater)
            {
                // transitioning from land to water, normal movement should be fine
                await _tweenManager.Run(
                    new Vector3(_currentTile, 1, _currentZTile), 
                    new Vector3(_currentTile + xAdd, 1, _currentZTile + zAdd), 
                    _jumpDuration, 
                    value => _playerTransform.localPosition = value, 
                    Easer.FastLinear, 
                    this);
            }
            else if (tempIsOnWater && _onWater)
            {
                // water to water, do some special math
                await _tweenManager.Run(
                    new Vector3(_currentTile, 1, _playerTransform.localPosition.z), 
                    new Vector3(_currentTile + xAdd, 1, _playerTransform.localPosition.z + zAdd), 
                    _jumpDuration, 
                    value => _playerTransform.localPosition = value, 
                    Easer.FastLinear, 
                    this);
            }
            else
            {
                await _tweenManager.Run(
                    new Vector3(_currentTile, 1, _currentZTile), 
                    new Vector3(_currentTile + xAdd, 1, _currentZTile + zAdd), 
                    _jumpDuration, 
                    value => _playerTransform.localPosition = value, 
                    Easer.FastLinear, 
                    this);
            }

            if (_dead) return;
            _currentTile += xAdd;
            _currentZTile += zAdd;
            if (_currentTile > _maxReachedTile) _maxReachedTile = _currentTile;
            Debug.Log($"Current tiles: {_currentTile}, {_currentZTile}");

            _onWater = _logCollisionController.PlayerIsOnWater(_currentTile);
            // kill if on water w/o log
            if (_onWater && _logCollisionController.PlayerIsOnWaterWithoutLog(_currentTile))
            {
                Debug.Log("Killed on a log!");
                OnPlayerKilled();
                return;
            }
            else if (_onWater)
            {
                // we're on a log, bind the player's movement to it
                // hopefully this picks the right log ig
                var firstLog = LogCollisionController.ActiveLogs.First();
                _activeLog = firstLog.transform;
                _logZOffset = firstLog.transform.position.z - _playerTransform.transform.position.z;
            }
            
            _scoreController.UpdateScore(_currentTile);
            _jumping = false;
        }
    }
}
