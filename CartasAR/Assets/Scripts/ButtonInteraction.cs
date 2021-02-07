using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SpatialTracking;
using UnityEngine.UI;

public class ButtonInteraction : MonoBehaviour
{

    /*
     * 
     * CAMBIAR TODAS LAS FUNCIONES DE BUTTONINTERACTION AL PLAYERDETECT
     * 
     */

    private GameObject placementIndicator;
    private GameObject cardPrefab;
    private GameObject arCamera;
    private GameObject cardPositionGO;
    private TrackedPoseDriver aRTrackedPoseDriver;

    private bool isActive;
    private GameObject[] cardsInstatiated;
    private GameObject cardInstantiate;
    private GameObject cardInstantiate2;
    private GameObject cardInstantiate3;


    private bool cardSelected;
    private Vector3 playerPosition;
    private Quaternion playerRotation;

    private Vector3 cardPosition;
    private Quaternion cardRotation;

    private UnityAction<bool, Image> buttonPlacementPress;

    public ButtonInteraction(
                        GameObject placementIndicator,
                        GameObject cardPrefab,
                        GameObject arCamera,
                        GameObject cardPositionGO)
    {
        this.placementIndicator = placementIndicator;
        this.cardPrefab = cardPrefab;
        this.arCamera = arCamera;
        this.cardPositionGO = cardPositionGO;
    }

    void Awake()
    {
        isActive = true;
        cardSelected = false;

        aRTrackedPoseDriver = arCamera.GetComponent<TrackedPoseDriver>();
    }

    //----------------------------------------------
    //METHODS
    //----------------------------------------------

    public void ButtonPlacementPress()
    {
        buttonPlacementPress?.Invoke(isActive, GetComponent<Image>());
    }

    //Este metodo se activa al pulsar el boton de mazo y indica si hay que actualizar la posición de las cartas
    public void ButtonSelectCardPress()
    {
        cardSelected = !cardSelected;

    }

    public bool GetCardSelectStatus() => cardSelected;
}
