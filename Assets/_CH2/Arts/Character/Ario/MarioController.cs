using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MarioController : MonoBehaviour
{
    [SerializeField] private Sprite sit;
    private float jumpHeight = 1;
    private float jumpSpeed = 7;
    private bool isJump;
    private bool isTop;
    
    private Vector2 startPos;
    private Animator animator;
    private SpriteRenderer spr;
    private Sprite initSprite;

    private void Start()
    {
        animator = GetComponent<Animator>();
        spr = GetComponent<SpriteRenderer>();
        initSprite = spr.sprite;
        startPos = transform.position;
        ArioManager.instance.onPlay += InitData;
    }

    private void Update()
    {
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
    
    private IEnumerator Jump()
    {
        isJump = true;
        yield return new WaitForSeconds(0.65f);
        isJump = false;
        isTop = false;
        transform.position = startPos;
    }
    
    private IEnumerator Sit()
    {
        animator.enabled = false;
        spr.sprite = sit;
        yield return new WaitForSeconds(0.65f);
        animator.enabled = true;
        spr.sprite = initSprite;
    }

    private void InitData(bool isPlay)
    {
        if (isPlay)
        {
            transform.position = startPos;
            isJump = false;
            isTop = false;
            animator.enabled = true;
            spr.sprite = initSprite;
        }
        else
        {
            animator.enabled = false;
            isJump = false;
            StopAllCoroutines();
        }
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Obstacle") && ArioManager.instance.isPlay)
        {
            var isSit = other.GetComponent<ObstacleBase>().isSitObstacle;
            StartCoroutine(!isSit ? Jump() : Sit());
        }
    }
}
