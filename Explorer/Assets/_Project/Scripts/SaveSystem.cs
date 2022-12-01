using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveSystem
{
    private const string saveFilePath = "userData.save";

    public static void SavePlayer(UserDataReferenceHelper referenceHelper)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Path.Combine(Application.persistentDataPath, saveFilePath);
        FileStream stream = new FileStream(path, FileMode.Create);
        SerializableUserData data = new SerializableUserData(referenceHelper);
        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static SerializableUserData LoadPlayer()
    {
        string path = Path.Combine(Application.persistentDataPath, saveFilePath);
        
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            SerializableUserData data = formatter.Deserialize(stream) as SerializableUserData;
            stream.Close();
            return data;
        }
        else
        {
            return null;
        }
    }
}
