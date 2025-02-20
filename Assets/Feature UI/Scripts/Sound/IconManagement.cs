using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using FMODUnity;
using Unity.VisualScripting;
using UnityEngine;

public class IconManagement : MonoBehaviour
{
    [SerializeField] private GameObject gimmickIcon, initGimmickIcon;
    [SerializeField] private Transform parentTransform;
    //[SerializeField] private Transform playerHeadPosition, playerBodyPosition;
    //private EventReference[] playerHeadPositionSounds, playerBodyPositionSounds;
    [SerializeField] private List<GameObject> gimmickIconList = new List<GameObject>(8);

    private void Start()
    {
        gimmickIconList.Add(initGimmickIcon);
    }

    public void GimmickIconAdd()
    {
        gimmickIconList.Add(Instantiate(gimmickIcon, parentTransform));

        if(gimmickIconList.Count > 8)
        {
            GimmickIconRemove();
        }
    }

    public void GimmickIconRemove()
    {
        if(gimmickIconList.Count == 0) return;
        Destroy(gimmickIconList[0]);
        gimmickIconList.RemoveAt(0);
    }

    public async void AllGimmickSoundTest()
    {
        foreach (var gimmickIcon in gimmickIconList)
        {
            gimmickIcon.GetComponent<DraggableButton>().GimmickSoundTest();
            int randomTime = Random.Range(0, 180);
            await Task.Delay(randomTime);
        }
    }
}
