
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
    
    public class OpenEyeState : IState      // ���� ������ �� ����
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
    
    public class OpeningEyeState : IState // ���� ���ݾ� �ߴ� ����
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
    
    public class CloseEyeState : IState     // ���� ������ ���� ����
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
    
    public class ClosingEyeState : IState // ���� ���ݾ� ���� ����
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
    
    public class BlinkEyeState : IState     // ���콺 ���� ������ ���� ���Ҵٰ� �ߴ� ����
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