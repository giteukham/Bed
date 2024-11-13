using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResolutionInteractable : MonoBehaviour
{
    [SerializeField] private SaveManager saveManager;
    [SerializeField] private TMP_Text itemLabel;
    private Toggle item;

    private int monitorWidth = 0;
    private int monitorHeight = 0;

    private int pixelWidth = 0;
    private int pixelHeight = 0;

    private bool isFullScreen = true;

    private void Awake()
    {
        item = GetComponent<Toggle>();
        monitorWidth = Display.main.systemWidth;
        monitorHeight = Display.main.systemHeight;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            print(itemLabel.text);
        }
    }

    private void OnEnable()
    {
        isFullScreen = saveManager.LoadIsFullScreen();

        //â����϶��� ����� ũ�� �ǹ� �����Ƿ� ��� ������ Ȱ��ȭ
        /*if (isFullScreen == false)
        {
            item.interactable = true;
            return;
        }*/

        //StartCoroutine(ComparePixelSize());
    }

    private IEnumerator ComparePixelSize()
    {
        //�ٷ� �����ϸ� itemLabel.text�� ������ "Option A"��� ���ڸ� �޾ƿ��⶧���� �������� ��� �� �ڵ� ����
        yield return null;

        //���� ��üȭ���� ���ÿ� �ػ� �������� width, height�� ������� ����� �԰��� ����ٸ�
        //������ ��Ȱ��ȭ
        string[] parts = itemLabel.text.Split(" X ");
        print(itemLabel.text);
        print(parts[0] + " " + parts[1]);
        pixelWidth = int.Parse(parts[0]);
        pixelHeight = int.Parse(parts[1]);

        if ((pixelWidth > monitorWidth) || (pixelHeight > monitorHeight))
        {
            item.interactable = false;
        }
    }


}
