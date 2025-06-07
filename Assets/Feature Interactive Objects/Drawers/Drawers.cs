using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drawers : MonoBehaviour
{
    [SerializeField] private GameObject openedDrawer, closedDrawer;
    static public bool isOpen = false;

    void Start()
    {
        isOpen = false;
    }

    void Update()
    {
        openedDrawer.SetActive(isOpen);
        closedDrawer.SetActive(!isOpen);
    }

    static public void Open(bool _isOpen)
    {
        if (isOpen == _isOpen) return;
        isOpen = _isOpen;
        // 서랍 소리
    }

    static public void OpenNoSound(bool _isOpen)
    {
        if (isOpen == _isOpen) return;
        isOpen = _isOpen;
    }
}
