using System;
using System.Collections;
using System.Collections.Generic;
using System.Transactions;
using UnityEngine;

public class Ario : MonoBehaviour
{
    [SerializeField] private float jumpHeight;
    [SerializeField] private float jumpSpeed;
    [SerializeField] private Sprite sit;
    
    private bool isJump;
    private bool isTop;
    private Vector2 startPos;
    private CapsuleCollider2D col;
    private Animator animator;
    private SpriteRenderer spr;
    private Sprite initSprite;
    private int life = 1;
    
    private void Start()
    {
        spr = GetComponent<SpriteRenderer>();
        col = GetComponent<CapsuleCollider2D>();
        animator = GetComponent<Animator>();
        initSprite = spr.sprite;
        startPos = transform.position;
        ArioManager.instance.onPlay += InitData;
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
        else if (Input.GetKey(KeyCode.DownArrow) && !isJump)
        {
            if(col.offset.y == 0)
                col.offset = new Vector2(0, -0.1f);
            animator.enabled = false;
            spr.sprite = sit;
        }
        else if (transform.position.y <= startPos.y)
        {
            animator.enabled = true;
            spr.sprite = initSprite;
            isJump = false;
            isTop = false;
            transform.position = startPos;
            if(col.offset.y != 0)
                col.offset = new Vector2(0, 0);
        }
    }
    
    private void InitData(bool isPlay)
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
            life = 1;
            animator.enabled = false;
            isJump = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Obstacle") && ArioManager.instance.isPlay)
        {
            life--;
            ArioManager.instance.ChangeHeartUI(life);
        }
        else if (other.CompareTag("Coin") && ArioManager.instance.isPlay)
        {
            ArioManager.instance.GetCoin();
            other.gameObject.SetActive(false);
        }
    }
}
