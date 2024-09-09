using Cysharp.Threading.Tasks;
using UnityEngine;
public class PlayerEyeStates
{
    private static PlayerEyeControl playerEyeControl;

    public PlayerEyeStates(PlayerEyeControl playerEyeControl)
    {
        PlayerEyeStates.playerEyeControl = playerEyeControl;
    }
    
    public class OpenEyeState : IState      // ?��?�� ?��?��?�� ?�� ?��?��
    {
        public void Enter()
        {
            //customVignette.blink.value = PlayerEyeControl.BLINK_VALUE_MIN;
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
            
        }

        public void Execute()
        {
        }

        public void Exit()
        {
        }
    }
    
    public class CloseEyeState : IState     // ?��?�� ?��?��?�� 감�?? ?��?��
    {
        public void Enter()
        {
            //customVignette.blink.value = PlayerEyeControl.BLINK_VALUE_MAX;
        }

        public void Execute()
        {
            PlayerConstant.EyeClosedCAT += Time.deltaTime;
            PlayerConstant.EyeClosedLAT += Time.deltaTime;
        }

        public void Exit()
        {
        }
    }
    
    public class OpeningEyeState : IState // ���� ���ݾ� �ߴ� ����
    {
        public void Enter()
        {
            
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
            // float elapsedTime = 0f;
            // while (customVignette.blink.value < PlayerEyeControl.BLINK_VALUE_MAX)
            // {
            //     elapsedTime += Time.deltaTime;
            //     customVignette.blink.value = Mathf.Lerp(playerEyeControl.mouseBlinkValues[playerEyeControl.mouseCount], PlayerEyeControl.BLINK_VALUE_MAX, elapsedTime / PlayerConstant.blinkSpeed);
            //     await UniTask.Yield();
            // } 
            // // ������ üũ
            // playerEyeControl.UpdateEyeState();
            //
            // PlayerConstant.EyeBlinkCAT++;
            // PlayerConstant.EyeBlinkLAT++;
            //
            // await UniTask.Delay(150);
            // elapsedTime = 0f;
            //
            // while (customVignette.blink.value > playerEyeControl.mouseBlinkValues[playerEyeControl.mouseCount])
            // {
            //     elapsedTime += Time.deltaTime;
            //     customVignette.blink.value = Mathf.Lerp(customVignette.blink.value, playerEyeControl.mouseBlinkValues[playerEyeControl.mouseCount], elapsedTime / PlayerConstant.blinkSpeed);
            //     await UniTask.Yield();
            // } 
            await UniTask.Yield();

            playerEyeControl.UpdateEyeState();
        }
        
        public void Execute()
        {
            
        }

        public void Exit()
        {
            
        }
    }
    

    

}