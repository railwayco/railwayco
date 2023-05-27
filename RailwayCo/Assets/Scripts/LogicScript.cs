using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LogicScript : MonoBehaviour
{

    public GameObject coinText;
    public GameObject expText;
    private int exp;
    private int coin;
    
    void Start()
    {
        exp = int.Parse(expText.GetComponent<Text>().text);
        coin = int.Parse(coinText.GetComponent<Text>().text);
    }

    // Will enforce the need to provide parameters later. Work with globals first.
    public void updateRewards()
    {
        // Stopgap solution. Will fix again
        coin += 2000;
        exp += 100;
        coinText.GetComponent<Text>().text = coin.ToString();
        expText.GetComponent<Text>().text = exp.ToString();
    }
}
