using UnityEngine;

public class StateMachine : MonoBehaviour
{
    private IState currentState, prevState;
    
    public void ChangeState(IState toState, bool canStateOverlap = false)
    {
        if (currentState == toState && !canStateOverlap)
        {
            Debug.LogWarning($"이미 {currentState?.GetType().Name} 상태입니다.");
            return;            
        }

        prevState = currentState ?? null;
        currentState?.Exit();
        
        currentState = toState;
        currentState.Enter();
    }
    
    /// <summary>
    /// 이전 상태로 돌아가는 함수
    /// </summary>
    public void RevertState()
    {
        if (prevState == null)
        {
            Debug.LogWarning("이전 상태가 없습니다.");
            return;
        }
        
        currentState?.Exit();
        
        currentState = prevState;
        currentState.Enter();
    }
    
    
    
    public bool IsCurrentState(IState currentState)
    {
        return this.currentState == currentState;
    }
    
    public bool IsPrevState(IState prevState)
    {
        return this.prevState == prevState;
    }
    
    private void Update()
    {
        currentState?.Execute();
    }

    public override string ToString()
    {
        return $"Current State: {currentState?.GetType().Name}";
    }
}