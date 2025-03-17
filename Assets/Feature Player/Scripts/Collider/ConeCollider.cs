
using System;
using AbstractGimmick;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
namespace Bed.Collider
{
    public class ConeCollider : MonoBehaviour
    {
        [SerializeField, Tooltip("가로")] private float horizontal;
        [SerializeField, Tooltip("세로")] private float vertical;
        [SerializeField, Tooltip("Z축 거리")] private float distance;
        [SerializeField, Range(4f, 100f), Tooltip("각짐 정도")] private int epllipseSegments;
        [SerializeField, Tooltip("디버그 이미지")] private GameObject debugImage;
        private float debugImageVertical; // 디버그 이미지 Y 초기 값
        private float debugImageVerticalValue;
        private float coliderVerticalValue;
        private MeshCollider coneCollider;
        
        private Vector3 currentScale; // 콜라이더 스케일일
        
        private Action<UnityEngine.Collider> _onEnter, _onStay, _onExit;

        //TODO: Trigger Enter, Exit 구현
        private void OnTriggerEnter(UnityEngine.Collider other)
        {  
            if (other.gameObject.CompareTag("Gimmick"))
            {
                _onEnter?.Invoke(other);
                Debug.Log("Enter");
                if (other.gameObject.TryGetComponent(out Gimmick gimmick))
                {
                    Debug.Log("isDetected = true");
                    gimmick.isDetected = true;
                }
            } 
        }
        
        private void OnTriggerStay(UnityEngine.Collider other)
        {
            if (other.gameObject.CompareTag("Gimmick"))
            {
                _onStay?.Invoke(other);
                if (other.gameObject.TryGetComponent(out Gimmick gimmick))
                {
                    Debug.Log("isDetected = true");
                    gimmick.isDetected = true;
                }
            }
        }
        
        private void OnTriggerExit(UnityEngine.Collider other)
        {
            if (other.gameObject.CompareTag("Gimmick"))
            {
                _onExit?.Invoke(other);
                Debug.Log("Exit");
                if (other.gameObject.TryGetComponent(out Gimmick gimmick))
                {
                    Debug.Log("isDetected = false");
                    gimmick.isDetected = false;
                }
            }
        }
        
        private void Awake()
        {
            coneCollider = this.gameObject.AddComponent<MeshCollider>();
            coneCollider.sharedMesh = CreateConeMesh();
            coneCollider.sharedMesh.RecalculateNormals();
            coneCollider.sharedMesh.RecalculateBounds();
            coneCollider.convex = true;
            coneCollider.isTrigger = true;
            coneCollider.hideFlags = HideFlags.HideInInspector;

            debugImageVertical = vertical * 210;

            coliderVerticalValue = vertical;
            debugImageVerticalValue = debugImageVertical;

            currentScale = coneCollider.transform.localScale;
        }

        // 인스펙터에서 값이 변경될 때마다 호출
        private void OnValidate()
        {
            if (coneCollider != null)
            {
                coneCollider.sharedMesh = CreateConeMesh();
                coneCollider.sharedMesh.RecalculateNormals();
                coneCollider.sharedMesh.RecalculateBounds();
            }
        }

        private Vector3[] GetEpllipseCoordinate()
        {
            Vector3[] epllipsePoints = new Vector3[epllipseSegments + 1];
            Vector3 epllipsePoint = new Vector3(0f, 0f, distance);
            distance = distance < 1.0f ? 1.0f : distance;
            
            epllipsePoints[0] = Vector3.zero;
            for (int i = 1; i < epllipseSegments; i++)
            {
                float angle = ((float)i / (float)epllipseSegments) * 360 * Mathf.Deg2Rad;
                float x = Mathf.Sin(angle) * horizontal;
                float y = Mathf.Cos(angle) * vertical;
                
                epllipsePoint.x = x;
                epllipsePoint.y = y;
                epllipsePoints[i] = epllipsePoint;
            }
            
            return epllipsePoints;
        }
        
        private Mesh CreateConeMesh()
        {
            Vector3[] epllipsePoints = GetEpllipseCoordinate();
            Mesh mesh = new Mesh();
            mesh.Clear();
            
            Vector3[] vertices = new Vector3[epllipseSegments + 1];
            for (int i = 0; i < epllipseSegments; i++)
            {
                vertices[i] = epllipsePoints[i];
            }
            
            int[] triangles = new int[epllipseSegments * 3];
            for (int i = 0; i < epllipseSegments; i++)
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = (i + 1) % epllipseSegments + 1;
            }

            mesh.vertices = vertices;
            mesh.triangles = triangles;

            return mesh;
        }

