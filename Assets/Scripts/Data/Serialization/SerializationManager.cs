using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Runtime.Serialization;

public class SerializationManager : MonoBehaviour
{
    public static string filePath = Application.persistentDataPath + "/saves";

    public static bool Save(string saveName, object saveData) {

        BinaryFormatter fromatter = GetBinaryFormatter();
        Debug.Log("file path: " + filePath);

        if (!Directory.Exists(filePath)) {
            Debug.Log("create path!");
            Directory.CreateDirectory(filePath);
        }

        string path = filePath + "/" + saveName + ".save";
        Debug.Log("path: " + path);
        FileStream file = File.Create(path);
        fromatter.Serialize(file, saveData);
        file.Close();

        return true;
    }

    public static object Load(string path) {
        if (!File.Exists(path)) {
            Debug.Log("doesnt exist");
            return null;
        }
        BinaryFormatter fromatter = GetBinaryFormatter();

        FileStream file = File.Open(path, FileMode.Open);

        try {
            object save = fromatter.Deserialize(file);
            file.Close();
            return save;
        } catch {
            Debug.LogErrorFormat("Failed to load file at {0}", path);
            file.Close();
            return null;
        }


    }

    public static BinaryFormatter GetBinaryFormatter() {
        BinaryFormatter formatter = new BinaryFormatter();
        SurrogateSelector selector = new SurrogateSelector();

        Vector3SerializationSurrogate vector3Surrogate = new Vector3SerializationSurrogate();
        QuaternionSerializationSurrogate quaternionSurrogate = new QuaternionSerializationSurrogate();
        ColorSerializationSurrogate colorSurrogate = new ColorSerializationSurrogate();

        selector.AddSurrogate(typeof(Vector3), new StreamingContext(StreamingContextStates.All), vector3Surrogate);
        selector.AddSurrogate(typeof(Quaternion), new StreamingContext(StreamingContextStates.All), quaternionSurrogate);
        selector.AddSurrogate(typeof(Color), new StreamingContext(StreamingContextStates.All), colorSurrogate);
        
        formatter.SurrogateSelector = selector;
        return formatter;
    }
}
