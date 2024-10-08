using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerOpenWorld : MonoBehaviour
{
    public float speed = 3f;

    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private FloatingJoystick _joystick;

    private Transform _orbit;
    private Animator _animator;

    void Start()
    {
        _orbit = transform.GetChild(0).transform;
        _animator = transform.GetChild(0).GetChild(0).transform.GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        Vector3 direction = Quaternion.Euler(0f, Camera.main.transform.eulerAngles.y, 0f) * new Vector3(_joystick.Horizontal * speed, _rigidbody.velocity.y, _joystick.Vertical * speed);
        _rigidbody.velocity = direction;

        if (_joystick.Horizontal != 0 || _joystick.Vertical != 0)
        {
            _orbit.rotation = Quaternion.Slerp(_orbit.rotation, Quaternion.LookRotation(direction), 0.15f);
            _animator.SetBool("isMoving", true);
        }
        else
        {
            _animator.SetBool("isMoving", false);
        }
    }
}
