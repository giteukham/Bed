
using System;
using Cysharp.Threading.Tasks.Linq;
using Cysharp.Threading.Tasks.Triggers;
using UnityEngine.UI;

public static class ImageExtension
{
    public static void OnDoubleClick(this Image image, Action action)
    {
        image.GetAsyncPointerClickTrigger().Subscribe((data) =>
        {
            if (data.clickCount == 2)
            {
                action();
            }
        });
    }
}
