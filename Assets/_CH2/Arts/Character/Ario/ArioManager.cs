using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ArioManager : MonoBehaviour
{
    #region instance

    public static ArioManager instance;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    #endregion

    public delegate void OnPlay(bool isPlay);
    public OnPlay onPlay;

    public float gameSpeed = 1;
    public int life = 1;
    public bool isPlay;
    public GameObject startText;

    private void Start()
    {
        StartCoroutine(WaitStart());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) && !isPlay)
            StartGame();
    }
    
    private IEnumerator WaitStart()
    {
        yield return new WaitForSeconds(0.5f);
        StartGame();
    }

    public void StartGame()
    {
        startText.SetActive(false);
        isPlay = true;
        onPlay.Invoke(isPlay);
    }

    public void GameOver()
    {
        startText.SetActive(true);
        isPlay = false;
        onPlay.Invoke(isPlay);
    }
}
