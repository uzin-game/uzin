using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(Canvas))]
public class CanvasFix : NetworkBehaviour
{
    [SerializeField] private int localOrder = 100;
    [SerializeField] private int remoteOrder = 0;

    private Canvas _canvas;

    private void Awake()
    {
        _canvas = GetComponent<Canvas>();
        _canvas.overrideSorting = true;
    }

    public override void OnNetworkSpawn()
    {
        _canvas.sortingOrder = IsOwner ? localOrder : remoteOrder;
    }
}