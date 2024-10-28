using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    private RectTransform rectTransform;
    private Canvas canvas;
    private bool isDragging = false;
    private Vector2 offset, movePos, minPosition, maxPosition;
    private float panelWidth, panelHeight, floorWidth, floorHeight, xRatio, yRatio;
    private RectTransform panelRect;
    public GameObject testObject;
    private GameObject testObjectInstance;
    private Transform testObjectParent;
    private EventReference[] gimmickSounds;
    [SerializeField] private bool isInitalBatch = false; // 생성 되는 거라면 flase, 이미 배치된 것이라면 true

    // 가로 : 아이콘 y == 오브젝트 x,
    // 세로 : 아이콘 x == 오브젝트 z 
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
    }

    private void Start()
    {
        gimmickSounds = new EventReference[]
        {
            AudioManager.instance.catMeow,
            AudioManager.instance.handClap,
            AudioManager.instance.gimmickTest1,
            AudioManager.instance.gimmickTest2,
            AudioManager.instance.gimmickTest3,
            AudioManager.instance.gimmickTest4
        };

        floorWidth = 4f;
        floorHeight = 4f;

        panelRect = transform.parent.GetComponent<RectTransform>();

        minPosition = panelRect.rect.min - rectTransform.rect.min;
        maxPosition = panelRect.rect.max - rectTransform.rect.max;

        panelWidth = panelRect.rect.width;
        panelHeight = panelRect.rect.height;

        xRatio = panelWidth / floorWidth;
        yRatio = panelHeight / floorHeight;

        if (!isInitalBatch)
        {
            float randomX = Random.Range(minPosition.x, maxPosition.x);
            float randomY = Random.Range(minPosition.y, maxPosition.y);
            rectTransform.anchoredPosition = new Vector2(randomX, randomY);

            VolumeSliderManagement volumeSliderManagement = GameObject.Find("Sound Settings Screen").GetComponent<VolumeSliderManagement>();
            GetComponent<MouseWheelHoverEvent>().onScrollUp.AddListener(volumeSliderManagement.UpGimmickVolume);
            GetComponent<MouseWheelHoverEvent>().onScrollDown.AddListener(volumeSliderManagement.DownGimmickVolume);
        }

        testObjectParent= GameObject.Find("Floor").transform;
        testObjectInstance = Instantiate(testObject, testObjectParent);
        testObjectInstance.transform.localPosition = new Vector3((rectTransform.anchoredPosition.y  + panelHeight / 2) / yRatio, 0.5f, (-rectTransform.anchoredPosition.x  +  panelWidth / 2) / xRatio);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, eventData.position, eventData.pressEventCamera, out Vector2 localMousePos);
        offset = rectTransform.anchoredPosition - localMousePos;

        isDragging = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging || canvas == null) return;
        RectTransformUtility.ScreenPointToLocalPointInRectangle( canvas.transform as RectTransform, eventData.position, eventData.pressEventCamera, out movePos );

        Vector2 targetPosition = movePos + offset;

        targetPosition.x = Mathf.Clamp(targetPosition.x, minPosition.x, maxPosition.x);
        targetPosition.y = Mathf.Clamp(targetPosition.y, minPosition.y, maxPosition.y);

        rectTransform.anchoredPosition = targetPosition;
        testObjectInstance.transform.localPosition = new Vector3((targetPosition.y  + panelHeight / 2) / yRatio, 0.5f, (-targetPosition.x  +  panelWidth / 2) / xRatio);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        GimmickSoundTest();
        isDragging = false;
    }

    public void GimmickSoundTest()
    {
        AudioManager.instance.PlayOneShot(AudioManager.instance.handClap, testObjectInstance.transform.position);
    }

    public void OnDestroy()
    {
        Destroy(testObjectInstance);
    }
}
