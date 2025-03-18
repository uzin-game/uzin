using System.Collections;
using MapScripts;
using UnityEngine;
using UnityEngine.Tilemaps;
using Unity.Netcode;

namespace PlayerScripts
{
    public class PlayerNetwork : NetworkBehaviour
    {
        public float moveSpeed = 3f; // Speed of movement
        private Rigidbody2D rb;
        private Vector2 movement;
        public GameObject player;

        public Tilemap tilemap; // Reference to the Tilemap
        public TileBase waterTile; // Assign the water tile in the Inspector

        private Vector3 lastValidPosition; // Stores last safe position

        public GameObject cameraPrefab;
        private GameObject playerCamera;


        void Start()
        {
            // Get the Rigidbody2D component
            rb = GetComponent<Rigidbody2D>();
            AdjustPlayerSize();

            // Find a safe spawn point
            //Vector3 safeSpawn = FindSafeSpawn();
            //rb.MovePosition(safeSpawn);
            //lastValidPosition = safeSpawn; // Ensure first position is valid

            //player.transform.position = new Vector3(spawnX, spawnY, player.transform.position.z);
        }

        public override void OnNetworkSpawn()
        {
            
            AdjustPlayerSize();
            
            

            if (IsOwner)
            {
                var chunkManager = FindFirstObjectByType<ChunkManager>();

                if (chunkManager != null)
                {
                    chunkManager.player = transform;
                }
            }
            
        }

        void AdjustPlayerSize()
        {
            player.transform.localScale = new Vector3(1f, 1f, 1f);
        }

        /*
        Vector3 FindSafeSpawn()
        {
            Vector3Int spawnTilePos = tilemap.WorldToCell(Vector3.zero); // Start searching from (0,0)
            int searchRadius = 10; // How far to search for land

            for (int r = 0; r < searchRadius; r++)
            {
                for (int x = -r; x <= r; x++)
                {
                    for (int y = -r; y <= r; y++)
                    {
                        Vector3Int checkPos = spawnTilePos + new Vector3Int(x, y, 0);
                        TileBase tile = tilemap.GetTile(checkPos);

                        if (tile != waterTile) // Found land!
                        {
                            return tilemap.CellToWorld(checkPos) + new Vector3(0.5f, 0.5f, 0); // Center player on tile
                        }
                    }
                }
            }

            // Fallback (if no land found, use default spawn)
            return Vector3.zero;
        }*/

        private void Update()
        {
            if (!IsOwner) return;

            // Capture input for movement
            Vector3Int playerTilePos = tilemap.WorldToCell(player.transform.position);
            TileBase currentTile = tilemap.GetTile(playerTilePos);

            movement.x = Input.GetAxisRaw("Horizontal"); // Left/Right
            movement.y = Input.GetAxisRaw("Vertical"); // Up/Down

            if (movement.magnitude > 1)
            {
                movement.Normalize();
            }
        }

        void FixedUpdate()
        {
            if (!IsOwner) return;
            // Apply movement to the Rigidbody2D
            rb.MovePosition(rb.position + movement * (moveSpeed * Time.fixedDeltaTime));
        }
    }
}