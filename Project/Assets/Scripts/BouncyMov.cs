using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using Vector3 = UnityEngine.Vector3;

public class BouncyMov : MonoBehaviour {

    private CharacterController _characterController;
    public float speed;
    private float _jumpSpeed;
    private float _gravity;

    void Awake() {
        _characterController = GetComponent<CharacterController>();
    }

    private void Start() {
        speed = 10;
        _jumpSpeed = 100;
        _gravity = 10;
    }

    // Update is called once per frame
    void Update() {
        if (_characterController.isGrounded && Input.GetKey(KeyCode.Space)) {
            Jump();
        }
        else if (!_characterController.isGrounded) {
            Fall();
        }
        
        if (Input.GetKey("w")) {
            MoveForward();
        }
        if (Input.GetKey("a")) {
            MoveLeft();
        }
        if (Input.GetKey("s")) {
            MoveBack();
        }
        if (Input.GetKey("d")) {
            MoveRight();
        }
    }

    private void Fall() {
        _characterController.Move(_gravity * Time.deltaTime * Vector3.down);
    }

    private void Jump() {
        _characterController.Move(_jumpSpeed * Vector3.up);
    }

    private void MoveRight() {
        _characterController.Move(speed * Time.deltaTime * Vector3.right);
    }

    private void MoveBack() {
        _characterController.Move(speed * Time.deltaTime * Vector3.back);
    }

    private void MoveLeft() {
        _characterController.Move(speed * Time.deltaTime * Vector3.left);
    }

    private void MoveForward() {
        _characterController.Move(speed * Time.deltaTime * Vector3.forward);
    }
}
