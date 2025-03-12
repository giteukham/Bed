
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class InsideNavigationBar : MonoBehaviour
{
    public void SetNavigationBarActive(bool active)
    {
        gameObject?.SetActive(active);
    }
}
