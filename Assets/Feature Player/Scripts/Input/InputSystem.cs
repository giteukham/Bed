using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Bed
{
    /// <summary>
    /// 나중에 Player에서 빼고 main에 넣어야 할 것 같음.
    /// </summary>
    public class InputSystem : MonoSingleton<InputSystem>
    {
        private float mouseDeltaX;
        private float mouseDeltaY;
        public float MouseDeltaX
        {
            get { return mouseDeltaX; }
            private set { mouseDeltaX = value; }
        }
    
        public float MouseDeltaY
        {
            get { return mouseDeltaY; }
            private set { mouseDeltaY = value; }
        }
    
        #region Mouse Events
        public event Action OnMouseWheelClickEvent; 
        public event Action<int> OnMouseScrollEvent;         // 마우스 휠. 위로는 120, 아래로는 -120
        public event Action OnMouseClickEvent;               // 마우스 클릭 이벤트
        #endregion
        
        #region Keyboard Events
        public event Action OnEscEvent;                      // ESC 키 이벤트
        #endregion
        private void OnMouseDelta(InputValue value)
        {
            if (PlayerConstant.isPlayerStop) return;
                
            if ( PlayerConstant.isParalysis ) 
            {
                MouseDeltaX = value.Get<Vector2>().x * 0.02f;
                MouseDeltaY = value.Get<Vector2>().y * 0.02f;
            }
            else
            {
                MouseDeltaX = value.Get<Vector2>().x;
                MouseDeltaY = value.Get<Vector2>().y;
            }
        }
    
        private void OnMouseScroll(InputValue value)
        {
            OnMouseScrollEvent?.Invoke(Convert.ToInt32(value.Get<Vector2>().y));
        }
        
        private void OnMouseWheelClick(InputValue value)
        {
            if (value.isPressed)
            {
                OnMouseWheelClickEvent?.Invoke();
            }
        }
        
        private void OnMouseClick(InputValue value)
        {
            if (value.isPressed)
            {
                OnMouseClickEvent?.Invoke();
            }
        }
    }
}
