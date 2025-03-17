using System.Collections.Generic;
using UnityEngine;

public class ItemDatabase : MonoBehaviour
{
    public static ItemDatabase Instance;
    public Dictionary<int, Sprite> iconLibrary;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        iconLibrary = new Dictionary<int, Sprite>
        {
            { 0, Resources.Load<Sprite>("Icons/Carré_rouge") },
            { 1, Resources.Load<Sprite>("Icons/Fer") },
            { 2, Resources.Load<Sprite>("Icons/Cuivre") },
            { 4, Resources.Load<Sprite>("Icons/minerais fer")}
        };
    }

    public Sprite GetSprite(int iconId)
    {
        return iconLibrary.TryGetValue(iconId, out var sprite) ? sprite : null;
    }
}