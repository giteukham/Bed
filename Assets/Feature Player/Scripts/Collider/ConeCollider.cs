
using System;
using UnityEditor;
using UnityEngine;
namespace Bed.Collider
{
    public class ConeCollider : MonoBehaviour
    {
        [SerializeField, Tooltip("가로")] private float horizontal = 8f;
        [SerializeField, Tooltip("세로")] private float vertical = 3f;
        [SerializeField, Tooltip("Z축 거리")] private float distance = 1f;
        [SerializeField, Range(4f, 100f), Tooltip("각짐 정도")] private int epllipseSegments;
        [SerializeField, Tooltip("디버그 이미지")] private GameObject debugImage;
        private Vector2 debugImageSize;
        private MeshCollider coneCollider;

        //TODO: Trigger Enter, Exit 구현
        private void OnTriggerEnter(UnityEngine.Collider other)
        {

        }

        private void OnTriggerExit(UnityEngine.Collider other)
        {

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
        public void SetColider(float value)
        {
            debugImageSize = debugImage.GetComponent<RectTransform>().sizeDelta; // 디버그 이미지 초기화

            if (!gameObject.activeSelf) gameObject.SetActive(true);

            if (value < 0.5) horizontal = 8 * (1 - value);

            if (1 - value * 2 > 0) vertical = 3 * (1 - value * 2);
            else vertical = 3 * (1 - value);

            debugImageSize = new Vector2(horizontal * 135, vertical * 130); // 디버그 이미지 사이즈 조절

            if (value <= 0.001f)
            {
                horizontal = 8f;
                vertical = 3f;
                debugImageSize = new Vector2(1080, 390); // 디버그 이미지 사이즈 조절
            }
            else if (value == 1)
            {
                gameObject.SetActive(false);
            }
            debugImage.GetComponent<RectTransform>().sizeDelta = debugImageSize; // 디버그 이미지 사이즈 조절

            OnValidate();
        }
    }
}