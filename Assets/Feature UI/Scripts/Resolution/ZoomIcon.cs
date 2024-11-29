using UnityEngine;
using UnityEngine.EventSystems;


public class ZoomIcon : MonoBehaviour, IPointerClickHandler
{
    private ICommand zoomInCommand, zoomOutCommand;
    private ICommand currentCommand;
    
    public void OnPointerClick(PointerEventData eventData)
    {
        currentCommand = currentCommand == zoomInCommand ? zoomOutCommand : zoomInCommand;
        currentCommand?.Execute();
    }
}

public interface ICommand
{
    void Execute();
}

public class ZoomInCommand : ICommand
{
    public void Execute()
    {
        
    }
}

public class ZoomOutCommand : ICommand
{
    public void Execute()
    {
        
    }
}
