using Unity.Netcode;
using Unity.Collections;
using System;

[System.Serializable]
public struct Item : INetworkSerializable, IEquatable<Item>
{
    public FixedString64Bytes itemName; // Nom de l'item (FixedString pour Netcode)
    public int iconId;  // ID de l'icône
    public int maxStack; // Nombre max dans un slot

    public Item(string name, int icon, int stack)
    {
        itemName = name;
        iconId = icon;
        maxStack = stack;
    }

    // Implémentation de la sérialisation réseau
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref itemName);
        serializer.SerializeValue(ref iconId);
        serializer.SerializeValue(ref maxStack);
    }

    // Implémentation de IEquatable<Itemss>
    public bool Equals(Item other)
    {
        return itemName.Equals(other.itemName) && iconId == other.iconId && maxStack == other.maxStack;
    }

    public override bool Equals(object obj)
    {
        return obj is Item other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(itemName.GetHashCode(), iconId, maxStack);
    }

    public static bool operator ==(Item left, Item right)
    {
        return left.itemName == right.itemName;
    }

    public static bool operator !=(Item left, Item right)
    {
        return !(left == right);
    }
}