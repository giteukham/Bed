
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace Bed.UI
{
    public enum CursorType
    {
        Normal,
        DiagonalResize1,
        DiagonalResize2,
        HorizontalResize,
        VerticalResize
    }
    
    public class Cursor : MonoBehaviour
    {
        private static Cursor instance;
        
        [Header("커서 텍스쳐")]
        public Texture2D diagonalResize1;
        public Texture2D diagonalResize2;
        public Texture2D horizontalResize;
        public Texture2D verticalResize;
        
        private CursorType _currentCursorType = CursorType.Normal;
        
        private void Awake()
        {
            instance = this;
        }
        
        public static void SetCursor(CursorType cursorType)
        {
            switch (cursorType)
            {
                case CursorType.Normal:
                    UnityEngine.Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
                    break;
                case CursorType.DiagonalResize1:
                    UnityEngine.Cursor.SetCursor(instance.diagonalResize1, new Vector2(12, 12), CursorMode.Auto);
                    break;
                case CursorType.DiagonalResize2:
                    UnityEngine.Cursor.SetCursor(instance.diagonalResize2, new Vector2(12, 12), CursorMode.Auto);
                    break;
                case CursorType.HorizontalResize:
                    UnityEngine.Cursor.SetCursor(instance.horizontalResize, new Vector2(12, 12), CursorMode.Auto);
                    break;
                case CursorType.VerticalResize:
                    UnityEngine.Cursor.SetCursor(instance.verticalResize, new Vector2(12, 12), CursorMode.Auto);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }    
}

