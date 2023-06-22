using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class SaveGameManager
{
    private static SaveGameManager _instance = null;
    public SaveGame SaveGame = new SaveGame();

    private SaveGameManager()
    {
        Load();
    }
    
    public static SaveGameManager Instance
    {
        get { return _instance ??= new SaveGameManager(); }
    }

    public void Save()
    {
        BinaryFormatter bf = new BinaryFormatter(); 
        FileStream file = File.Create(Application.persistentDataPath 
                                      + "/MySaveData.dat"); 
        SaveGame data = new SaveGame();
        bf.Serialize(file, data);
        file.Close();
        Debug.Log("Game data saved!"); 
    }
    
    public void Load()
    {
        if (File.Exists(Application.persistentDataPath 
                        + "/MySaveData.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = 
                File.Open(Application.persistentDataPath 
                          + "/MySaveData.dat", FileMode.Open);
            SaveGame = (SaveGame)bf.Deserialize(file);
            file.Close();
            Debug.Log("Game data loaded!");
        }
        else
            Debug.LogError("There is no save data!");
    }
}
