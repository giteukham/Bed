using System;
using UnityEngine;

// �ش� Ŭ������ ��� �ִ� ������Ʈ�� ������ �� Ȱ��ȭ �Ǿ� ������ ���� ��.
public class MouseWindowUI : MonoBehaviour, IWindowUIBase
{
    public static event Action OnScreenActive, OnScreenDeactive;

    private void Start()
    {
        OnScreenActive?.Invoke();
    }
    
    private void OnDisable()
    {
        OnScreenDeactive?.Invoke();
    }
}
