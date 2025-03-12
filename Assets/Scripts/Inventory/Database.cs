using System.Collections.Generic;
using UnityEngine;

public class ItemDatabase : MonoBehaviour
{
    public static ItemDatabase Instance;
    public Dictionary<int, Sprite> iconLibrary;

    private void Awake()
    {
        Instance = this;
        iconLibrary = new Dictionary<int, Sprite>
        {
            { 0, Resources.Load<Sprite>("Icons/default_icon") },
            { 1, Resources.Load<Sprite>("Icons/sword_icon") },
            { 2, Resources.Load<Sprite>("Icons/shield_icon") }
        };
    }

    public Sprite GetSprite(int iconId)
    {
        return iconLibrary.TryGetValue(iconId, out var sprite) ? sprite : null;
    }
}