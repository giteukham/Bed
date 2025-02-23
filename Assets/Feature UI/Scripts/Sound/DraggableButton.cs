using System.Drawing;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;
using Sequence = DG.Tweening.Sequence;

public class DraggableButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    private RectTransform rectTransform;
    private Canvas canvas;
    private bool isDragging = false;
    private Vector2 offset, movePos, minPosition, maxPosition;
    private float panelWidth, panelHeight, floorWidth, floorHeight, xRatio, yRatio, panelXScale, panelYScale;
    private RectTransform panelRect;
    public GameObject testObject;
    private GameObject testObjectInstance;
    private Transform testObjectParent;
    [SerializeField] private bool isInitalBatch = false; // 생성 되는 거라면 flase, 이미 배치된 것이라면 true


    Sequence sequence;
    [SerializeField] private Image soundWave;
    [SerializeField] private float fadeDuration = 0.1f;
    [SerializeField] private float scaleDuration = 0.2f;

    // 가로 : 아이콘 y == 오브젝트 x,
    // 세로 : 아이콘 x == 오브젝트 z 
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
    }

    private void Start()
    {
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
        RectTransformUtility.ScreenPointToLocalPointInRectangle(panelRect, eventData.position, eventData.pressEventCamera, out Vector2 localMousePos);
        offset = rectTransform.anchoredPosition- localMousePos;

        isDragging = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging || canvas == null) return;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(panelRect as RectTransform, eventData.position, eventData.pressEventCamera, out movePos );

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
        if (sequence != null && sequence.IsPlaying()) sequence.Kill();
        sequence = DOTween.Sequence();
        soundWave.transform.localScale = Vector3.zero;

        AudioManager.Instance.PlayOneShot(AudioManager.Instance.handClap, testObjectInstance.transform.position);
        sequence.Append(soundWave.DOFade(0.05f, fadeDuration));
        sequence.Join(soundWave.transform.DOScale(new Vector3(1.3f, 1.3f, 1f), scaleDuration).SetLoops(1, LoopType.Incremental));
        sequence.Append(soundWave.DOFade(0f, fadeDuration));
        sequence.Play();
    }

    public void OnDestroy()
    {
        Destroy(testObjectInstance);
    }
}
