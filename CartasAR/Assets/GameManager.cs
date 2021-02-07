using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private PlaceOnPlane placeOnPlane;

    void Awake()
    {
        ConfigurePlaceOnPlane();
    }

    //------------------------------------------------------
    //Configuraciones
    //------------------------------------------------------

    public void ConfigurePlaceOnPlane()
    {
        placeOnPlane = new PlaceOnPlane(
            );
    }
}
