using UnityEngine;
using UnityEngine.Tilemaps;
using RedstoneinventeGameStudio;
using QuestsScrpit;
using System.Linq;
using System.Collections.Generic;
using MapScripts;
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

    [Header("Chunk Manager")] [SerializeField]
    private ChunkManager chunkManager;

    private Dictionary<TileBase, InventoryItemData> _tileToOre;
    private QuestManager _questManager;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
        {
            enabled = false;
        }
        
        _questManager = FindFirstObjectByType<QuestManager>();
    }

    void Awake()
    {
        _tileToOre = new Dictionary<TileBase, InventoryItemData>
        {
            { charbonTile, coal },
            { ironTile, iron },
            { copperTile, copper },
            { goldTile, gold }
        };

        if (resourceTilemap == null)
            resourceTilemap = FindFirstObjectByType<Tilemap>();

        if (chunkManager == null)
            chunkManager = FindFirstObjectByType<ChunkManager>();
    }

    void Start()
    {
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

        bool added = inventoryUsing.AddItem(oreData);
        if (!added)
        {
            Debug.LogWarning("[Mining] Inventaire plein ou slot introuvable.");
            return;
        }

        // Demande au serveur de mettre à jour la tuile pour tous
        MineTileServerRpc(cellPos, grassTile.name, tile.name);

        // Progression de quête locale
        //UpdateQuestProgress(tile);
    }

    private void UpdateQuestProgress(TileBase tile)
    {
        if (_questManager != null)
        {
            int qi = _questManager.currentQuestIndex;
            bool match =
                (tile == charbonTile && qi == 1) ||
                (tile == ironTile && qi == 2) ||
                (tile == copperTile && qi == 3) ||
                (tile == goldTile && qi == 4);
            if (match)
                _questManager.Quests[qi].Progress(1f);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void MineTileServerRpc(Vector3Int cellPos, string newTileName, string originalTileName)
    {
        Debug.Log($"[ServerRpc] Received mining request from client at {cellPos} for {newTileName}");
        if (!IsServer) return;

        TileBase newTile = GetTileByName(newTileName);
        if (newTile == null) return;

        // Serveur met à jour sa tilemap et notifie tous les clients
        chunkManager.MineTile(cellPos, newTile);
        
        // Avance la quête si c'est du charbon et la quête n°1
        if (_questManager != null && _questManager.currentQuestIndex == 1)
        {
            TileBase originalTile = GetTileByName(originalTileName);
            if (originalTile == charbonTile)
            {
                _questManager.Quests[1].Progress(1f);
            }
        }
    }

    private TileBase GetTileByName(string name)
    {
        return new[] { charbonTile, ironTile, copperTile, goldTile, grassTile }
            .FirstOrDefault(t => t != null && t.name == name);
    }
}