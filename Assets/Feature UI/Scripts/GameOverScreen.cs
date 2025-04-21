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
    

    [Tooltip("공백으로 글씨를 나눔 Ex) par ents")]
    [SerializeField]
    private List<string> parentsTexts = new();
    
    [Tooltip("공백으로 글씨를 나눔 Ex) ne ighbor")]
    [SerializeField]
    private List<string> neighborTexts = new();

    private Dictionary<GameObject, TMP_Text[]> textPositions = new();
    private string[] splitTexts = new [] { string.Empty };                                                  // 일단 공백을 기준으로 텍스트를 나눔
    
    private void Initialize()
    {
        Assert.IsNotNull(dTextObject, "GameOverScreen의 dTextObject가 없습니다.");
        Assert.IsNotNull(nTextObject, "GameOverScreen의 nTextObject가 없습니다.");

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
                Assert.IsNotNull(child, $"GameOverScreen의 {textObject.Key.name}의 자식이 없습니다.");
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
            Debug.LogError($"GameOverScreen의 {gimmickName}의 자식이 없습니다.");

        // 테스트
        // yield return gameOverCoroutine = StartCoroutine(ActiveOrDeActiveDText(true));
        // yield return new WaitForSeconds(2f);                                    // TODO: n초 뒤에
        // yield return gameOverCoroutine = StartCoroutine(ActiveOrDeActiveNText(true));
    }

    public IEnumerator ActiveOrDeActiveDText(bool isActive)
    {
        if (gameOverCoroutine != null) StopCoroutine(gameOverCoroutine);
        if (nTextObject.activeSelf) nTextObject.SetActive(false);                   // nTextObject 살아있으면 비활성화
        
        if (isActive)
        {
            var isComplete = false;
            
            dTextObject.SetActive(true);                                            // dTextObject 활성화
            yield return new WaitForSeconds(1f);                                    // 2초 뒤에
            dTextChild.gameObject.SetActive(true);                                  // dText position 활성화
            dTextChild.DOText(splitTexts[0], parentsTextSpeed).onComplete += () =>
            {
                isComplete = true;
            };
            
            yield return new WaitUntil(() => isComplete);
        }
        else
        {
            dTextObject.SetActive(false);
            dTextChild.gameObject.SetActive(false);                                 // dText position 비활성화
        }
    }
    
    public IEnumerator ActiveOrDeActiveNText(bool isActive)
    {
        if (gameOverCoroutine != null) StopCoroutine(gameOverCoroutine);
        if (dTextObject.activeSelf) dTextObject.SetActive(false);                   // dTextObject 살아있으면 비활성화
        
        if (isActive)
        {
            var isComplete = false;
            
            nTextObject.SetActive(true);                                            // nTextObject 활성화
            yield return new WaitForSeconds(1f);                                    // 2초 뒤에
            nTextChild.gameObject.SetActive(true);                                  // nText position 활성화
            nTextChild.DOText(splitTexts[1], neighborTextSpeed).onComplete += () =>
            {
                isComplete = true;
            };
            yield return new WaitUntil(() => isComplete);
        }
        else
        {
            nTextObject.SetActive(false);
            nTextChild.gameObject.SetActive(false);                                 // nText position 비활성화
        }
    }
}
