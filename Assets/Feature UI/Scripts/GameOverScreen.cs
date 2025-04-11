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
    
    [Space]
    [SerializeField]
    private float parentsTextSpeed = 1f;

    [SerializeField]
    private float neighborTextSpeed = 1f;
    
    private Dictionary<GameObject, TMP_Text[]> textPositions = new Dictionary<GameObject, TMP_Text[]>();

    [SerializeField]
    private string parentsText = "par ents";
    
    [SerializeField]
    private string neighborText = "neig hbor";

    private void Initialize()
    {
        Assert.IsNotNull(dTextObject, "GameOverScreen�� dTextObject�� �����ϴ�.");
        Assert.IsNotNull(nTextObject, "GameOverScreen�� nTextObject�� �����ϴ�.");
        
        if (dTextObject.activeSelf) dTextObject.SetActive(false);
        if (nTextObject.activeSelf) nTextObject.SetActive(false);
        
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

    public IEnumerator ActiveGameOverScreen(string gimmickName)
    {
        Initialize();
        var splitTexts = new [] { string.Empty };                                            // �ϴ� ������ �������� �ؽ�Ʈ�� ����
        TMP_Text dTextChild = null, nTextChild = null;
        
        switch (gimmickName)
        {
            case "Parents":
                splitTexts = parentsText.Split(' ');
                dTextChild = textPositions[dTextObject][0];                                 // parents position
                nTextChild = textPositions[nTextObject][0];                                 
                break;
            case "Neighbor":
                splitTexts = neighborText.Split(' ');
                dTextChild = textPositions[dTextObject][1];                                 // neighbor position
                nTextChild = textPositions[nTextObject][1];                                 
                break;
        }
        
        if (!dTextChild || !nTextChild)
        {
            Debug.LogError($"GameOverScreen�� {gimmickName}�� �ڽ��� �����ϴ�.");
            yield break;
        }
        
        dTextObject.SetActive(true);                                            // dTextObject Ȱ��ȭ
        yield return new WaitForSeconds(2f);                                    // 2�� �ڿ�
        dTextChild.gameObject.SetActive(true);                                  // dText position Ȱ��ȭ
        dTextChild.DOText(splitTexts[0], parentsTextSpeed);
        
        yield return new WaitForSeconds(2f);                                    // TODO: n�� �ڿ�
        dTextObject.SetActive(false);
        
        nTextObject.SetActive(true);                                            // nTextObject Ȱ��ȭ
        yield return new WaitForSeconds(2f);                                    // 2�� �ڿ�
        nTextChild.gameObject.SetActive(true);                                  // nText position Ȱ��ȭ
        nTextChild.DOText(splitTexts[1], parentsTextSpeed);
    }
}
