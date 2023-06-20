using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CargoDetailButton : MonoBehaviour
{
    [SerializeField] private Button cargoInfo;

    void Start()
    {
        cargoInfo.onClick.AddListener(OnButtonClicked);
    }

    public void OnButtonClicked()
    {
        Debug.Log("A Cargo has been clicked");
    }
}
