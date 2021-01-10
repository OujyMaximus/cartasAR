using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonInteraction : MonoBehaviour
{
    public bool isActive = true;
    public GameObject placementIndicator;

    public void buttonPlacementPress()
    {
        isActive = !isActive;
        placementIndicator.SetActive(isActive);
        if (isActive)
        {
            GetComponent<Image>().color = new Color(0.2926f, 1f, 0.033f, 1f);
        }
        else
        {
            GetComponent<Image>().color = new Color(1f, 0.3537f, 0.3537f, 1f);
        }
    }
}
