
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
    
    public class OpenEyeState : IState      // 눈을 완전히 뜬 상태
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
    
    public class OpeningEyeState : IState // 눈을 조금씩 뜨는 상태
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
    
    public class CloseEyeState : IState     // 눈을 완전히 감은 상태
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
    
    public class ClosingEyeState : IState // 눈을 조금씩 감는 상태
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
    
    public class BlinkEyeState : IState     // 마우스 휠을 누르면 눈을 감았다가 뜨는 상태
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
                
                await UniTask.Delay(7);
                eyePositionY = topEyelidPosition.y;
            } while (topEyelidPosition.y != 0f && bottomEyelidPosition.y != 0f);
            
            await UniTask.Delay(3);
            
            do
            {
                topEyelidPosition.y = eyePositionY + blinkUpdateValue;
                bottomEyelidPosition.y = -eyePositionY - blinkUpdateValue;
                topEyelid.offsetMin = topEyelidPosition;
                bottomEyelid.offsetMax = bottomEyelidPosition;
                
                await UniTask.Delay(7);
                eyePositionY = topEyelidPosition.y;
            } while (topEyelidPosition.y != PlayerEyeControl.EYE_POSITION_MAX_Y && bottomEyelidPosition.y != PlayerEyeControl.EYE_POSITION_MAX_Y);
            
            playerEyeControl.ChangeState(PlayerEyeStateTypes.Open);
        }

        public void Execute()
        {
        }

        public void Exit()
        {
        }
    }
    

    

}