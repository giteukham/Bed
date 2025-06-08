using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drawer : MonoBehaviour
{
    [SerializeField] private GameObject openedDrawer, closedDrawer;
    static public bool isOpen = false;
    static private Vector3 drawerPosition;

    void Start()
    {
        isOpen = false;
        drawerPosition = transform.position;
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
        if(isOpen)
            AudioManager.Instance.PlayForce(AudioKeys.DrawerOpen, drawerPosition);
        else
            AudioManager.Instance.PlayForce(AudioKeys.DrawerClose, drawerPosition);
    }

    static public void OpenNoSound(bool _isOpen)
    {
        if (isOpen == _isOpen) return;
        isOpen = _isOpen;
    }
}
