using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    InputAction movement;
    NewControls controls;
    
    void Awake()
    {
        controls = new NewControls();
        movement = controls.Player.Move;
    }
    
    void OnEnable()
    {
        movement.Enable();
        movement.started += Started;
        movement.performed += Performed;
        movement.canceled += Canceled;
    }
    
    void OnDisable()
    {
        movement.Disable();
        movement.started -= Started;
        movement.performed -= Performed;
        movement.canceled -= Canceled;
    }

    private void FixedUpdate()
    {
        Vector2 move = movement.ReadValue<Vector2>();
        Move(move.x, move.y);
    }

    private void Started(InputAction.CallbackContext obj)
    {
        Debug.Log("Started");
    }
    
    private void Performed(InputAction.CallbackContext obj)
    {
        Debug.Log("Performed");
    }
    
    private void Canceled(InputAction.CallbackContext obj)
    {
        Debug.Log("Canceled");
    }

    private void Move(float x, float y)
    {
        this.transform.position = new Vector2(this.transform.position.x + x, this.transform.position.y + y);
    }
}

