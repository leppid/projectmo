using TMPro;
using UnityEngine;

public class PlayerWorld : MonoBehaviour
{
    public float speed = 3f;

    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private FloatingJoystick _joystick;

    private Transform _orbit;
    private Animator _animator;
    public Transform _nickname;
    private float movingSeconds = 0;

    void Awake()
    {
        _orbit = transform.GetChild(0).transform;
        _animator = transform.GetChild(0).GetChild(0).GetChild(2).GetComponent<Animator>();
        _nickname = transform.GetChild(1).transform;
    }

    void FixedUpdate()
    {
        Vector3 direction = Quaternion.Euler(0f, Camera.main.transform.eulerAngles.y, 0f) * new Vector3(_joystick.Horizontal * speed, _rigidbody.linearVelocity.y, _joystick.Vertical * speed);
        _rigidbody.linearVelocity = direction;

        if (_joystick.Horizontal != 0 || _joystick.Vertical != 0)
        {
            _orbit.rotation = Quaternion.Slerp(_orbit.rotation, Quaternion.LookRotation(direction), 0.15f);
            float magnitude = _rigidbody.linearVelocity.magnitude;
            _animator.speed = magnitude / speed;

            if (magnitude > 5.0f)
            {
                _animator.SetBool("IsRunning", true);
                _animator.SetBool("IsWalking", false);
            }
            else
            {
                _animator.SetBool("IsRunning", false);
                _animator.SetBool("IsWalking", true);
            }

            movingSeconds += 1 * Time.deltaTime;

            if (movingSeconds > 10f)
            {
                UiManager.instance.HideBottomBar();
            }
        }
        else
        {
            _animator.speed = 1f;
            _animator.SetBool("IsRunning", false);
            _animator.SetBool("IsWalking", false);

            if (movingSeconds > 0)
            {
                UiManager.instance.ShowBottomBar();
                movingSeconds = 0;
            }
        }

        _nickname.transform.rotation = Camera.main.transform.rotation;
    }

    public void UpdateNickname(string text)
    {
        _nickname.GetComponent<TextMeshProUGUI>().text = text;
    }
}
