using UnityEngine;

public class LayerManager : Singleton<LayerManager>
{
    [SerializeField]
    private LayerMask pieceDropLayerMask;


    public static LayerMask PieceDropLayerMask { get => Instance.pieceDropLayerMask; }


    public static bool IsLayerPartOfLayerMask(int layer, LayerMask layerMask)
    {
        return layerMask == (layerMask | (1 << layer));
    }

    protected override LayerManager GetThis()
    {
        return this;
    }
}
