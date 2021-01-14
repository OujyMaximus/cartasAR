using UnityEngine;
using UnityEngine.SpatialTracking;
using UnityEngine.UI;

public class ButtonInteraction : MonoBehaviour
{
    public bool isActive = true;
    public GameObject placementIndicator;
    public GameObject cardPrefab;
    private GameObject cardInstantiate;

    public GameObject arCamera;
    TrackedPoseDriver aRTrackedPoseDriver;

    private bool cardSelected;
    private Vector3 playerPosition;
    private Quaternion playerRotation;

    private Vector3 cardPosition;
    private Quaternion cardRotation;

    void Awake()
    {
        cardSelected = false;
        aRTrackedPoseDriver = arCamera.GetComponent<TrackedPoseDriver>();
    }

    void Update()
    {
        playerRotation = aRTrackedPoseDriver.transform.rotation.normalized;
        playerPosition = aRTrackedPoseDriver.transform.position;

        if (cardSelected)
        {
            cardPosition = new Vector3(playerPosition.x + playerRotation.x, playerPosition.y + playerRotation.y, playerPosition.z + playerRotation.z);
            cardRotation = new Quaternion(1 - playerRotation.x, 1 - playerRotation.y, 1 - playerRotation.z, 1 - playerRotation.w);

            cardInstantiate.transform.position = cardPosition;
            cardInstantiate.transform.rotation = cardRotation;
        }
    }

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

    //Este metodo se activa al pulsar el boton de mazo y indica si hay que actualizar la posición de las cartas
    public void buttonSelectCardPress()
    {
        cardPosition = new Vector3(playerPosition.x + playerRotation.x, playerPosition.y + playerRotation.y, playerPosition.z + playerRotation.z);
        cardRotation = new Quaternion(1 - playerRotation.x, 1 - playerRotation.y, 1 - playerRotation.z, 1 - playerRotation.w);

        cardSelected = !cardSelected;
        cardInstantiate = GameObject.Instantiate(cardPrefab, cardPosition, cardRotation);
    }
}
