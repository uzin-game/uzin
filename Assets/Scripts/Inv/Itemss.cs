using Unity.Netcode;
using Unity.Collections;
using System;

[System.Serializable]
public struct Itemss : INetworkSerializable, IEquatable<Itemss>
{
    public FixedString64Bytes itemName; // Nom de l'item (FixedString pour Netcode)
    public int iconId;  // ID de l'icône
    public int maxStack; // Nombre max dans un slot

    public Itemss(string name, int icon, int stack)
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
    public bool Equals(Itemss other)
    {
        return itemName.Equals(other.itemName) && iconId == other.iconId && maxStack == other.maxStack;
    }

    public override bool Equals(object obj)
    {
        return obj is Itemss other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(itemName.GetHashCode(), iconId, maxStack);
    }

    public static bool operator ==(Itemss left, Itemss right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Itemss left, Itemss right)
    {
        return !(left == right);
    }
}