        public void SetColider()
        {
            if(!gameObject.activeSelf) gameObject.SetActive(true);
            

            if(BlinkEffect.Blink == 0f) 
            {
                vertical = coliderVerticalValue;
            }
            else if(BlinkEffect.Blink == 1)
            {
                gameObject.SetActive(false);
            }
            else if(BlinkEffect.Blink != 0 && BlinkEffect.Blink != 1)
            {
                vertical = coliderVerticalValue * (1 - BlinkEffect.Blink);
            }
                
            currentScale.y = 1 - BlinkEffect.Blink;
            coneCollider.transform.localScale = currentScale;
        }
        
        public void AddEnterListener(Action<UnityEngine.Collider> action) => _onEnter += action;
        public void AddStayListener(Action<UnityEngine.Collider> action) => _onStay += action;
        public void AddExitListener(Action<UnityEngine.Collider> action) => _onExit += action;
        public void RemoveEnterListener(Action<UnityEngine.Collider> action) => _onEnter -= action;
        public void RemoveStayListener(Action<UnityEngine.Collider> action) => _onStay -= action;
        public void RemoveExitListener(Action<UnityEngine.Collider> action) => _onExit -= action;
        public void ResetEnterListener() => _onEnter = null;
        public void ResetStayListener() => _onStay = null;
        public void ResetExitListener() => _onExit = null;

        #if UNITY_EDITOR
        public void SetDebugImage()
        {
            if(!debugImage.GetComponent<Image>().IsActive()) debugImage.GetComponent<Image>().enabled = true;

            if(BlinkEffect.Blink == 0f) 
            {
                 debugImage.GetComponent<RectTransform>().sizeDelta = new Vector2(debugImage.GetComponent<RectTransform>().sizeDelta.x, debugImageVertical); 
            }
            else if(BlinkEffect.Blink == 1)
            {
                 debugImage.GetComponent<Image>().enabled = false;
            }
            else if(BlinkEffect.Blink != 0 && BlinkEffect.Blink != 1)
            {
                debugImage.GetComponent<RectTransform>().sizeDelta = new Vector2(debugImage.GetComponent<RectTransform>().sizeDelta.x, debugImageVerticalValue * (1 - BlinkEffect.Blink));    
            }
        }
        #endif
    }
    
#if UNITY_EDITOR
    [CustomEditor(typeof(ConeCollider))]
    public class ConeColliderEditor : Editor
    {
        private SerializedProperty horizontal;
        private SerializedProperty vertical;
        private SerializedProperty distance;
        private SerializedProperty epllipseSegments;
        private Vector3[] epllipsePoints;
        private ConeCollider coneCollider;
        private SerializedProperty debugImage;
        
        private void OnEnable()
        {
            horizontal = serializedObject.FindProperty("horizontal");
            vertical = serializedObject.FindProperty("vertical");
            distance = serializedObject.FindProperty("distance");
            epllipseSegments = serializedObject.FindProperty("epllipseSegments");
            coneCollider = target as ConeCollider;
            debugImage = serializedObject.FindProperty("debugImage");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            {
                EditorGUILayout.PropertyField(horizontal);
                EditorGUILayout.PropertyField(vertical);
                EditorGUILayout.PropertyField(distance);
                EditorGUILayout.PropertyField(epllipseSegments);
                EditorGUILayout.PropertyField(debugImage);
            }
            serializedObject.ApplyModifiedProperties();
        }

        private void OnSceneGUI()
        {
            if (!EditorApplication.isPlaying)
            {
                distance.floatValue = distance.floatValue < 1.0f ? 1.0f : distance.floatValue;
                epllipsePoints = new Vector3[epllipseSegments.intValue];
                DrawEpllipse();
            }
        }
        
        private void DrawEpllipse()
        {
            Vector3 epllipseCenter = coneCollider.transform.position + coneCollider.transform.TransformDirection(Vector3.forward) * distance.floatValue;
            
            for (int i = 0; i < epllipseSegments.intValue; i++)
            {
                float angle = ((float)i / (float)epllipseSegments.intValue) * 360 * Mathf.Deg2Rad;
                float x = Mathf.Sin(angle) * horizontal.floatValue;
                float y = Mathf.Cos(angle) * vertical.floatValue;
                
                Handles.color = Color.blue;
                epllipseCenter.x = x + coneCollider.transform.position.x;
                epllipseCenter.y = y + coneCollider.transform.position.y;
                epllipsePoints[i] = epllipseCenter;
                Handles.DrawLine(epllipseCenter, coneCollider.transform.position);
            }

            for (int i = 0; i < epllipseSegments.intValue; i++)
            {
                int nextIndex = (i + 1) % epllipseSegments.intValue;
                Handles.DrawLine(epllipsePoints[i], epllipsePoints[nextIndex]);
            }
        }
    }
#endif
}