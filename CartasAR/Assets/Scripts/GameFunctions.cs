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
    public GameObject arSessionOrigin;
    public GameObject cardPositionGO;
    public TrackedPoseDriver aRTrackedPoseDriver;
    #endregion

    #region PlayerDetect variables
    private Vector3 playerPosition;
    private GameObject placedTable;
    private float distanceTablePlayer;
    private Vector3 placedTablePosition;
    private bool isPlaced;

    public Material iceMaterial;
    #endregion

    public void AwakeGameFunction()
    {

    }

    public void StartGameFunction()
    {
        
    }

    public void UpdateGameFunction()
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
            cardsInstantiated[0].transform.SetParent(cardPositionGO.transform);

            Vector3 cardPosition2 = new Vector3(cardsInstantiated[0].transform.localPosition.x - 0.08f, cardsInstantiated[0].transform.localPosition.y, cardsInstantiated[0].transform.localPosition.z - 0.05f);

            cardsInstantiated[1] = GameObject.Instantiate(cardPrefab, cardPosition, cardRotation);
            cardsInstantiated[1].transform.SetParent(cardPositionGO.transform);
            cardsInstantiated[1].transform.localPosition = cardPosition2;

            Vector3 cardPosition3 = new Vector3(cardsInstantiated[0].transform.localPosition.x - (0.08f * 2), cardsInstantiated[0].transform.localPosition.y, cardsInstantiated[0].transform.localPosition.z - 0.05f);

            cardsInstantiated[2] = GameObject.Instantiate(cardPrefab, cardPosition, cardRotation);
            cardsInstantiated[2].transform.SetParent(cardPositionGO.transform);
            cardsInstantiated[2].transform.localPosition = cardPosition3;
        }
        else
        {
            if (cardsInstantiated[0] != null)
                Destroy(cardsInstantiated[0]);
            if (cardsInstantiated[1] != null)
                Destroy(cardsInstantiated[1]);
            if (cardsInstantiated[2] != null)
                Destroy(cardsInstantiated[2]);
        }

        return cardsInstantiated;
    }


    //Funcion para comprobar la distancia entre el jugador y el tablero, se ejecutara en el Update de PlayerDetect
    public void CheckPlayerTableDistance()
    {
        playerPosition = aRTrackedPoseDriver.transform.position;

        if (isPlaced)
        {
            placedTable = placeOnPlane.spawnedObject;
            placedTablePosition = placedTable.transform.position;

            distanceTablePlayer = (placedTablePosition - playerPosition).magnitude;

            if (distanceTablePlayer < 0.5)
            {
                placedTable.GetComponent<MeshRenderer>().material = iceMaterial;
            }
        }
    }
}
