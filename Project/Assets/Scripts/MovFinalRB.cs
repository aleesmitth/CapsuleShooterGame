using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovFinalRB : MonoBehaviour {

    [SerializeField] public float gravity;
    [SerializeField] public float speed;
    [SerializeField] public float jumpSpeed;
    public Transform playerT;
    private float _height;
    private Rigidbody _rb;
    private Vector3 _direction;
    private float _distToGround;

    // Start is called before the first frame update
    void Start() {
        gravity = 100;
        speed = 8;
        jumpSpeed = .5f;
        _height = GetComponent<CapsuleCollider>().height;
        _rb = GetComponent<Rigidbody>();
        // get the distance to ground
        _distToGround = GetComponent<Collider>().bounds.extents.y;
    }

    // Update is called once per frame
    private void FixedUpdate() {
        // tema flechitas, el normalized hace q no vaya mas rapido en diagonal (porq mete pitagoras y va a vel hipotenusa). Este movimiento se siente muy slippery probar esto  rb.velocity = movement * speed;
        _direction = new Vector3 (Input.GetAxis ("Horizontal"), 0, Input.GetAxis ("Vertical")).normalized;
        _direction = playerT.TransformDirection(_direction);
        _rb.MovePosition (Time.deltaTime * speed * _direction + playerT.position);
        
        // tema gravedad, esto no lo entendi bien, es mucho de documentacion sobre Force, AddForce, etc, la idea es incluir la masa y aceleracion
        // sin el controler no tengo un isGrounded, lo hago con un RayCast
        //if (!IsGrounded()) { 
        /*} else*/ if (IsGrounded() && Input.GetKey(KeyCode.Space)) {
            // tema saltar, raiz 2*g*h es velocidad, similar a lo de gravedad
            _rb.AddForce( jumpSpeed * (float) Math.Sqrt(2 * gravity * _height) * playerT.up, ForceMode.VelocityChange);
        }
        else if (!IsGrounded()) {
            _rb.AddForce(-playerT.up * gravity, ForceMode.Acceleration);
            /*if (Physics.Raycast(transform.position, Vector3.down, _distToGround + .5f)) {
                transform.position = new Vector3(transform.position.x, _distToGround + .1f, transform.position.z);
            }*/
        }
    }
    // me dice si la distancia al piso es menor que distToGround +0.1 (por irregularidades y skin del player)
    bool IsGrounded() {
        return Physics.Raycast(playerT.position, Vector3.down, _distToGround + .2f);
    }
}
