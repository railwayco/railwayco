using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RightPanelManager : MonoBehaviour
{
    public void DestroyRightPanelChildren()
    {
        int noChild = this.transform.childCount;
        for (int i =0; i<noChild; i++)
        {
            Destroy(this.transform.GetChild(i).gameObject);
        }
    }
}
