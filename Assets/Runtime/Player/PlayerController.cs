using System.Collections;
using System.Collections.Generic;
using AuraTween;
using CrossyRoad.Input;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

namespace CrossyRoad.Player
{
    public class PlayerController : MonoBehaviour, CrossyRoadInput.IPlayerInputActions
    {
        [SerializeField]
        private Animator _playerAnimator = null!;

        [SerializeField]
        private Transform _playerTransform = null!;
        
        [SerializeField]
        private TweenManager _tweenManager = null!;
        
        private float _jumpDuration = 0.333f;

        private CrossyRoadInput _input = null!;
        private bool _jumping = false;
        private int _currentTile = 0;
        
        private int _jumpTriggerId = Animator.StringToHash("Jump");
        private int _finishJumpTriggerId = Animator.StringToHash("FinishMoving");
        private int _jumpAnimationId = Animator.StringToHash("Move");
        // Start is called before the first frame update
        void Start()
        {
            (_input = new CrossyRoadInput()).PlayerInput.Enable();
            _input.PlayerInput.AddCallbacks(this);
        }

        // Update is called once per frame
        void Update()
        {
            // Debug.Log(_playerVisualsTransform.position);
        }

        public void OnJump(InputAction.CallbackContext ctx)
        {
            if (ctx.performed) return;
            Jump().Forget();
        }

        private async UniTask Jump()
        {
            if (_jumping) return;
            _jumping = true;
            
            Debug.Log("It's jumpin time!");
            _playerAnimator.SetTrigger(_jumpTriggerId);

            await UniTask.NextFrame();
            
            await _tweenManager.Run(new Vector3(_currentTile, 1, 0), new Vector3(_currentTile + 1, 1, 0), _jumpDuration, value => _playerTransform.localPosition = value, Easer.FastLinear, this);
            Debug.Log("done jumpy.");

            _currentTile += 1;
            _jumping = false;
        }
    }
}
