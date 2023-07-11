using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RailwayCo.Unity
{
    public class TrackManager : MonoBehaviour
    {
        public int PathCost { get; private set; }
        private void Awake()
        {
            int numTracks = this.transform.childCount;
            for (int i =0; i<numTracks; i++)
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
    }
}
