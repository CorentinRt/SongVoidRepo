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

    [SerializeField] private float _jumpBufferTimer;
    private Coroutine _jumpBufferCoroutine;
    [SerializeField] private float _jumpCoyoteTimer;
    private float _jumpCoyoteCurrentCounter;

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

        // Coyote time
        if (_groundedPlayer)  // Coyote time counter
        {
            _jumpCoyoteCurrentCounter = _jumpCoyoteTimer;
        }
        else
        {
            _jumpCoyoteCurrentCounter -= Time.deltaTime;
        }
        // End coyote time

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
        if (_requestJump && (_groundedPlayer || _jumpCoyoteCurrentCounter > 0f))
        {
            Jump();
        }

        _playerVelocity.y += _gravityValue * deltaTime;
        _characterController.Move(_playerVelocity * deltaTime);
    }
    #region Jump
    private void ReceiveJump()
    {
        if (_requestJump) return;
        if (_jumpBufferCoroutine != null)
        {
            StopCoroutine(_jumpBufferCoroutine);
            _jumpBufferCoroutine = null;
        }

        _jumpBufferCoroutine = StartCoroutine(JumpBufferCoroutine());
    }
    private void Jump()
    {
        Debug.Log("Jump");
        _requestJump = false;
        _playerVelocity.y += Mathf.Sqrt(_jumpHeight * -2.0f * _gravityValue);
        _jumpCoyoteCurrentCounter = 0f;
    }
    private void ResetJumpBuffer()
    {
        if (_jumpBufferCoroutine != null)
        {
            StopCoroutine(_jumpBufferCoroutine);
            _jumpBufferCoroutine = null;
        }
    }
    private IEnumerator JumpBufferCoroutine()
    {
        float currentTime = 0f;

        while (currentTime < _jumpBufferTimer)
        {
            currentTime += Time.deltaTime;

            if (_characterController.isGrounded)
            {
                _requestJump = true;

                yield break;
            }
            else if (_jumpCoyoteCurrentCounter > 0f)   // Coyote Time Detection
            {
                _requestJump = true;

                yield break;
            }

            yield return null;
        }

        ResetJumpBuffer();

        yield return null;
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
