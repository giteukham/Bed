
using Cysharp.Threading.Tasks;
using UnityEngine;
public class PlayerEyeStates
{
    private static PlayerEyeControl playerEyeControl;
    private static RectTransform topEyelid, bottomEyelid;
    private static Vector2 topEyelidPosition = Vector2.zero, bottomEyelidPosition = Vector2.zero;
    
    public PlayerEyeStates(PlayerEyeControl playerEyeControl, RectTransform topEyelid, RectTransform bottomEyelid)
    {
        PlayerEyeStates.playerEyeControl = playerEyeControl;
        PlayerEyeStates.topEyelid = topEyelid;
        PlayerEyeStates.bottomEyelid = bottomEyelid;
    }
    
    public class OpenEyeState : IState      // ���� ������ �� ����
    {
        public void Enter()
        {
            topEyelidPosition.y = PlayerEyeControl.EYE_POSITION_MAX_Y;
            bottomEyelidPosition.y = -PlayerEyeControl.EYE_POSITION_MAX_Y;
            topEyelid.offsetMin = topEyelidPosition;
            bottomEyelid.offsetMax = bottomEyelidPosition;
        }

        public void Execute()
        {

        }

        public void Exit()
        {
        }
    }
    
    public class OpeningEyeState : IState // ���� ���ݾ� �ߴ� ����
    {
        public void Enter()
        {
            float eyePositionY = playerEyeControl.GetChangedEyePosition();
            
            topEyelidPosition.y = eyePositionY;
            bottomEyelidPosition.y = -eyePositionY;
            topEyelid.offsetMin = topEyelidPosition;
            bottomEyelid.offsetMax = bottomEyelidPosition;
        }

        public void Execute()
        {
        }

        public void Exit()
        {
        }
    }
    
    public class CloseEyeState : IState     // ���� ������ ���� ����
    {
        public void Enter()
        {
            topEyelidPosition.y = 0f;
            bottomEyelidPosition.y = 0f;
            topEyelid.offsetMin = topEyelidPosition;
            bottomEyelid.offsetMax = bottomEyelidPosition;
        }

        public void Execute()
        {
        }

        public void Exit()
        {
        }
    }
    
    public class ClosingEyeState : IState // ���� ���ݾ� ���� ����
    {
        public void Enter()
        {
            float eyePositionY = playerEyeControl.GetChangedEyePosition();
            
            topEyelidPosition.y = eyePositionY;
            bottomEyelidPosition.y = -eyePositionY;
            topEyelid.offsetMin = topEyelidPosition;
            bottomEyelid.offsetMax = bottomEyelidPosition;
        }

        public void Execute()
        {
        }

        public void Exit()
        {
        }
    }
    
    public class BlinkEyeState : IState     // ���콺 ���� ������ ���� ���Ҵٰ� �ߴ� ����
    {
        public async void Enter()
        {
            float eyePositionY = playerEyeControl.GetChangedEyePosition();
            float blinkUpdateValue = PlayerEyeControl.EYE_POSITION_MAX_Y / PlayerEyeControl.LITTLE_BLINK_COUNT_MAX;
            
            do
            {
                topEyelidPosition.y = eyePositionY - blinkUpdateValue;
                bottomEyelidPosition.y = -eyePositionY + blinkUpdateValue;
                topEyelid.offsetMin = topEyelidPosition;
                bottomEyelid.offsetMax = bottomEyelidPosition;
                
                await UniTask.Delay(200);
                
            } while (topEyelidPosition.y != 0f && bottomEyelidPosition.y != 0f);
            
            await UniTask.Delay(150);
            
            do
            {
                eyePositionY = playerEyeControl.GetChangedEyePosition();
                topEyelidPosition.y = eyePositionY - blinkUpdateValue;
                bottomEyelidPosition.y = -eyePositionY + blinkUpdateValue;
                topEyelid.offsetMin = topEyelidPosition;
                bottomEyelid.offsetMax = bottomEyelidPosition;
                await UniTask.Delay(200);
            } while (topEyelidPosition.y != PlayerEyeControl.EYE_POSITION_MAX_Y && bottomEyelidPosition.y != PlayerEyeControl.EYE_POSITION_MAX_Y);
        }

        public void Execute()
        {
        }

        public void Exit()
        {
        }
    }
    

    

}