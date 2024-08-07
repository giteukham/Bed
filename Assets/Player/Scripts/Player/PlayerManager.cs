using UnityEngine;
using UnityEngine.Serialization;

public class PlayerManager : MonoSingleton<PlayerManager>
{
    [Header("State Machine")]
    public StateMachine playerTurnStateMachine;

    private float stressGauge = 0f, fearGauge = 0f;
    private float stressGaugeMax = 100f, fearGaugeMax = 100f;
    
}
