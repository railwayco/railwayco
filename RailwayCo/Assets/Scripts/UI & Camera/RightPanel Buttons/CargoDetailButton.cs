using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CargoDetailButton : MonoBehaviour
{
    public Button cargoButton;

    void Start()
    {
        cargoButton.onClick.AddListener(OnButtonClicked);
    }

    public void OnButtonClicked()
    {
        
    }
}
