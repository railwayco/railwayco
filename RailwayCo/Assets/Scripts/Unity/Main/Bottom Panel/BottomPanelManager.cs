using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BottomPanelManager : MonoBehaviour
{
    // Start is called before the first frame update
    private void Awake()
    {
        GameObject mainUI = GameObject.Find("MainUI");
        Vector2 refReso = mainUI.GetComponent<CanvasScaler>().referenceResolution;
        float bottomPanelHeightRatio = this.GetComponent<RectTransform>().rect.height/ refReso[1];

        CameraManager camMgr = GameObject.Find("CameraList").GetComponent<CameraManager>();
        camMgr.SetBottomPanelHeightRatio(bottomPanelHeightRatio);
    }
}
