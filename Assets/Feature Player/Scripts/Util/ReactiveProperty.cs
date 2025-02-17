
using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;

interface IObserver<T>
{
    void Subscribe(T observer);
    void Unsubscribe(T observer);
    void Notify();
}

public class ReactiveProperty<T>
{
    public T Prev;
    public T Next;
    
    List<IObserver<T>> observers = new List<IObserver<T>>();

    public T Value
    {
        get => Value;
        set
        {
            
        }
    }

    public ReactiveProperty(T value)
    {
        Value = value;
    }
    
    
    

    
}
