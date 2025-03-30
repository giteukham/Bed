
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

public delegate void StateActionHandler(MarkovState state);

public class MarkovState : IEquatable<MarkovState>
{
    public string name;

    public event StateActionHandler OnStateAction;
    
    public bool Equals(MarkovState other)
    {
        return name == other.name;
    }

    public override bool Equals(object obj)
    {
        return obj is MarkovState other && Equals(other);
    }

    public override int GetHashCode()
    {
        return name != null ? name.GetHashCode() : 0;
    }

    public void Active()
    {
        if (OnStateAction == null)
        {
            Debug.LogWarning("OnStateAction이 null입니다.");
            return;
        }
        
        OnStateAction(this);
    }
}

public class MarkovChain
{
    private Dictionary<MarkovState, Dictionary<MarkovState, int>> transitions = new();

    /// <summary>
    /// 특정 상태에 등록되어 있는 모든 전이 확률을 반환합니다.
    /// </summary>
    /// <param name="state"></param>
    public int[] this[MarkovState state]
    {
        get { return transitions.TryGetValue(state, out var t) ? t.Values.ToArray() : null; }
    }

    /// <summary>
    /// 상태 전이 정보 등록 함수
    /// to 매개변수 갯수와 probabilities의 갯수를 동일하게 맞춰야 합니다.
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <param name="probabilities">전이 확률</param>
    public void InsertTransition(MarkovState from, MarkovState[] to, int[] probabilities)
    {
        if (to.Length != probabilities.Length)
        {
            Debug.LogWarning("상태와 확률의 개수가 일치하지 않습니다.");
            return;
        }
        
        if (!transitions.ContainsKey(from))
        {
            transitions[from] = new Dictionary<MarkovState, int>();
        }
            
        for (int i = 0; i < to.Length; i++)
        {
            transitions[from][to[i]] = probabilities[i];
        }
    }

    public void RemoveTransition(MarkovState from, MarkovState to)
    {
        if (transitions.TryGetValue(from, out var toDict))
        {
            toDict.Remove(to);
        }
        else
        {
            Debug.LogWarning("삭제하려고 하는 상태가 존재하지 않습니다.");
        }
    }

    /// <summary>
    /// 등록된 전이 확률이 probability보다 작거나 같으면 다음 상태로 전이합니다.
    /// </summary>
    /// <param name="curr"></param>
    /// <param name="probability"></param>
    /// <returns>다음 상태</returns>
    public MarkovState GetNextState(MarkovState curr, int probability)
    {
        if (!transitions.ContainsKey(curr))
        {
            Debug.LogWarning($"상태 {curr.name}에 대한 전이가 정의되지 않았습니다.");
            return curr;
        }
        
        var next = curr;
        foreach (var (state, prob) in transitions[curr])
        {
            if (probability >= prob)
            {
                next = state;
            }
        }
        
        next.Active();
        return next;
    }

    public MarkovState GetCurrentState(MarkovState curr)
    {
        if (!transitions.ContainsKey(curr))
        {
            Debug.LogWarning($"상태 {curr.name}에 대한 전이가 정의되지 않았습니다.");
            return curr;
        }
        
        curr.Active();
        return curr;
    }
}