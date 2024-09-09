using Bed.Collider;
using UnityEditor;
using UnityEngine;
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
