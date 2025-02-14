using CREMOT.SongVoid;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class PlayerBehavior : MonoBehaviour
{
    #region Fields
    [Header("Controller")]
    [SerializeField] private PlayerController _playerController;
    [SerializeField] private CharacterController _characterController;

    [Header("Move")]
    private Vector3 _moveDir;
    private Vector3 _playerVelocity;
    [SerializeField] private float _playerSpeed = 2.0f;

    [Header("Jump")]
    [SerializeField] private float _jumpHeight = 1.0f;
    private float _gravityValue = -9.81f;
    private bool _groundedPlayer;
    private bool _requestJump;

    #endregion


    private void Start()
    {
        _playerController.OnNotifyUpdateMove += ReceiveMove;

        _playerController.OnNotifyJump += ReceiveJump;
    }
    private void OnDestroy()
    {
        _playerController.OnNotifyUpdateMove -= ReceiveMove;

        _playerController.OnNotifyJump -= ReceiveJump;
    }

    private void FixedUpdate()
    {
        _groundedPlayer = _characterController.isGrounded;
        if (_groundedPlayer && _playerVelocity.y < 0)
        {
            _playerVelocity.y = 0f;
        }

        _characterController.Move(_moveDir * Time.fixedDeltaTime * _playerSpeed);

        if (_moveDir != Vector3.zero)
        {
            gameObject.transform.forward = _moveDir;
        }

        // Makes the player jump
        if (_requestJump && _groundedPlayer)
        {
            _requestJump = false;
            _playerVelocity.y += Mathf.Sqrt(_jumpHeight * -2.0f * _gravityValue);
        }

        _playerVelocity.y += _gravityValue * Time.deltaTime;
        _characterController.Move(_playerVelocity * Time.deltaTime);
    }

    private void ReceiveMove(Vector2 dir)
    {
        dir.Normalize();
        _moveDir = new Vector3(dir.x, 0f, dir.y);
    }

    private void ReceiveJump()
    {
        if (_groundedPlayer)
        {
            _requestJump = true;
        }
    }
}
