using UnityEngine;
using UnityEngine.SpatialTracking;
using UnityEngine.UI;

public class ButtonInteraction : MonoBehaviour
{
    public bool isActive = true;
    public GameObject placementIndicator;
    public GameObject cardPrefab;
    private GameObject cardInstantiate;
    private GameObject cardInstantiate2;
    private GameObject cardInstantiate3;

    public GameObject arCamera;
    public GameObject cardPositionGO;
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

    public void ButtonPlacementPress()
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
    public void ButtonSelectCardPress()
    {
        cardSelected = !cardSelected;

        if (cardSelected)
        {
            cardPosition = cardPositionGO.transform.position;
            //cardRotation = new Quaternion(1 - playerRotation.x, 1 - playerRotation.y, 1 - playerRotation.z, 1 - playerRotation.w);
            cardRotation = cardPositionGO.transform.rotation;

            cardInstantiate = GameObject.Instantiate(cardPrefab, cardPosition, cardRotation);
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

    public bool GetCardSelectStatus() => cardSelected;
}
