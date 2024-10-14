using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubObjectSwitcher : MonoBehaviour
{
    private List<GameObject> _childObjects;
    void OnEnable()
    {
        InitChildObjects();
    }

    void InitChildObjects()
    {
        _childObjects = new List<GameObject>();
        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject _currentObj = transform.GetChild(i).gameObject;
            if (_currentObj != null)
            {
                _childObjects.Add(_currentObj);
            }
        }
    }

    public void SetActiveSubObject(int Index)
    {
        if (_childObjects == null)
        {
            InitChildObjects();
        }
        for (int i = 0; i < _childObjects.Count; i++)
        {
            GameObject _currentObj = _childObjects[i].gameObject;
            if (_currentObj != null)
            {
                _currentObj.SetActive(i == Index);
            }
        }
    }
}
