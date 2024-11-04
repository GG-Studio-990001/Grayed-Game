using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = System.Random;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private GameObject[] obstacles;
    private List<GameObject> _obstaclePool = new List<GameObject>();
    private int objCnt = 3;

    private void Awake()
    {
        for (int i = 0; i < obstacles.Length; i++)
        {
            for (int q = 0; q < objCnt; q++)
            {
                _obstaclePool.Add(CreateObj(obstacles[i], transform));
            }
        }
    }

    private void Start()
    {
        ArioManager.instance.onPlay += PlayGame;
    }

    private void PlayGame(bool isPlay)
    {
        if (isPlay)
        {
            for (int index = 0; index < _obstaclePool.Count; index++)
            {
                GameObject item = _obstaclePool[index];
                if (item.activeSelf)
                    item.SetActive(false);
            }
            StartCoroutine(CreateObstacle());
        }
        else
        {
            StopAllCoroutines();
        }
    }

    private IEnumerator CreateObstacle()
    {
        yield return new WaitForSeconds(0.5f);
        while (ArioManager.instance.isPlay)
        {
            _obstaclePool[DeactiveObstacle()].SetActive(true);
            yield return new WaitForSeconds(UnityEngine.Random.Range(1f, 3f));
        }
    }

    private int DeactiveObstacle()
    {
        List<int> num = new List<int>();
        for (int i = 0; i < _obstaclePool.Count; i++)
        {
            if(!_obstaclePool[i].activeSelf)
                num.Add(i);
        }

        int x = 0;
        if (num.Count > 0)
        {
            x = num[UnityEngine.Random.Range(0, num.Count)];
        }
        return x;
    }

    private GameObject CreateObj(GameObject obj, Transform parent)
    {
        GameObject copy = Instantiate(obj);
        copy.transform.SetParent(parent);
        copy.SetActive(false);
        return copy;
    }
}
