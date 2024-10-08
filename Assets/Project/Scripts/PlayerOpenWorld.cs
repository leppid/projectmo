using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerOpenWorld : MonoBehaviour
{

    public float speed = 3f;
    
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private FloatingJoystick _joystick;

    private Animator _animator;

    float initialHitDistance;

    // Start is called before the first frame update
    void Start()
    {
        RaycastHit hit = RaycastDown();
        initialHitDistance = hit.distance;
        _animator = transform.GetChild(0).GetChild(0).transform.GetComponent<Animator>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        Vector3 direction = Quaternion.Euler(0f, Camera.main.transform.eulerAngles.y, 0f) * new Vector3(_joystick.Horizontal * speed, _rigidbody.velocity.y, _joystick.Vertical * speed);   
        _rigidbody.velocity = direction;

        if (_joystick.Horizontal != 0 || _joystick.Vertical != 0)
        {
            transform.GetChild(0).transform.rotation = Quaternion.Slerp(transform.GetChild(0).transform.rotation, Quaternion.LookRotation(direction), 0.15f);
            _animator.SetBool("isMoving", true);
        } 
        else
        {
            _animator.SetBool("isMoving", false);
        }
    }

    void MonitorGroundBelow()
    {
        RaycastHit hit = RaycastDown();
        
        if (initialHitDistance > hit.distance)
        {
            this.transform.position = new Vector3(this.transform.position.x,  this.transform.position.y + (hit.point.y * 5f) * Time.deltaTime, this.transform.position.z);
        } 
        else if (initialHitDistance < hit.distance)
        {
            this.transform.position = new Vector3(this.transform.position.x,  this.transform.position.y - (hit.point.y * 5f) * Time.deltaTime, this.transform.position.z);
        }
    }

    RaycastHit RaycastDown()
    {
        RaycastHit hit;
        Ray downRay = new Ray(this.transform.position, -Vector3.up);

        Physics.Raycast(downRay, out hit);

        return hit;
    }
}
