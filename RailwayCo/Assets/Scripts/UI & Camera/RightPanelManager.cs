using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Only the Right Panel (under the MainUI) have this script component attached
// All other GameObejcts should find this manager via reference to the RightPanel GameObject.
public class RightPanelManager : MonoBehaviour
{
    [SerializeField] private GameObject cargoPanelPrefab;
   public void resetRightPanel()
    {
        if (!this.gameObject.activeInHierarchy) this.gameObject.SetActive(true);
        DestroyRightPanelChildren();
    }


    private void DestroyRightPanelChildren()
    {
        int noChild = this.transform.childCount;
        for (int i =0; i<noChild; i++)
        {
            Destroy(this.transform.GetChild(i).gameObject);
        }
    }

    // Loads the cargo panel when the train is in the station
    public void loadCargoPanelInStation(GameObject train)
    {
        resetRightPanel();
        GameObject cargoPanel = Instantiate(cargoPanelPrefab);
        cargoPanel.transform.SetParent(this.transform);
        cargoPanel.transform.localPosition = new Vector3(0, 0, 0);
        // TODO: Populate cargo information
        cargoPanel.transform.localScale = new Vector3(1, 1, 1);

        // So that the depart button knows which Train to depart
        cargoPanel.transform.Find("DepartButton").GetComponent<ButtonTrainDepart>().setTrainToDepart(train);
    }
}
