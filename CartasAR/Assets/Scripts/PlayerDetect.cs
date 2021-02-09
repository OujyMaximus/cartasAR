using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SpatialTracking;
using UnityEngine.UI;

public class PlayerDetect : MonoBehaviour
{
    private bool isActive;
    private GameObject[] cardsInstatiated;

    private bool cardSelected;
    private Quaternion playerRotation;

    private Vector3 cardPosition;
    private Quaternion cardRotation;

    private Vector2 touchPosition;

    private UnityAction<bool, Image> buttonPlacementPress;
    private Func<bool, GameObject[]> buttonSelectCardPress;

    private UnityAction checkPlayerTableDistance;
    private UnityAction<Vector2> checkCardSwitching;

    public PlayerDetect(
                        UnityAction<bool, Image> buttonPlacementPress,
                        Func<bool, GameObject[]> buttonSelectCardPress,
                        UnityAction checkPlayerTableDistance,
                        UnityAction<Vector2> checkCardSwitching)
    {
        this.buttonPlacementPress = buttonPlacementPress;
        this.buttonSelectCardPress = buttonSelectCardPress;
        this.checkPlayerTableDistance = checkPlayerTableDistance;
        this.checkCardSwitching = checkCardSwitching;
    }

    public void AwakePlayerDetect()
    {
        isActive = true;
        cardSelected = false;

        touchPosition = new Vector2();
    }

    public void StartPlayerDetect()
    {

    }

    public void UpdatePlayerDetect()
    {
        //Aqui habra que poner comprobaciones para que se ejecuten solo en x situaciones
        CheckPlayerTableDistance();
        CheckCardSwitching();
    }

    //----------------------------------------------
    //BUTTONS METHODS
    //----------------------------------------------

    public void ButtonPlacementPress()
    {
        isActive = !isActive;
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
    //PLAYER ENVIRONMENT METHODS
    //----------------------------------------------

    public void CheckPlayerTableDistance()
    {
        checkPlayerTableDistance?.Invoke();
    }

    //----------------------------------------------
    //CARDS METHODS
    //----------------------------------------------

    public void CheckCardSwitching()
    {
        checkCardSwitching?.Invoke(touchPosition);
    }

    public void SetTouchPosition(Vector2 touchPosition) => this.touchPosition = touchPosition;

}