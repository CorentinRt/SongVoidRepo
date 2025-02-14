using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace CREMOT.SongVoid
{
    public class PlayerController : MonoBehaviour
    {
        #region Fields
        [Header("Input actions")]
        [Space]
        [Header("Move")]
        [SerializeField] private InputActionReference _move;
        public event Action<Vector2> OnNotifyUpdateMove;

        [Header("Look")]
        [SerializeField] private InputActionReference _look;
        public event Action<Vector2> OnNotifyUpdateLook;

        [Header("Jump")]
        [SerializeField] private InputActionReference _jump;
        public event Action OnNotifyJump;

        #endregion


        private void Awake()
        {
            if (_move != null)
            {
                _move.action.started += NotifyUpdateMove;
                _move.action.performed += NotifyUpdateMove;
                _move.action.canceled += NotifyUpdateMove;
            }
            if (_look != null)
            {
                _look.action.started += NotifyUpdateLook;
                _look.action.performed += NotifyUpdateLook;
                _look.action.canceled += NotifyUpdateLook;
            }
            if (_jump != null)
            {
                _jump.action.started += NotifyJump;
            }
        }

        private void OnDestroy()
        {
            if (_move != null)
            {
                _move.action.started -= NotifyUpdateMove;
                _move.action.performed -= NotifyUpdateMove;
                _move.action.canceled -= NotifyUpdateMove;
            }
            if (_look != null)
            {
                _look.action.started -= NotifyUpdateLook;
                _look.action.performed -= NotifyUpdateLook;
                _look.action.canceled -= NotifyUpdateLook;
            }
            if (_jump != null)
            {
                _jump.action.started -= NotifyJump;
            }
        }

        #region Notify Move
        private void NotifyUpdateMove(InputAction.CallbackContext ctx)
        {
            Vector2 moveValue = ctx.ReadValue<Vector2>();
            OnNotifyUpdateMove?.Invoke(moveValue);
        }
        #endregion

        #region Notify Look
        private void NotifyUpdateLook(InputAction.CallbackContext ctx)
        {
            Vector2 lookValue = ctx.ReadValue<Vector2>();
            OnNotifyUpdateLook?.Invoke(lookValue);
        }
        #endregion

        #region Notify Jump
        private void NotifyJump(InputAction.CallbackContext ctx)
        {
            OnNotifyJump?.Invoke();
        }

        #endregion
    }
}
