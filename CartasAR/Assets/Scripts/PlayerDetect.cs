using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SpatialTracking;
using UnityEngine.UI;

public class PlayerDetect : MonoBehaviour
{
    public GameObject ARSessionOriginGO;
    PlaceOnPlane placeOnPlane;

    private bool isActive;
    private GameObject[] cardsInstatiated;

    private bool cardSelected;
    private Quaternion playerRotation;

    private Vector3 cardPosition;
    private Quaternion cardRotation;

    private UnityAction<bool, Image> buttonPlacementPress;
    private Func<bool, GameObject[]> buttonSelectCardPress;

    private UnityAction checkPlayerTableDistance;

    public PlayerDetect(
                        UnityAction<bool, Image> buttonPlacementPress,
                        Func<bool, GameObject[]> buttonSelectCardPress,
                        UnityAction checkPlayerTableDistance)
    {
        this.buttonPlacementPress = buttonPlacementPress;
        this.buttonSelectCardPress = buttonSelectCardPress;
        this.checkPlayerTableDistance = checkPlayerTableDistance;
    }

    public void AwakePlayerDetect()
    {
        isActive = true;
        cardSelected = false;
    }

    public void StartPlayerDetect()
    {

    }

    public void UpdatePlayerDetect()
    {
        checkPlayerTableDistance?.Invoke();
    }

    //----------------------------------------------
    //BUTTON INTERACTION
    //----------------------------------------------

    public void ButtonPlacementPress()
    {
        buttonPlacementPress?.Invoke(isActive, GetComponent<Image>());
    }

    //----------------------------------------------

    //Este metodo se activa al pulsar el boton de mazo y indica si hay que actualizar la posición de las cartas
    public void ButtonSelectCardPress()
    {
        cardSelected = !cardSelected;
        cardsInstatiated = buttonSelectCardPress?.Invoke(cardSelected);
    }

    //----------------------------------------------

    public bool GetCardSelectStatus() => cardSelected;

    //----------------------------------------------

    public bool GetPlacementIndicatorStatus() => isActive;

    //----------------------------------------------
    //PLAYER DETECT
    //----------------------------------------------


}