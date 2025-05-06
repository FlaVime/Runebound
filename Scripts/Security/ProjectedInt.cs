using UnityEngine;

[System.Serializable]
public struct ProjectedInt
{
    [SerializeField] private int encryptedValue;
    [SerializeField] private int encryptionKey;

    public void Set(int value)
    {
        encryptionKey = Random.Range(int.MinValue, int.MaxValue);
        encryptedValue = value ^ encryptionKey; // XOR encryption
    }

    public int Value
    {
        get => encryptedValue ^ encryptionKey; // Decrypt the value
        set
        {
            encryptionKey = Random.Range(int.MinValue, int.MaxValue); // Change the key for security
            encryptedValue = value ^ encryptionKey; // Encrypt the new value
        }
    }

    public static implicit operator int(ProjectedInt projectedInt) => projectedInt.Value;
    public static implicit operator ProjectedInt(int value)
    {
        ProjectedInt projectedInt = new ProjectedInt();
        projectedInt.Set(value);
        return projectedInt;
    }
}