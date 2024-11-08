using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResolutionManagement : MonoBehaviour
{

    [SerializeField] Camera cam;
    [SerializeField] Text screenText;

    int temp = 0;
    int nowWidth = 0;
    int nowHeight = 0;

    //const float CRITERIA_NUM = 16 / 9;
    bool change = true;

    private void Awake()
    {
        //cam.clearFlags = CameraClearFlags.SolidColor;
        //cam.backgroundColor = Color.black;
        //GL.Clear(true, true, Color.black);
        //ChangeResolution(32, 9);
        //Hi();
    }

    private void Update()
    {

        //screenText.text = Screen.width + "";
        screenText.text = nowWidth + " : " + nowHeight;

        if (Input.GetKeyDown(KeyCode.N))
        {
            //GL.Clear(true, true, Color.black);
            //print("black");
            Hi();
        }
        else if (Input.GetKeyDown(KeyCode.M))
        {
            Bye();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            print("����");

            switch (temp)
            {
                case 0:
                    temp++;
                    Screen.SetResolution(1440, 1080, change);
                    TestResolution();
                    break;

                case 1:
                    temp++;
                    Screen.SetResolution(1920, 1200, change);
                    TestResolution();
                    break;

                case 2:
                    temp++;
                    Screen.SetResolution(1920, 1080, change);
                    TestResolution();
                    break;

                case 3:
                    temp++;
                    Screen.SetResolution(2560, 1080, change);
                    TestResolution();
                    break;

                case 4:
                    temp = 0;
                    Screen.SetResolution(3840, 1080, change);
                    TestResolution();
                    change = !change;
                    break;
            }

        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="width">'��ǥ' ���κ����� �����</param>
    /// <param name="height">'��ǥ' ���κ����� �����</param>
    private void TestResolution()
    {
        GL.Clear(true, true, Color.black);  // ȭ���� ���������� ����

        float width = Screen.width;
        float height = Screen.height;

        //16 / 9���� ���Ͽ� 16:9 ȭ�鿡�� ���α��� Ȥ�� ���α��� �߿���
        //��� ���̰� �� ���� �˾Ƴ��� �Ǻ��� ����
        float checkedValue = Screen.width / Screen.height;
        float CRITERIA_NUM = 16f / 9f;

        Rect rect = new Rect(0, 0, 1, 1);
        float temp1 = 0;
        //���� ������ 9�� ���� �� ����� ��ǥ ���� ����
        float temp2 = 0;

        //��ǥ ������ 16 : 9�϶�
        if (checkedValue == CRITERIA_NUM)
        {
            cam.rect = rect;
            print("16 : 9");
            return;
        }
        //��ǥ ������ '���� ����'�� 16 : 9���� Ŭ��
        else if (checkedValue > CRITERIA_NUM)
        {
            //��ǥ ���� * X = 9�� ������ ���� X���� ����
            temp1 = 9 / height;
            //������ X�� '��ǥ' ������ ���θ� ���Ͽ� '����' ���κ����� ����
            temp2 = temp1 * width;

            //������ '����'������ ������ ����  (temp2 : 9)

            //16 / '����' ���κ����� ���� rect.width�� ����
            rect.width = 16 / temp2;

            //1 - rect.width�� �� �� 2�� ���� ���� rect.x�� ����(�и� ȭ�� �߾����� �̵�)
            rect.x = (1 - rect.width) / 2;
        }
        //��ǥ ������ '���� ����'�� 16 : 9���� Ŭ��
        else if (checkedValue < CRITERIA_NUM)
        {
            //��ǥ ���� * X = 9�� ������ �� X���� ����
            temp1 = 9 / height;
            //������ X�� '��ǥ' ������ ���θ� ���Ͽ� '����' ���κ����� ����
            temp2 = temp1 * width;

            //������ '����'������ ������ ����  (temp2 : 9)

            //'����' ���κ��� / 16�� ���� rect.height�� ����
            rect.height = temp2 / 16;

            //1 - rect.height�� �� �� 2�� ���� ���� rect.y�� ����(�и� ȭ�� �߾����� �̵�)
            rect.y = (1 - rect.height) / 2;
        }

        //ī�޶� ��������
        cam.rect = rect;
    }

    private void Hi()
    {
        Screen.SetResolution(1440, 1080, true);
        print("dd");
    }

    private void Bye()
    {
        Screen.SetResolution(1920, 1080, true);
        print("dd");
    }

    //���͹ڽ� ���� ���������� ������ ��
    //void OnPreCull() => GL.Clear(true, true, Color.black);

    public void ChangeResolution(float width, float height)
    {
        nowWidth = (int)width;
        nowHeight = (int)height;
        //cam = GetComponent<Camera>();
        //Rect rect = cam.rect;
        Rect rect = new Rect(0, 0, 1, 1);
        float scaleheight = ((float)Screen.width / Screen.height) / ((float)width / height); // (���� / ����)
        float scalewidth = 1f / scaleheight;
        if (scaleheight < 1)
        {
            rect.height = scaleheight;
            rect.y = (1f - scaleheight) / 2f;
        }
        else
        {
            rect.width = scalewidth;
            rect.x = (1f - scalewidth) / 2f;
        }
        cam.rect = rect;

        print(cam.rect);
    }

    //void OnPreCull() => GL.Clear(true, true, Color.black);


    private int ScreenSizeX = 0;
    private int ScreenSizeY = 0;

    private void RescaleCamera(float width, float height)
    {

        //if (Screen.width == ScreenSizeX && Screen.height == ScreenSizeY) return;
        //Camera cam2 = Camera.main;
        //cam2.Render();  // ī�޶� ���� �������� �ٽ� �������ϰ� ��
        GL.Clear(true, true, Color.black);  // ȭ���� ���������� ����
        print("asdfafdd");
        nowWidth = (int)width;
        nowHeight = (int)height;

        //float targetaspect = 16.0f / 9.0f;
        float targetaspect = width / height;
        float windowaspect = (float)Screen.width / (float)Screen.height;
        float scaleheight = windowaspect / targetaspect;
        //Camera camera = GetComponent<Camera>();

        if (scaleheight < 1.0f)
        {
            Rect rect = cam.rect;

            rect.width = 1.0f;
            rect.height = scaleheight;
            rect.x = 0;
            rect.y = (1.0f - scaleheight) / 2.0f;

            cam.rect = rect;
        }
        else // add pillarbox
        {
            float scalewidth = 1.0f / scaleheight;

            Rect rect = cam.rect;

            rect.width = scalewidth;
            rect.height = 1.0f;
            rect.x = (1.0f - scalewidth) / 2.0f;
            rect.y = 0;

            cam.rect = rect;
        }

        ScreenSizeX = Screen.width;
        ScreenSizeY = Screen.height;
    }

    void OnPreCull()
    {
        if (Application.isEditor) return;
        Rect wp = Camera.main.rect;
        Rect nr = new Rect(0, 0, 1, 1);

        Camera.main.rect = nr;
        GL.Clear(true, true, Color.red);

        Camera.main.rect = wp;

    }
}
