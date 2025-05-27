using RedstoneinventeGameStudio;
using Unity.Netcode;
using UnityEngine;

public class DroppedItem : NetworkBehaviour
{
    [Header("Composants")]
    public NetworkObject networkObject;
    public Rigidbody2D rb;
    
    [Header("Données de l'objet")]
    public InventoryItemData item;
    
    [Header("Paramètres")]
    public float distanceProximiteRecherche = 2f;
    public int profondeurMaxRecherche = 10;
    
    [Header("Debug")]
    public bool enableDebugLogs = true;

    private bool objetTraite = false;
    private bool estAttrapeParConstructeur = false;

    private void Start()
    {
        if (rb != null)
        {
            rb.gravityScale = 0;
            rb.linearDamping = 0.5f;
        }
    }

    private void FixedUpdate()
    {
        // Si l'objet est attrapé par un constructeur, on stoppe sa vitesse
        if (estAttrapeParConstructeur && rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.isKinematic = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (objetTraite) return;

        if (enableDebugLogs)
        {
            Debug.Log($"=== TRIGGER ENTER ===");
            Debug.Log($"DroppedItem: {item?.itemName} x{item?.itemNb}");
            Debug.Log($"Collision avec: {collision.gameObject.name}");
        }

        if (!EstUneMachine(collision.gameObject))
        {
            if (enableDebugLogs)
                Debug.LogWarning($"❌ Objet {collision.gameObject.name} n'est PAS une machine");
            return;
        }

        if (enableDebugLogs)
            Debug.Log($"✓ Objet {collision.gameObject.name} est bien une machine");

        TraiterInteractionMachine(collision.gameObject);
    }

    private bool EstUneMachine(GameObject objet)
    {
        return objet.layer == LayerMask.NameToLayer("Machines");
    }

    private void TraiterInteractionMachine(GameObject machine)
    {
        string tag = machine.gameObject.tag;
        Debug.Log($"TraiterInteractionMachine: {tag}");
        switch (tag)
        {
            case "Furnace":
                if (enableDebugLogs) Debug.Log($"➤ Détection FOUR");
                GererInteractionFour(machine);
                break;
            case "Constructor":
                if (enableDebugLogs) Debug.Log($"➤ Détection CONSTRUCTEUR");
                estAttrapeParConstructeur = true; // Marquer comme attrapé par constructeur
                GererInteractionConstructeur(machine);
                break;
                
            default:
                if (enableDebugLogs)
                    Debug.LogWarning($"❌ Tag non reconnu pour {machine.name}: '{tag}'");
                break;
        }
    }

    private void GererInteractionFour(GameObject objetFour)
    {
        FurnaceInteraction furnaceInteraction = objetFour.GetComponent<FurnaceInteraction>();
        if (furnaceInteraction == null)
        {
            if (enableDebugLogs)
                Debug.LogWarning($"Composant FurnaceInteraction non trouvé sur {objetFour.name}");
            return;
        }

        FurnaceUsing furnaceUsing = furnaceInteraction.furnaceScript;
        if (furnaceUsing == null || furnaceUsing.InputCard == null)
        {
            if (enableDebugLogs)
                Debug.LogWarning($"FurnaceUsing ou InputCard non trouvé sur {objetFour.name}");
            return;
        }

        CardManager gestionnaireCarte = furnaceUsing.InputCard.GetComponent<CardManager>();
        if (gestionnaireCarte == null)
        {
            if (enableDebugLogs)
                Debug.LogWarning($"CardManager non trouvé sur InputCard de {objetFour.name}");
            return;
        }

        if (PeutAjouterObjet(gestionnaireCarte))
        {
            AjouterObjetACarte(gestionnaireCarte);
            DetruireObjetTombe();
        }
    }

    private void GererInteractionConstructeur(GameObject objetConstructeur)
    {
        ConstructeurUsing constructeurUsing = RecupererConstructeurUsing(objetConstructeur);
        
        if (constructeurUsing == null)
        {
            if (enableDebugLogs)
                Debug.LogError($"❌ Composant ConstructeurUsing non trouvé sur {objetConstructeur.name}");
            return;
        }

        if (TenterAjoutConstructeur(constructeurUsing))
        {
            DetruireObjetTombe();
        }
        else
        {
            // Si l'objet n'a pas pu être ajouté, on le libère
            estAttrapeParConstructeur = false;
            if (rb != null) rb.isKinematic = false;
        }
    }

    private ConstructeurUsing RecupererConstructeurUsing(GameObject objetConstructeur)
    {
        FurnaceInteraction f = objetConstructeur.GetComponent<FurnaceInteraction>();
        if (f == null) Debug.LogError("furnaceInteraction null");
        return f.constructeurScript;
    }
    private ConstructeurUsing RechercherConstructeurParProximite(GameObject objetConstructeur)
    {
        if (objetConstructeur == null) return null;

        // Trouver tous les ConstructeurUsing dans la scène
        ConstructeurUsing[] tousConstructeurs = FindObjectsOfType<ConstructeurUsing>();
        ConstructeurUsing constructeurLePlusProche = null;
        float distanceMinimale = float.MaxValue;

        foreach (var constructeur in tousConstructeurs)
        {
            float distance = Vector3.Distance(constructeur.transform.position, objetConstructeur.transform.position);
        
            // Vérifier si le constructeur est assez proche et plus proche que le précédent trouvé
            if (distance < distanceProximiteRecherche && distance < distanceMinimale)
            {
                distanceMinimale = distance;
                constructeurLePlusProche = constructeur;
            }
        }

        if (enableDebugLogs && constructeurLePlusProche != null)
        {
            Debug.Log($"Constructeur trouvé par proximité: {constructeurLePlusProche.gameObject.name} " +
                      $"(Distance: {distanceMinimale:F2}, Seuil: {distanceProximiteRecherche})");
        }

        return constructeurLePlusProche;
    }
    private bool TenterAjoutConstructeur(ConstructeurUsing constructeur)
    {
        // Essayer CardIn1
        if (TenterAjoutCarte(constructeur, 1))
            return true;

        // Essayer CardIn2
        if (TenterAjoutCarte(constructeur, 2))
            return true;

        return false;
    }

    private bool TenterAjoutCarte(ConstructeurUsing constructeur, int numeroCarte)
    {
        GameObject carte = RecupererCarte(constructeur, numeroCarte);
        if (carte == null) return false;

        CardManager cardManager = carte.GetComponent<CardManager>();
        if (cardManager == null) return false;

        if (!PeutAjouterObjet(cardManager)) return false;

        AjouterObjetACarte(cardManager);
        return true;
    }

    private GameObject RecupererCarte(ConstructeurUsing constructeur, int numeroCarte)
    {
        if (numeroCarte == 1 && constructeur.CardIn1 != null)
            return constructeur.CardIn1;
        if (numeroCarte == 2 && constructeur.CardIn2 != null)
            return constructeur.CardIn2;

        // Recherche par nom si les références directes sont nulles
        string nomCarte = $"Card - Inventory ({numeroCarte})";
        return TrouverCarteParNom(constructeur.transform, nomCarte);
    }

    private GameObject TrouverCarteParNom(Transform parent, string nomCarte)
    {
        foreach (Transform enfant in parent)
        {
            if (enfant.name.Equals(nomCarte, System.StringComparison.OrdinalIgnoreCase))
                return enfant.gameObject;

            GameObject resultat = TrouverCarteParNom(enfant, nomCarte);
            if (resultat != null)
                return resultat;
        }
        return null;
    }

    private bool PeutAjouterObjet(CardManager gestionnaireCarte)
    {
        if (gestionnaireCarte == null) return false;

        InventoryItemData objetExistant = gestionnaireCarte.itemData;
        
        // Emplacement vide
        if (objetExistant == null || objetExistant.itemNb <= 0)
            return true;
        
        // Même type d'objet
        if (objetExistant.itemName == item.itemName)
            return true;
        
        return false;
    }

    private void AjouterObjetACarte(CardManager gestionnaireCarte)
    {
        if (gestionnaireCarte == null || item == null) return;

        InventoryItemData objetExistant = gestionnaireCarte.itemData;
        
        if (objetExistant == null || objetExistant.itemNb <= 0)
        {
            gestionnaireCarte.SetItem(item);
        }
        else if (objetExistant.itemName == item.itemName)
        {
            int nouvelleQuantite = objetExistant.itemNb + item.itemNb;
            InventoryItemData objetCombine = objetExistant.CreateCopyWithQuantity(nouvelleQuantite);
            
            gestionnaireCarte.UnSetItem();
            gestionnaireCarte.SetItem(objetCombine);
        }
    }

    private void DetruireObjetTombe()
    {
        objetTraite = true;
        
        if (IsOwner)
        {
            if (networkObject != null && networkObject.IsSpawned)
            {
                networkObject.Despawn(true);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (item != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, 0.5f);
            
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, distanceProximiteRecherche);
            
            UnityEditor.Handles.Label(transform.position + Vector3.up * 0.7f, 
                                    $"{item.itemName} x{item.itemNb}");
        }
    }
#endif
}