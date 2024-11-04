using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArioController : MonoBehaviour
{
    [SerializeField] private float jumpHeight;
    [SerializeField] private float jumpSpeed;

    private bool isJump;
    private bool isTop;
    private Vector2 startPos;
    private CapsuleCollider2D col;
    private Animator animator;
    
    private void Start()
    {
        col = GetComponent<CapsuleCollider2D>();
        animator = GetComponent<Animator>();
        startPos = transform.position;
        ArioManager.instance.onPlay += InitPos;
    }

    private void Update()
    {
        if (ArioManager.instance.isPlay)
        {
            KeyEvent();
            
            if (isJump)
            {
                if (transform.position.y <= jumpHeight - 0.1f && !isTop)
                {
                    transform.position = Vector2.Lerp(transform.position,
                        new Vector2(transform.position.x, jumpHeight), jumpSpeed * Time.deltaTime);
                }
                else
                {
                    isTop = true;
                }

                if (transform.position.y > startPos.y && isTop)
                {
                    transform.position = Vector2.MoveTowards(transform.position, startPos, jumpSpeed * Time.deltaTime);
                }
            }
        }
    }

    private void KeyEvent()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            isJump = true;
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            if(col.offset.y == 0)
                col.offset = new Vector2(0, -0.1f);
        }
        else if (transform.position.y <= startPos.y)
        {
            isJump = false;
            isTop = false;
            transform.position = startPos;
            if(col.offset.y != 0)
                col.offset = new Vector2(0, 0);
        }
    }
    
    private void InitPos(bool isPlay)
    {
        if (isPlay)
        {
            transform.position = startPos;
            isJump = false;
            isTop = false;
            animator.enabled = true;
        }
        else
        {
            animator.enabled = false;
            isJump = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Obstacle") && ArioManager.instance.isPlay)
        {
            ArioManager.instance.GameOver();
            animator.enabled = false;
        }
    }
}
