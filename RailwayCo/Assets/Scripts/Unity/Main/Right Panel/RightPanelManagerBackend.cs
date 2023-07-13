using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RightPanelManagerBackend : MonoBehaviour
{
   public List<GameObject> GetPlatformListByUnlockStatus()
    {
        List<GameObject> unlockedPlatformList = new List<GameObject>();
        List<GameObject> lockedPlatformList = new List<GameObject>();

        List<GameObject[]> tmp = new List<GameObject[]>();
        tmp.Add(GameObject.FindGameObjectsWithTag("PlatformLR"));
        tmp.Add(GameObject.FindGameObjectsWithTag("PlatformTD"));

        foreach (GameObject[] collection in tmp)
        {
            foreach (GameObject platform in collection)
            {
                if (platform.GetComponent<PlatformManager>().IsPlatformUnlocked)
                {
                    unlockedPlatformList.Add(platform);
                }
                else
                {
                    lockedPlatformList.Add(platform);
                }
            }
        }

        unlockedPlatformList.AddRange(lockedPlatformList);
        return unlockedPlatformList;
    }
}
