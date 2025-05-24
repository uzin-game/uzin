// -----------------------------
// FINAL Mining.cs (FIXED)
// -----------------------------
using QuestsScrpit;
using RedstoneinventeGameStudio;
using UnityEngine;
using UnityEngine.Tilemaps;
using Unity.Netcode;
using System.Collections.Generic;
using MapScripts;

public class Mining : NetworkBehaviour
{
    [Header("Références")]
    [SerializeField] private Tilemap resourceTilemap;

    [Header("Tiles (minerais)")]
    [SerializeField] private TileBase charbonTile;
    [SerializeField] private TileBase ironTile;
    [SerializeField] private TileBase copperTile;
    [SerializeField] private TileBase goldTile;
    [SerializeField] private TileBase grassTile;

    [Header("Items (drops)")]
    [SerializeField] private InventoryItemData coal;
    [SerializeField] private InventoryItemData iron;
    [SerializeField] private InventoryItemData copper;
    [SerializeField] private InventoryItemData gold;

    private Dictionary<TileBase, InventoryItemData> tileToOre;

    private void Awake()
    {
        tileToOre = new Dictionary<TileBase, InventoryItemData>
        {
            { charbonTile, coal },
            { ironTile, iron },
            { copperTile, copper },
            { goldTile, gold }
        };

        if (resourceTilemap == null)
            resourceTilemap = FindObjectOfType<ChunkManager>()?.tilemap;
    }

    private void Update()
    {
        if (!IsOwner) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            Vector3 worldPos = transform.position;
            // CORRECTION: Passer directement ce NetworkObject au lieu de l'ID
            TryMineServerRpc(worldPos);
        }
    }

    [ServerRpc(RequireOwnership = true)] // CORRECTION: RequireOwnership = true
    private void TryMineServerRpc(Vector3 playerWorldPos)
    {
        Vector3Int cellPos = resourceTilemap.WorldToCell(playerWorldPos);
        TileBase tile = resourceTilemap.GetTile(cellPos);

        if (tile == null || !tileToOre.TryGetValue(tile, out InventoryItemData ore))
            return;

        // CORRECTION: Utiliser directement ce NetworkObject (le joueur qui a appelé le ServerRpc)
        NetworkObject playerObject = this.NetworkObject;

        if (playerObject != null && playerObject.TryGetComponent(out InventoryUsing playerInventory))
        {
            playerInventory.Increment(ore);
            Debug.Log($"[Mining] Ajout de {ore.itemName} à l'inventaire de {playerObject.name}");
        }
        else
        {
            Debug.LogWarning($"[Mining] Aucun inventaire trouvé sur {playerObject?.name}");
        }

        if (playerObject.TryGetComponent(out QuestManager questManager))
        {
            int qi = questManager.currentQuestIndex;
            bool relevant = (tile == charbonTile && qi == 1) ||
                          (tile == ironTile && qi == 2) ||
                          (tile == copperTile && qi == 3) ||
                          (tile == goldTile && qi == 4);

            if (relevant)
                questManager.Quests[qi].Progress(1f);
        }

        var chunkManager = FindObjectOfType<ChunkManager>();
        if (chunkManager != null)
            chunkManager.MineTile(cellPos, grassTile);

        Debug.Log($"[Mining] Miné {ore.itemName} en {cellPos} par {playerObject.name}");
    }
}