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
    private Vector2 _inputDir;
    private Vector3 _moveDir;
    private Vector3 _playerVelocity;
    [SerializeField] private float _playerSpeed = 2.0f;

    [Header("Jump")]
    [SerializeField] private float _jumpHeight = 1.0f;
    private float _gravityValue = -9.81f;
    private bool _groundedPlayer;
    private bool _requestJump;

    [Header("Look")]
    [SerializeField] private Camera _playerCamera;
    [SerializeField] private float _lookHorizontalSpeed;
    [SerializeField] private float _lookVerticalSpeed;
    private Vector2 _lookDelta;

    #endregion

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    private void Start()
    {
        _playerController.OnNotifyUpdateMove += ReceiveMove;

        _playerController.OnNotifyJump += ReceiveJump;

        _playerController.OnNotifyUpdateLook += ReceiveLook;
    }
    private void OnDestroy()
    {
        _playerController.OnNotifyUpdateMove -= ReceiveMove;

        _playerController.OnNotifyJump -= ReceiveJump;

        _playerController.OnNotifyUpdateLook -= ReceiveLook;
    }

    private void Update()
    {
        HandleVelocity(Time.deltaTime);
        HandleLook(Time.deltaTime);
    }

    #region Move
    private void ReceiveMove(Vector2 dir)
    {
        dir.Normalize();

        _inputDir = new Vector2(dir.x, dir.y);
        _moveDir = (_characterController.transform.right * dir.x) + (_characterController.transform.forward * dir.y);

    }
    private void HandleVelocity(float deltaTime)
    {
        _groundedPlayer = _characterController.isGrounded;
        if (_groundedPlayer && _playerVelocity.y < 0)
        {
            _playerVelocity.y = 0f;
        }

        _moveDir = (_characterController.transform.right * _inputDir.x) + (_characterController.transform.forward * _inputDir.y);

        _characterController.Move(_moveDir * deltaTime * _playerSpeed);

        if (_moveDir != Vector3.zero)
        {
            gameObject.transform.forward = _moveDir;
        }

        // Makes the player jump
        if (_requestJump && _groundedPlayer)
        {
            Debug.Log("Jump");
            _requestJump = false;
            _playerVelocity.y += Mathf.Sqrt(_jumpHeight * -2.0f * _gravityValue);
        }

        _playerVelocity.y += _gravityValue * deltaTime;
        _characterController.Move(_playerVelocity * deltaTime);
    }
    #region Jump
    private void ReceiveJump()
    {
        Debug.Log("Input jump");
        if (_groundedPlayer)
        {
            _requestJump = true;
        }
    }
    #endregion
    #endregion

    #region Look
    private void ReceiveLook(Vector2 lookDelta)
    {
        _lookDelta = lookDelta;
    }
    private void HandleLook(float deltaTime)
    {
        Vector3 lookRot = new Vector3(0f , _lookDelta.x, 0f);

        Vector3 cameraLookRot = new Vector3(_lookDelta.y, 0f, 0f);

        lookRot *= deltaTime * _lookHorizontalSpeed;
        cameraLookRot *= deltaTime * _lookVerticalSpeed;

        _characterController.transform.Rotate(lookRot);
        _playerCamera.transform.Rotate(-cameraLookRot);
    }

    #endregion
}
