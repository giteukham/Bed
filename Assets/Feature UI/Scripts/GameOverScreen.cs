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
    private Coroutine gameOverCoroutine = null;

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

    private Dictionary<GameObject, TMP_Text[]> textPositions = new();
    private string[] splitTexts = new [] { string.Empty };                                                  // �ϴ� ������ �������� �ؽ�Ʈ�� ����
    
    private void Initialize()
    {
        Assert.IsNotNull(dTextObject, "GameOverScreen�� dTextObject�� �����ϴ�.");
        Assert.IsNotNull(nTextObject, "GameOverScreen�� nTextObject�� �����ϴ�.");

        if (dTextObject.activeSelf) dTextObject.SetActive(false);
        if (nTextObject.activeSelf) nTextObject.SetActive(false);

        if (textPositions.Count > 0) textPositions.Clear(); 
        textPositions.Add(dTextObject, new TMP_Text[]
        {
            dTextObject.transform.GetChild(0).GetComponent<TMP_Text>(),       // parents position
            dTextObject.transform.GetChild(1).GetComponent<TMP_Text>(),       // neighbor position
        });
        
        textPositions.Add(nTextObject, new TMP_Text[]
        {
            nTextObject.transform.GetChild(0).GetComponent<TMP_Text>(),
            nTextObject.transform.GetChild(1).GetComponent<TMP_Text>(),
        });
        
        foreach (var textObject in textPositions)
        {
            foreach (var child in textObject.Value)
            {
                Assert.IsNotNull(child, $"GameOverScreen�� {textObject.Key.name}�� �ڽ��� �����ϴ�.");
                if (child.gameObject.activeSelf) child.gameObject.SetActive(false);
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            StartCoroutine(ActiveOrDeActiveDText(false));
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            StartCoroutine(ActiveOrDeActiveNText(false));
        }
    }

    public void Setting(string gimmickName)
    {
        Initialize();
        
        switch (gimmickName)
        {
            case "Parents":
            {
                var parentsText = parentsTexts[UnityEngine.Random.Range(0, parentsTexts.Count)];
                splitTexts = parentsText.Split(' ');
                dTextChild = textPositions[dTextObject][0];                                 // parents position
                nTextChild = textPositions[nTextObject][0];                                 
                break;
            }
            case "Neighbor":
            {
                var neighborText = neighborTexts[UnityEngine.Random.Range(0, neighborTexts.Count)];
                splitTexts = neighborText.Split(' ');
                dTextChild = textPositions[dTextObject][1];                                 // parents position
                nTextChild = textPositions[nTextObject][1];                                 
                break;
            }
        }
        
        if (!dTextChild || !nTextChild)
            Debug.LogError($"GameOverScreen�� {gimmickName}�� �ڽ��� �����ϴ�.");

        // �׽�Ʈ
        // yield return gameOverCoroutine = StartCoroutine(ActiveOrDeActiveDText(true));
        // yield return new WaitForSeconds(2f);                                    // TODO: n�� �ڿ�
        // yield return gameOverCoroutine = StartCoroutine(ActiveOrDeActiveNText(true));
    }

    public IEnumerator ActiveOrDeActiveDText(bool isActive)
    {
        if (gameOverCoroutine != null) StopCoroutine(gameOverCoroutine);
        if (nTextObject.activeSelf) nTextObject.SetActive(false);                   // nTextObject ��������� ��Ȱ��ȭ
        
        if (isActive)
        {
            var isComplete = false;
            
            dTextObject.SetActive(true);                                            // dTextObject Ȱ��ȭ
            yield return new WaitForSeconds(1f);                                    // 2�� �ڿ�
            dTextChild.gameObject.SetActive(true);                                  // dText position Ȱ��ȭ
            dTextChild.DOText(splitTexts[0], parentsTextSpeed).onComplete += () =>
            {
                isComplete = true;
            };
            
            yield return new WaitUntil(() => isComplete);
        }
        else
        {
            dTextObject.SetActive(false);
            dTextChild.gameObject.SetActive(false);                                 // dText position ��Ȱ��ȭ
        }
    }
    
    public IEnumerator ActiveOrDeActiveNText(bool isActive)
    {
        if (gameOverCoroutine != null) StopCoroutine(gameOverCoroutine);
        if (dTextObject.activeSelf) dTextObject.SetActive(false);                   // dTextObject ��������� ��Ȱ��ȭ
        
        if (isActive)
        {
            var isComplete = false;
            
            nTextObject.SetActive(true);                                            // nTextObject Ȱ��ȭ
            yield return new WaitForSeconds(1f);                                    // 2�� �ڿ�
            nTextChild.gameObject.SetActive(true);                                  // nText position Ȱ��ȭ
            nTextChild.DOText(splitTexts[1], neighborTextSpeed).onComplete += () =>
            {
                isComplete = true;
            };
            yield return new WaitUntil(() => isComplete);
        }
        else
        {
            nTextObject.SetActive(false);
            nTextChild.gameObject.SetActive(false);                                 // nText position ��Ȱ��ȭ
        }
    }
}
