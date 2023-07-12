using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BottomPanelManager : MonoBehaviour
{
    // Tells the camera manager how much bottom space the bottom panel is taking up.
    // Need it for proper camera click "isolation"
    private void Awake()
    {
        GameObject mainUI = GameObject.Find("MainUI");
        Vector2 refReso = mainUI.GetComponent<CanvasScaler>().referenceResolution;
        float bottomPanelHeightRatio = this.GetComponent<RectTransform>().rect.height/ refReso[1];

        CameraManager camMgr = GameObject.Find("CameraList").GetComponent<CameraManager>();
        camMgr.SetBottomPanelHeightRatio(bottomPanelHeightRatio);
    }
}
