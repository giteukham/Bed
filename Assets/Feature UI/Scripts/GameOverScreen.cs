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
    

    [Tooltip("공백으로 글씨를 나눔 Ex) par ents")]
    [SerializeField]
    private List<string> parentsTexts = new();
    
    [Tooltip("공백으로 글씨를 나눔 Ex) ne ighbor")]
    [SerializeField]
    private List<string> neighborTexts = new();

    private Dictionary<GameObject, TMP_Text[]> texts = new();
    private string[] splitTexts = new [] { string.Empty };                                                  // 일단 공백을 기준으로 텍스트를 나눔

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
                Assert.IsNotNull(child, $"GameOverScreen의 {textObject.Key.name}의 자식이 없습니다.");
                if (child.gameObject.activeSelf) child.gameObject.SetActive(false);
            }
        }
    }
    
    /// <summary>
    /// gimmickName을 통해 텍스트가 랜덤으로 바뀜 
    /// </summary>
    /// <param name="isActive"></param>
    /// <param name="gimmickName">기본 null</param>
    /// <returns></returns>
    public IEnumerator ControlDText(bool isActive, string gimmickName)
    {
        if (isActive)
        {
            if (nTextObject.activeSelf) nTextObject.SetActive(false);                   // nTextObject 살아있으면 비활성화

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
    /// 엔딩 스크린에서 D 자식 텍스트를 랜덤으로 변경
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
    /// gimmickName을 통해 텍스트가 랜덤으로 바뀜 
    /// </summary>
    /// <param name="isActive"></param>
    /// <param name="gimmickName">기본 null</param>
    /// <returns></returns>
    public IEnumerator ControlNText(bool isActive, string gimmickName)
    {
        if (isActive)
        {
            if (dTextObject.activeSelf) dTextObject.SetActive(false);                   // dTextObject 살아있으면 비활성화
            
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
    /// 기믹 이름에 따라 엔딩 텍스트를 랜덤으로 변화시키는 함수
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
            Debug.LogError($"GameOverScreen의 {gimmickName}의 자식이 없습니다.");
    }
}
