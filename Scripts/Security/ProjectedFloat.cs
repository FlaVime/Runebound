using UnityEngine;

[System.Serializable]
public struct ProjectedFloat
{
    [SerializeField] private int encryptedValue;
    [SerializeField] private int encryptionKey;

    public void Set(float value)
    {
        encryptionKey = Random.Range(int.MinValue, int.MaxValue);
        int floatBits = System.BitConverter.ToInt32(System.BitConverter.GetBytes(value), 0); // Convert float to int bits
        encryptedValue = floatBits ^ encryptionKey; // XOR encryption
    }

    public float Value
    {
        get
        {
            int decryptedBits = encryptedValue ^ encryptionKey;
            return System.BitConverter.ToSingle(System.BitConverter.GetBytes(decryptedBits), 0);
        }
        set => Set(value);
    }

    public static implicit operator float(ProjectedFloat projectedFloat) => projectedFloat.Value;
    public static implicit operator ProjectedFloat(float value)
    {
        ProjectedFloat projectedFloat = new ProjectedFloat();
        projectedFloat.Set(value);
        return projectedFloat;
    }
}