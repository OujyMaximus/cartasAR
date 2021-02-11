using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PlayerDetect
{
    private bool isActive;
    private GameObject[] cardsInstatiated;

    private bool cardSelected;
    private Quaternion playerRotation;

    private Vector3 cardPosition;
    private Quaternion cardRotation;

    private Vector2 touchPosition;

    private UnityAction<bool, Button> buttonPlacementPress;
    private Func<bool, GameObject[]> buttonSelectCardPress;

    private UnityAction checkPlayerTableDistance;
    private UnityAction<Vector2> checkCardSwitching;

    private Button[] buttons;

    public PlayerDetect(
                        UnityAction<bool, Button> buttonPlacementPress,
                        Func<bool, GameObject[]> buttonSelectCardPress,
                        UnityAction checkPlayerTableDistance,
                        UnityAction<Vector2> checkCardSwitching,
                        Button[] buttons)
    {
        this.buttonPlacementPress = buttonPlacementPress;
        this.buttonSelectCardPress = buttonSelectCardPress;
        this.checkPlayerTableDistance = checkPlayerTableDistance;
        this.checkCardSwitching = checkCardSwitching;
        this.buttons = buttons;
    }

    public void AwakePlayerDetect()
    {
        isActive = true;
        cardSelected = false;

        touchPosition = new Vector2();

        ConfigureButtons();
    }

    public void StartPlayerDetect()
    {

    }

    public void UpdatePlayerDetect()
    {
        //Aqui habra que poner comprobaciones para que se ejecuten solo en x situaciones
        //CheckPlayerTableDistance();
        //CheckCardSwitching();
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

    void ButtonPlacementPress(Button button)
    {
        isActive = !isActive;
        buttonPlacementPress?.Invoke(isActive, button);
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