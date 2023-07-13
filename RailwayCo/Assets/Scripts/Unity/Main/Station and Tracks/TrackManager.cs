using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TrackManager : MonoBehaviour
{
    public int PathCost { get; private set; }
    public bool IsTrackUnlocked { get; private set; }


    ////////////////////////////////////////
    /// INITIALISATION PROCESSES
    ////////////////////////////////////////

    private void Awake()
    {
        CalculatePathCost();
        SetInitialTrackStatus();
        UpdateTrackRender();
    }

    private void CalculatePathCost()
    {
        int numTracks = this.transform.childCount;
        for (int i = 0; i < numTracks; i++)
        {
            Transform child = this.transform.GetChild(i);
            string tagName = child.gameObject.tag;

            switch (tagName)
            {
                case "BridgeTD":
                case "BridgeLR":
                case "Track_LR":
                case "Track_TD":
                    PathCost += 5;
                    break;
                case "Track_Curved_RU":
                case "Track_Curved_RD":
                case "Track_Curved_LU":
                case "Track_Curved_LD":
                    PathCost += 20;
                    break;
                case "SlopeTD":
                case "SlopeLR":
                    PathCost += 15;
                    break;
                default:
                    Debug.LogWarning($"{this.name}: Unhandled tag {tagName} for child {child.name} for the track manager to calculate path cost. Default to value of 5");
                    PathCost += 5;
                    break;

            }
        }
    }

    private void SetInitialTrackStatus()
    {

        // TODO: Query from backend save the particular track
        // If the query fail, we go back to the default values

        string platformConnectionName = this.name;

        if (platformConnectionName == "Platform1_1-Platform6_1")
        {
            Debug.Log($"Setting default active track connection {platformConnectionName}");
            IsTrackUnlocked = true;
        }
        else
        {
            IsTrackUnlocked = false;
        }
    }



    ///////////////////////////////////////
    /// EVENT UPDATES
    ////////////////////////////////////////

    public void UpdateTrackStatus(bool isUnlocked)
    {
        IsTrackUnlocked = isUnlocked;
        UpdateTrackRender();
    }

    private void UpdateTrackRender()
    {
        int numTracks = this.transform.childCount;
        for (int i = 0; i < numTracks; i++)
        {
            Transform child = this.transform.GetChild(i);
            Color trackColor = child.GetComponent<SpriteRenderer>().color;
            Transform minimapMarker = child.Find("MinimapMarker");

            if (IsTrackUnlocked)
            {
                trackColor.a = 1;
                child.GetComponent<SpriteRenderer>().color = trackColor;
                minimapMarker.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1);
            }
            else
            {
                trackColor.a = 0.392f; //(100/255)
                child.GetComponent<SpriteRenderer>().color = trackColor;
                minimapMarker.GetComponent<SpriteRenderer>().color = new Color(0.4f, 0.4f, 0.4f); //0x666666
            }
        }
    }
}
