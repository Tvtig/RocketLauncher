using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    public delegate void FireAction();
    public static event FireAction OnFired;

    public float TestProperty;

    [SerializeField]
    private float _walkingSpeed = 7.5f;
    [SerializeField]
    private float _runningSpeed = 11.5f;
    [SerializeField]
    private float _jumpSpeed = 8.0f;
    [SerializeField]
    private float _gravity = 9.8f;
    [SerializeField]
    private Camera _playerCamera;
    [SerializeField]
    private float _lookSpeed = 2.0f;
    [SerializeField]
    private float _lookXLimit = 45.0f;
    [SerializeField]
    private bool _canMove = true;
    [SerializeField]
    private GameObject _missile;
    [SerializeField]
    private bool _canShoot = false;
    [SerializeField]
    private float _shootMovementDelay = 0.1f;

    private CharacterController _characterController;
    private Vector3 _moveDirection = Vector3.zero;
    private float _rotationX = 0;
    
    private Animator _animator;

    void Start()
    {
        _characterController = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        _canShoot = true;
    }

    void Update()
    {
        // We are grounded, so recalculate move direction based on axes
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);
        // Press Left Shift to run
        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        float curSpeedX = _canMove ? (isRunning ? _runningSpeed : _walkingSpeed) * Input.GetAxis("Vertical") : 0;
        float curSpeedY = _canMove ? (isRunning ? _runningSpeed : _walkingSpeed) * Input.GetAxis("Horizontal") : 0;
        float movementDirectionY = _moveDirection.y;
        _moveDirection = (forward * curSpeedX) + (right * curSpeedY);

        if (Input.GetButton("Jump") && _canMove && _characterController.isGrounded)
        {
            _moveDirection.y = _jumpSpeed;
        }
        else
        {
            _moveDirection.y = movementDirectionY;
        }

        // Apply gravity. Gravity is multiplied by deltaTime twice (once here, and once below
        // when the moveDirection is multiplied by deltaTime). This is because gravity should be applied
        // as an acceleration (ms^-2)
        if (!_characterController.isGrounded)
        {
            _moveDirection.y -= _gravity * Time.deltaTime;
        }

        // Move the controller
        _characterController.Move(_moveDirection * Time.deltaTime);

        // Player and Camera rotation
        if (_canMove)
        {
            _rotationX += -Input.GetAxis("Mouse Y") * _lookSpeed;
            _rotationX = Mathf.Clamp(_rotationX, -_lookXLimit, _lookXLimit);
            _playerCamera.transform.localRotation = Quaternion.Euler(_rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * _lookSpeed, 0);
        }

        if (Input.GetKeyDown(KeyCode.R) && (!_canShoot))
        {
            _animator.SetTrigger("Reload");
            _canShoot = true;
        }

        if (Input.GetButtonDown("Fire1"))
        {
            if (_canShoot)
            {
                StartCoroutine(DelayMovement());
                _animator.SetTrigger("RestUnloaded");
                //Create a new missile object with the position and rotation of the placeholder that will actually move
                Instantiate(_missile, _missile.transform.position, _missile.transform.rotation);
                //Fire event
                OnFired();
            }
        }
    }


    /// <summary>
    /// Defines the amount of induced delay after firing
    /// </summary>
    /// <returns></returns>
    private IEnumerator DelayMovement()
    {
        _canMove = false;
        yield return new WaitForSeconds(_shootMovementDelay);
        _canMove = true;
    }
}
