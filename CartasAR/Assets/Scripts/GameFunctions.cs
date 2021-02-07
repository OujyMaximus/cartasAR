using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SpatialTracking;
using UnityEngine.UI;

public class GameFunctions : MonoBehaviour
{
    #region Scripts variables
    private PlaceOnPlane placeOnPlane;
    private PlayerDetect playerDetect;
    #endregion

    #region Scene GameObject variables
    public GameObject placementIndicator;
    public GameObject cardPrefab;
    public GameObject arCamera;
    public GameObject cardPositionGO;
    public TrackedPoseDriver aRTrackedPoseDriver;
    #endregion

    #region ButtonInteraction variables

    #endregion

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    //----------------------------------------------
    //BUTTON INTERACTION
    //----------------------------------------------

    public void ButtonPlacementPress(bool isActive, Image image)
    {
        isActive = !isActive;
        placementIndicator.SetActive(isActive);
        if (isActive)
        {
            image.color = new Color(0.2926f, 1f, 0.033f, 1f);
        }
        else
        {
            image.color = new Color(1f, 0.3537f, 0.3537f, 1f);
        }
    }

    //----------------------------------------------

    //Este metodo se activa al pulsar el boton de mazo y indica si hay que actualizar la posición de las cartas
    public GameObject[] ButtonSelectCardPress(bool cardSelected)
    {
        GameObject[] cardsInstantiated = new GameObject[3];
        Vector3 cardPosition;
        Quaternion cardRotation;

        if (cardSelected)
        {
            cardPosition = cardPositionGO.transform.position;
            cardRotation = cardPositionGO.transform.rotation;

            cardsInstantiated[0] = GameObject.Instantiate(cardPrefab, cardPosition, cardRotation);
            cardInstantiate.transform.SetParent(cardPositionGO.transform);

            Vector3 cardPosition2 = new Vector3(cardInstantiate.transform.localPosition.x - 0.08f, cardInstantiate.transform.localPosition.y, cardInstantiate.transform.localPosition.z - 0.05f);

            cardInstantiate2 = GameObject.Instantiate(cardPrefab, cardPosition, cardRotation);
            cardInstantiate2.transform.SetParent(cardPositionGO.transform);
            cardInstantiate2.transform.localPosition = cardPosition2;

            Vector3 cardPosition3 = new Vector3(cardInstantiate.transform.localPosition.x - (0.08f * 2), cardInstantiate.transform.localPosition.y, cardInstantiate.transform.localPosition.z - 0.05f);

            cardInstantiate3 = GameObject.Instantiate(cardPrefab, cardPosition, cardRotation);
            cardInstantiate3.transform.SetParent(cardPositionGO.transform);
            cardInstantiate3.transform.localPosition = cardPosition3;
        }
        else
        {
            if (cardInstantiate != null)
                Destroy(cardInstantiate);
            if (cardInstantiate2 != null)
                Destroy(cardInstantiate2);
            if (cardInstantiate3 != null)
                Destroy(cardInstantiate3);
        }
    }
}
