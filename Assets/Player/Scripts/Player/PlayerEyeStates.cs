
using Bed.PostProcessing;
using Cysharp.Threading.Tasks;
using UnityEngine;
public class PlayerEyeStates
{
    private static PlayerEyeControl playerEyeControl;
    private static CustomVignette customVignette;

    public PlayerEyeStates(PlayerEyeControl playerEyeControl, CustomVignette customVignette)
    {
        PlayerEyeStates.playerEyeControl = playerEyeControl;
        PlayerEyeStates.customVignette = customVignette;
    }
    
    public class OpenEyeState : IState      // 눈을 완전히 뜬 상태
    {
        public void Enter()
        {
            customVignette.blink.value = PlayerEyeControl.BLINK_VALUE_MIN;
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
            customVignette.blink.value = PlayerEyeControl.BLINK_VALUE_MAX;
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
            float elapsedTime = 0f;
            while (customVignette.blink.value < PlayerEyeControl.BLINK_VALUE_MAX)
            {
                elapsedTime += Time.deltaTime;
                customVignette.blink.value = Mathf.Lerp(playerEyeControl.mouseBlinkValues[playerEyeControl.mouseCount], PlayerEyeControl.BLINK_VALUE_MAX, elapsedTime / PlayerConstant.eyeOpenCloseInterval);
                await UniTask.Yield();
            } 

            await UniTask.Delay(150);
            elapsedTime = 0f;

            while (customVignette.blink.value > playerEyeControl.mouseBlinkValues[playerEyeControl.mouseCount])
            {
                elapsedTime += Time.deltaTime;
                customVignette.blink.value = Mathf.Lerp(customVignette.blink.value, playerEyeControl.mouseBlinkValues[playerEyeControl.mouseCount], elapsedTime / PlayerConstant.eyeOpenCloseInterval);
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