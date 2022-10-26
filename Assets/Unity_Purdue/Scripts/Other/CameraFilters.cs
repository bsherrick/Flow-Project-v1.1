using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFilters : MonoBehaviour
{
    [HideInInspector]
    public CameraFilterPack_Color_Invert colorInvert;
    [HideInInspector]
    public CameraFilterPack_Color_GrayScale grayScale;
    [HideInInspector]
    public CameraFilterPack_Drawing_Manga5 drawingManga;
    [HideInInspector]
    public CameraFilterPack_Drawing_Toon drawingToon;
    [HideInInspector]
    public CameraFilterPack_Edge_Neon edgeNeon;
    [HideInInspector]
    public CameraFilterPack_FX_8bits fx8bits;
    [HideInInspector]
    public CameraFilterPack_TV_Horror tvHorror;
    [HideInInspector]
    public CameraFilterPack_TV_Old_Movie tvOldMovie;

    void Start()
    {
        colorInvert = GetComponent<CameraFilterPack_Color_Invert>();
        grayScale = GetComponent<CameraFilterPack_Color_GrayScale>();
        drawingManga = GetComponent<CameraFilterPack_Drawing_Manga5>();
        drawingToon = GetComponent<CameraFilterPack_Drawing_Toon>();
        edgeNeon = GetComponent<CameraFilterPack_Edge_Neon>();
        fx8bits = GetComponent<CameraFilterPack_FX_8bits>();
        tvHorror = GetComponent<CameraFilterPack_TV_Horror>();
        tvOldMovie = GetComponent<CameraFilterPack_TV_Old_Movie>();
    }

    void Update()
    {
        
    }
}
