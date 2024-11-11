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

        //창모드일때는 모니터 크기 의미 없으므로 모든 선택지 활성화
        /*if (isFullScreen == false)
        {
            item.interactable = true;
            return;
        }*/

        //StartCoroutine(ComparePixelSize());
    }

    private IEnumerator ComparePixelSize()
    {
        //바로 실행하면 itemLabel.text를 했을때 "Option A"라는 글자를 받아오기때문에 한프레임 대기 후 코드 실행
        yield return null;

        //만약 전체화면인 동시에 해상도 선택지의 width, height가 사용자의 모니터 규격을 벗어난다면
        //선택지 비활성화
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
