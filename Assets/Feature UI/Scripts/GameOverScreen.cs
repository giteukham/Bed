using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;

public class GameOverScreen : MonoBehaviour
{
    [SerializeField]
    private GameObject dTextObject;
    
    [SerializeField]
    private GameObject nTextObject;
    
    private TMP_Text dTextChild = null, nTextChild = null;

    [Space]
    [SerializeField]
    private float parentsTextSpeed = 1f;

    [SerializeField]
    private float neighborTextSpeed = 1f;
    

    [Tooltip("�������� �۾��� ���� Ex) par ents")]
    [SerializeField]
    private List<string> parentsTexts = new();
    
    [Tooltip("�������� �۾��� ���� Ex) ne ighbor")]
    [SerializeField]
    private List<string> neighborTexts = new();

    private Dictionary<GameObject, TMP_Text[]> texts = new();
    private string[] splitTexts = new [] { string.Empty };                                                  // �ϴ� ������ �������� �ؽ�Ʈ�� ����

    private void Awake()
    {
        Initialize();
    }

    private void Initialize()
    {
        if (dTextObject.activeSelf) dTextObject.SetActive(false);
        if (nTextObject.activeSelf) nTextObject.SetActive(false);

        if (texts.Count > 0) texts.Clear(); 
        texts.Add(dTextObject, new TMP_Text[]
        {
            dTextObject.transform.GetChild(0).GetComponent<TMP_Text>(),       // parents position
            dTextObject.transform.GetChild(1).GetComponent<TMP_Text>(),       // neighbor position
        });
        
        texts.Add(nTextObject, new TMP_Text[]
        {
            nTextObject.transform.GetChild(0).GetComponent<TMP_Text>(),
            nTextObject.transform.GetChild(1).GetComponent<TMP_Text>(),
        });
        
        foreach (var textObject in texts)
        {
            foreach (var child in textObject.Value)
            {
                Assert.IsNotNull(child, $"GameOverScreen�� {textObject.Key.name}�� �ڽ��� �����ϴ�.");
                if (child.gameObject.activeSelf) child.gameObject.SetActive(false);
            }
        }
    }
    
    /// <summary>
    /// gimmickName�� ���� �ؽ�Ʈ�� �������� �ٲ� 
    /// </summary>
    /// <param name="isActive"></param>
    /// <param name="gimmickName">�⺻ null</param>
    /// <returns></returns>
    public IEnumerator ControlDText(bool isActive, string gimmickName)
    {
        if (isActive)
        {
            if (nTextObject.activeSelf) nTextObject.SetActive(false);                   // nTextObject ��������� ��Ȱ��ȭ

            ChangeGameOverTextAtRandom(gimmickName);
            
            dTextObject.SetActive(true);
            yield return new WaitForSeconds(1f);
            dTextChild.gameObject.SetActive(true);
            
            StartCoroutine(TypingDText());
        }
        else
        {
            dTextObject.SetActive(false);
            dTextChild.gameObject.SetActive(false);
            StopCoroutine(TypingDText());
        }
    }

    /// <summary>
    /// ���� ��ũ������ D �ڽ� �ؽ�Ʈ�� �������� ����
    /// </summary>
    /// <returns></returns>
    private IEnumerator TypingDText()
    {
        if (!string.IsNullOrEmpty(dTextChild.text))
        {
            dTextChild.text = "";
        }
        yield return new DOTweenCYInstruction.WaitForCompletion(dTextChild.DOText(splitTexts[0], parentsTextSpeed));
    }
    
    /// <summary>
    /// gimmickName�� ���� �ؽ�Ʈ�� �������� �ٲ� 
    /// </summary>
    /// <param name="isActive"></param>
    /// <param name="gimmickName">�⺻ null</param>
    /// <returns></returns>
    public IEnumerator ControlNText(bool isActive, string gimmickName)
    {
        if (isActive)
        {
            if (dTextObject.activeSelf) dTextObject.SetActive(false);                   // dTextObject ��������� ��Ȱ��ȭ
            
            ChangeGameOverTextAtRandom(gimmickName);

            nTextObject.SetActive(true);
            yield return new WaitForSeconds(1f);
            nTextChild.gameObject.SetActive(true);
            
            StartCoroutine(TypingNText());
        }
        else
        {
            nTextObject.SetActive(false);
            nTextChild.gameObject.SetActive(false);
            StopCoroutine(TypingNText());
        }
    }
    
    private IEnumerator TypingNText()
    {
        if (!string.IsNullOrEmpty(nTextChild.text))
        {
            nTextChild.text = "";
        }
        yield return new DOTweenCYInstruction.WaitForCompletion(nTextChild.DOText(splitTexts[1], parentsTextSpeed));
    }
    
    /// <summary>
    /// ��� �̸��� ���� ���� �ؽ�Ʈ�� �������� ��ȭ��Ű�� �Լ�
    /// </summary>
    private void ChangeGameOverTextAtRandom(string gimmickName)
    {
        switch (gimmickName)
        {
            case "Parents":
            {
                var parentsText = parentsTexts[UnityEngine.Random.Range(0, parentsTexts.Count)];
                splitTexts = parentsText.Split(' ');
                dTextChild = texts[dTextObject][0];
                nTextChild = texts[nTextObject][0];
                break;
            }
            case "Neighbor":
            {
                var neighborText = neighborTexts[UnityEngine.Random.Range(0, neighborTexts.Count)];
                splitTexts = neighborText.Split(' ');
                dTextChild = texts[dTextObject][1];
                nTextChild = texts[nTextObject][1];                                 
                break;
            }
        }
        
        if (!dTextChild || !nTextChild)
            Debug.LogError($"GameOverScreen�� {gimmickName}�� �ڽ��� �����ϴ�.");
    }
}
