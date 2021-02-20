﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PlayerDetect
{
    private List<GameObject> cardsInstatiated;

    private bool isActive;
    private bool cardSelected;
    private bool isCardSelected;

    private Vector2 touchPosition;

    private UnityAction<bool, Button> buttonPlacementPress;
    private UnityAction<bool, List<GameObject>> buttonSelectCardPress;

    private UnityAction checkPlayerTableDistance;
    private UnityAction<Vector2> checkCardSwitching;
    private UnityAction<List<GameObject>, int> switchCardInFront;
    private UnityAction<List<GameObject>> selectCardInFront;

    private Button[] buttons;

    public PlayerDetect(
                        UnityAction<bool, Button> buttonPlacementPress,
                        UnityAction<bool, List<GameObject>> buttonSelectCardPress,
                        UnityAction checkPlayerTableDistance,
                        UnityAction<Vector2> checkCardSwitching,
                        UnityAction<List<GameObject>, int> switchCardInFront,
                        UnityAction<List<GameObject>> selectCardInFront,
                        Button[] buttons)
    {
        this.buttonPlacementPress = buttonPlacementPress;
        this.buttonSelectCardPress = buttonSelectCardPress;
        this.checkPlayerTableDistance = checkPlayerTableDistance;
        this.checkCardSwitching = checkCardSwitching;
        this.switchCardInFront = switchCardInFront;
        this.selectCardInFront = selectCardInFront;
        this.buttons = buttons;
    }

    public void StartPlayerDetect()
    {
        isActive = true;
        cardSelected = false;
        isCardSelected = false;

        touchPosition = new Vector2();
        cardsInstatiated = new List<GameObject>();

        ConfigureButtons();
    }

    public void UpdatePlayerDetect()
    {
        if (isCardSelected)
            CheckPlayerTableDistance();
        
        if(cardSelected)
            CheckCardSwitching();
    }

    public void ConfigureButtons()
    {        
        foreach (Button b in buttons)
        {
            b.onClick.RemoveAllListeners();
            if (b.gameObject.name == "ButtonPlacement")
                b.onClick.AddListener(() => ButtonPlacementPress(b));
            else if (b.gameObject.name == "ButtonCard")
                b.onClick.AddListener(ButtonSelectCardPress);
        }
    }

    //----------------------------------------------
    //BUTTONS METHODS
    //----------------------------------------------

    public void ButtonPlacementPress(Button button)
    {
        isActive = !isActive;
        buttonPlacementPress?.Invoke(isActive, button);
    }

    //----------------------------------------------

    //Este metodo se activa al pulsar el boton de mazo y indica si hay que actualizar la posición de las cartas
    public void ButtonSelectCardPress()
    {
        cardSelected = !cardSelected;
        buttonSelectCardPress?.Invoke(cardSelected, cardsInstatiated);
    }

    //----------------------------------------------

    public bool GetCardSelectStatus() => cardSelected;

    //----------------------------------------------

    public bool GetPlacementIndicatorStatus() => isActive;

    //----------------------------------------------

    public void SetPlacementIndicatorStatus(bool status) => isActive = status;

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

    public void SwitchCardInFront(int direction)
    {
        switchCardInFront?.Invoke(cardsInstatiated, direction);
    }

    public void SelectCardInFront()
    {
        isCardSelected = !isCardSelected;
        selectCardInFront?.Invoke(cardsInstatiated);
    }

    public void SetTouchPosition(Vector2 touchPosition) => this.touchPosition = touchPosition;
}