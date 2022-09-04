using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PlayerDetect
{
    private List<GameObject> cardsInstantiated;

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
    private UnityAction<List<GameObject>> setCardInTable;
    private UnityAction<int> setOpositeCardInTable;

    //TESTING

    private UnityAction changeBoardMaterial;

    #region Pyramid
    private UnityAction<List<GameObject>, int> addCardToDeck;
    private UnityAction giveCardToPlayer;
    private UnityAction makePyramid;
    private UnityAction<int> addCardToPyramid;
    private UnityAction findCardToFlipInPyramid;
    private UnityAction<int> flipCardInPyramid;
    private UnityAction prepareFinalRound;
    private UnityAction<int> addCardToFinalRound;
    private UnityAction selectCardToGiveFinalRound;
    private UnityAction<int> giveCardFinalRound;
    private UnityAction flipCardToPlayersFinalRound;
    private UnityAction flipCardFinalRound;
    private UnityAction dealCardsFinalRound;
    #endregion

    private Button[] buttons;

    public PlayerDetect(
                        UnityAction<bool, Button> buttonPlacementPress,
                        UnityAction<bool, List<GameObject>> buttonSelectCardPress,
                        UnityAction checkPlayerTableDistance,
                        UnityAction<Vector2> checkCardSwitching,
                        UnityAction<List<GameObject>, int> switchCardInFront,
                        UnityAction<List<GameObject>> selectCardInFront,
                        UnityAction<List<GameObject>> setCardInTable,
                        UnityAction<int> setOpositeCardInTable,
                        UnityAction<List<GameObject>, int> addCardToDeck,
                        UnityAction giveCardToPlayer,
                        UnityAction makePyramid,
                        UnityAction<int> addCardToPyramid,
                        UnityAction findCardToFlipInPyramid,
                        UnityAction<int> flipCardInPyramid,
                        UnityAction prepareFinalRound,
                        UnityAction<int> addCardToFinalRound,
                        UnityAction selectCardToGiveFinalRound,
                        UnityAction<int> giveCardFinalRound,
                        UnityAction flipCardToPlayersFinalRound,
                        UnityAction flipCardFinalRound,
                        UnityAction dealCardsFinalRound,
                        UnityAction changeBoardMaterial,
                        Button[] buttons)
    {
        this.buttonPlacementPress = buttonPlacementPress;
        this.buttonSelectCardPress = buttonSelectCardPress;
        this.checkPlayerTableDistance = checkPlayerTableDistance;
        this.checkCardSwitching = checkCardSwitching;
        this.switchCardInFront = switchCardInFront;
        this.selectCardInFront = selectCardInFront;
        this.setCardInTable = setCardInTable;
        this.setOpositeCardInTable = setOpositeCardInTable;
        this.addCardToDeck = addCardToDeck;
        this.giveCardToPlayer = giveCardToPlayer;
        this.makePyramid = makePyramid;
        this.addCardToPyramid = addCardToPyramid;
        this.findCardToFlipInPyramid = findCardToFlipInPyramid;
        this.flipCardInPyramid = flipCardInPyramid;
        this.prepareFinalRound = prepareFinalRound;
        this.addCardToFinalRound = addCardToFinalRound;
        this.selectCardToGiveFinalRound = selectCardToGiveFinalRound;
        this.giveCardFinalRound = giveCardFinalRound;
        this.flipCardToPlayersFinalRound = flipCardToPlayersFinalRound;
        this.flipCardFinalRound = flipCardFinalRound;
        this.dealCardsFinalRound = dealCardsFinalRound;
        this.changeBoardMaterial = changeBoardMaterial;
        this.buttons = buttons;
    }

    public void StartPlayerDetect()
    {
        isActive = true;
        cardSelected = false;
        isCardSelected = false;

        touchPosition = new Vector2();
        cardsInstantiated = new List<GameObject>();

        ConfigureButtons();

        GameObject.Find("MenuController").GetComponent<MenuController>().ConfigurePlayerMethods(
                                SetOpositeCardInTable,
                                AddCardToDeck,
                                AddCardToPyramid,
                                FlipCardInPyramid,
                                AddCardToFinalRound,
                                giveCardFinalRound,
                                flipCardFinalRound);
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
            else if (b.gameObject.name == "ButtonSelectCard")
                b.onClick.AddListener(SelectCardInFront);
            else if (b.gameObject.name == "ButtonSetCard")
                b.onClick.AddListener(SetCardInTable);
            else if (b.gameObject.name == "ButtonGiveCard")
                b.onClick.AddListener(GiveCardToPlayer);
            else if (b.gameObject.name == "ButtonMakePyramid")
                b.onClick.AddListener(MakePyramid);
            else if (b.gameObject.name == "ButtonFlipCard")
                b.onClick.AddListener(FindCardToFlipInPyramid);
            else if (b.gameObject.name == "ButtonFinalRound")
                b.onClick.AddListener(PrepareFinalRound);
            else if (b.gameObject.name == "ButtonGiveCardFinalRound")
                b.onClick.AddListener(GiveCardFinalRound);
            else if (b.gameObject.name == "ButtonFlipCardFinalRound")
                b.onClick.AddListener(FlipCardToPlayersFinalRound);
            else if (b.gameObject.name == "ButtonDealCardsFinalRound")
                b.onClick.AddListener(DealCardsFinalRound);
            else if (b.gameObject.name == "ButtonMaterial")
                b.onClick.AddListener(ButtonMaterial);
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

    //-----------------------------------------------------------------------------

    //Este metodo se activa al pulsar el boton de mazo y indica si hay que actualizar la posición de las cartas
    public void ButtonSelectCardPress()
    {
        cardSelected = !cardSelected;
        isCardSelected = false;
        buttonSelectCardPress?.Invoke(cardSelected, cardsInstantiated);
    }

    //-----------------------------------------------------------------------------

    public bool GetCardSelectStatus() => cardSelected;

    //-----------------------------------------------------------------------------

    public bool GetPlacementIndicatorStatus() => isActive;

    //-----------------------------------------------------------------------------

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

    //-----------------------------------------------------------------------------

    public void SwitchCardInFront(int direction)
    {
        switchCardInFront?.Invoke(cardsInstantiated, direction);
    }

    //-----------------------------------------------------------------------------

    public void SelectCardInFront()
    {
        isCardSelected = !isCardSelected;
        cardSelected = !cardSelected;
        selectCardInFront?.Invoke(cardsInstantiated);
    }

    //-----------------------------------------------------------------------------

    public void SetCardInTable()
    {
        isCardSelected = !isCardSelected;
        setCardInTable?.Invoke(cardsInstantiated);
    }

    //-----------------------------------------------------------------------------

    public void SetOpositeCardInTable(int id)
    {
        setOpositeCardInTable?.Invoke(id);
    }

    //-----------------------------------------------------------------------------

    public void AddCardToDeck(int id)
    {
        addCardToDeck?.Invoke(cardsInstantiated, id);
    }

    //-----------------------------------------------------------------------------

    public void GiveCardToPlayer()
    {
        giveCardToPlayer?.Invoke();
    }

    //-----------------------------------------------------------------------------

    public void MakePyramid()
    {
        makePyramid?.Invoke();
    }

    //-----------------------------------------------------------------------------

    public void AddCardToPyramid(int id)
    {
        addCardToPyramid?.Invoke(id);
    }

    //-----------------------------------------------------------------------------

    public void FindCardToFlipInPyramid()
    {
        findCardToFlipInPyramid?.Invoke();
    }

    //-----------------------------------------------------------------------------

    public void FlipCardInPyramid(int id)
    {
        flipCardInPyramid?.Invoke(id);
    }

    //-----------------------------------------------------------------------------

    public void PrepareFinalRound()
    {
        prepareFinalRound?.Invoke();
    }

    //-----------------------------------------------------------------------------

    public void AddCardToFinalRound(int id)
    {
        addCardToFinalRound?.Invoke(id);
    }

    //-----------------------------------------------------------------------------

    public void GiveCardFinalRound()
    {
        selectCardToGiveFinalRound?.Invoke();
    }

    //-----------------------------------------------------------------------------

    public void FlipCardToPlayersFinalRound()
    {
        flipCardToPlayersFinalRound?.Invoke();
    }

    //-----------------------------------------------------------------------------

    public void DealCardsFinalRound()
    {
        dealCardsFinalRound?.Invoke();
    }

    //-----------------------------------------------------------------------------

    public void SetTouchPosition(Vector2 touchPosition) => this.touchPosition = touchPosition;

    //----------------------------------------------
    //TESTING METHODS
    //----------------------------------------------

    public void ButtonMaterial()
    {
        Debug.Log("Hola");
        changeBoardMaterial?.Invoke();
    }
}