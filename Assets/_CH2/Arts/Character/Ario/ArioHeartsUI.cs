using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ArioHeartsUI : MonoBehaviour
{
    [SerializeField] private GameObject[] children;
    
    public void ChangeHeartUI(int count)
    {
        switch (count)
        {
            case 0:
                children[0].SetActive(false);
                children[1].SetActive(false);
                children[2].SetActive(false);
                break;
            case 1:
                children[0].SetActive(false);
                children[1].SetActive(false);
                children[2].SetActive(true);
                break;
            case 2:
                children[0].SetActive(false);
                children[1].SetActive(true);
                children[2].SetActive(true);
                break;
            case 3:
                children[0].SetActive(true);
                children[1].SetActive(true);
                children[2].SetActive(true);
                break;
        }
    }
}
