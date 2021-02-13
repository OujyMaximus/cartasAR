using System;
using System.Collections;
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
    private Touch playerTouch;

    private UnityAction<bool, Button> buttonPlacementPress;
    private UnityAction<bool, GameObject[]> buttonSelectCardPress;

    private UnityAction checkPlayerTableDistance;
    private UnityAction<Vector2> checkCardSwitching;
    private UnityAction<GameObject[], int> switchCardInFront;

    private Button[] buttons;

    public PlayerDetect(
                        UnityAction<bool, Button> buttonPlacementPress,
                        UnityAction<bool, GameObject[]> buttonSelectCardPress,
                        UnityAction checkPlayerTableDistance,
                        UnityAction<Vector2> checkCardSwitching,
                        UnityAction<GameObject[], int> switchCardInFront,
                        Button[] buttons)
    {
        this.buttonPlacementPress = buttonPlacementPress;
        this.buttonSelectCardPress = buttonSelectCardPress;
        this.checkPlayerTableDistance = checkPlayerTableDistance;
        this.checkCardSwitching = checkCardSwitching;
        this.switchCardInFront = switchCardInFront;
        this.buttons = buttons;
    }

    public void AwakePlayerDetect()
    {
        isActive = true;
        cardSelected = false;

        touchPosition = new Vector2();
        cardsInstatiated = new GameObject[3];

        ConfigureButtons();
    }

    public void StartPlayerDetect()
    {

    }

    public void UpdatePlayerDetect()
    {
        //Aqui habra que poner comprobaciones para que se ejecuten solo en x situaciones
        if(isActive)
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
        buttonSelectCardPress?.Invoke(cardSelected, cardsInstatiated);
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

    public void SwitchCardInFront(int direction)
    {
        switchCardInFront?.Invoke(cardsInstatiated, direction);
    }

    public void SetTouchPosition(Vector2 touchPosition) => this.touchPosition = touchPosition;

    private IEnumerator WaitCardSelected(float s)
    {
        yield return new WaitForSeconds(s);
        cardSelected = !cardSelected;
    }

}