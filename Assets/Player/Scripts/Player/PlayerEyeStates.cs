
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
    
    private static void ChangeEyePosition(float eyePositionY)
    {
        topEyelidPosition.y = eyePositionY;
        bottomEyelidPosition.y = -eyePositionY;
        topEyelid.offsetMin = topEyelidPosition;
        bottomEyelid.offsetMax = bottomEyelidPosition;
    }
    
    public class OpenEyeState : IState      // 눈을 완전히 뜬 상태
    {
        public void Enter()
        {
            ChangeEyePosition(PlayerEyeControl.EYE_POSITION_MAX_Y);
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
            ChangeEyePosition(playerEyeControl.GetChangedEyePosition());
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
            ChangeEyePosition(0f);
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
            ChangeEyePosition(playerEyeControl.GetChangedEyePosition());
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
            int blinkDelay = 7;

            do
            {
                playerEyeControl.UpdateBlinkCount(true);
                ChangeEyePosition(playerEyeControl.GetChangedEyePosition());
                await UniTask.Delay(blinkDelay);
            } while (playerEyeControl.GetEyeLittleBlinkCount() != PlayerEyeControl.LITTLE_BLINK_COUNT_MAX);
            
            await UniTask.Delay(3);
            
            do
            {
                playerEyeControl.UpdateBlinkCount(false);
                ChangeEyePosition(playerEyeControl.GetChangedEyePosition());
                await UniTask.Delay(blinkDelay);
            } while (playerEyeControl.GetEyeLittleBlinkCount() != PlayerEyeControl.LITTLE_BLINK_COUNT_MIN);
            
            playerEyeControl.ChangeEyeState(PlayerEyeStateTypes.Open);
        }

        public void Execute()
        {
        }

        public void Exit()
        {
        }
    }
    

    

}