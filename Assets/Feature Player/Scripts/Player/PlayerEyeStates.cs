using Cysharp.Threading.Tasks;
using UnityEngine;
using Bed.Collider;
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
            BlinkEffect.Blink = PlayerEyeControl.BLINK_VALUE_MIN;
            PlayerConstant.isEyeOpen = true;
        }

        public void Execute()
        {
            if(PlayerConstant.isEyeOpen == false) PlayerConstant.isEyeOpen = true;
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
            if(PlayerConstant.isEyeOpen == false) PlayerConstant.isEyeOpen = true;
        }

        public void Exit()
        {
        }
    }
    
    public class CloseEyeState : IState     // ?��?�� ?��?��?�� 감�?? ?��?��
    {
        public void Enter()
        {
            BlinkEffect.Blink = PlayerEyeControl.BLINK_VALUE_MAX;
            PlayerConstant.isEyeOpen = false;
        }

        public void Execute()
        {
            PlayerConstant.EyeClosedCAT += Time.deltaTime;
            PlayerConstant.EyeClosedLAT += Time.deltaTime;
            if(PlayerConstant.isEyeOpen == true) PlayerConstant.isEyeOpen = false;
        }

        public void Exit()
        {
            PlayerConstant.isEyeOpen = true;
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
            float elapsedTime = 0f;
            float currentBlinkValue = BlinkEffect.Blink;

            while (BlinkEffect.Blink < PlayerEyeControl.BLINK_VALUE_MAX)
            {
                elapsedTime += Time.deltaTime;
                BlinkEffect.Blink = Mathf.Lerp(currentBlinkValue, PlayerEyeControl.BLINK_VALUE_MAX, elapsedTime / PlayerConstant.blinkSpeed);
                await UniTask.Yield();
            } 
            playerEyeControl.UpdateEyeState();

            PlayerConstant.EyeBlinkCAT++;
            PlayerConstant.EyeBlinkLAT++;

            await UniTask.Delay(150);
            elapsedTime = 0f;

            while (BlinkEffect.Blink > currentBlinkValue)
            {
                elapsedTime += Time.deltaTime;
                BlinkEffect.Blink = Mathf.Lerp(BlinkEffect.Blink, currentBlinkValue, elapsedTime / PlayerConstant.blinkSpeed);
                await UniTask.Yield();
            } 
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