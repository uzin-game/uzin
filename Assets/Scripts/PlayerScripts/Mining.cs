using UnityEngine;
using UnityEngine.Tilemaps;
using RedstoneinventeGameStudio;
using QuestsScrpit;
using System.Collections.Generic;
using Unity.Netcode;

public class Mining : NetworkBehaviour
{
    [Header("Références UI")] [SerializeField]
    private InventoryUsing inventoryUsing;

    [Header("Tiles & Datas")] [SerializeField]
    private Tilemap resourceTilemap;

    [SerializeField] private TileBase grassTile;
    [SerializeField] private TileBase charbonTile, ironTile, copperTile, goldTile;
    [SerializeField] private InventoryItemData coal, iron, copper, gold;

    private Dictionary<TileBase, InventoryItemData> _tileToOre;
    private QuestManager _questManager;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
        {
            enabled = false;
        }
    }

    void Awake()
    {
        // Mapping Tile → ItemData
        _tileToOre = new Dictionary<TileBase, InventoryItemData>
        {
            { charbonTile, coal },
            { ironTile, iron },
            { copperTile, copper },
            { goldTile, gold }
        };

        if (resourceTilemap == null)
            resourceTilemap = FindObjectOfType<Tilemap>();
    }

    void Start()
    {
        _questManager = GetComponent<QuestManager>();
    }

    void Update()
    {
        if (!IsOwner) return;

        if (Input.GetKeyDown(KeyCode.Q))
            Mine();
    }

    private void Mine()
    {
        Vector3Int cellPos = resourceTilemap.WorldToCell(transform.position);
        var tile = resourceTilemap.GetTile(cellPos);

        if (tile == null || !_tileToOre.TryGetValue(tile, out var oreData))
        {
            Debug.LogWarning("[Mining] Pas de minerai ici.");
            return;
        }

        bool added = inventoryUsing.Increment(oreData);

        if (!added)
            Debug.LogWarning("[Mining] Inventaire plein ou slot introuvable.");

        resourceTilemap.SetTile(cellPos, grassTile);

        // TODO: réactiver les quêtes
        /*if (_questManager != null)
        {
            int qi = _questManager.currentQuestIndex;
            bool match =
                (tile == charbonTile && qi == 1) ||
                (tile == ironTile    && qi == 2) ||
                (tile == copperTile  && qi == 3) ||
                (tile == goldTile    && qi == 4);
            if (match)
                _questManager.Quests[qi].Progress(1f);
        }*/
    }
}