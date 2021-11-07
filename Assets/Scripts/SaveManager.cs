using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;

public static class SaveManager
{

    public static string directory = "/SaveData/";
    public static string fileName = "Highscore.txt";

    public static void SaveData(PlayerScore score) 
    {
        string dir = Application.persistentDataPath + directory;

        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir);

        string json = JsonUtility.ToJson(score);
        File.WriteAllText(dir + fileName, json);
    }

    public static PlayerScore LoadScore() 
    {
        string fullPath = Application.persistentDataPath + directory + fileName;

        PlayerScore score = new PlayerScore();

        if (File.Exists(fullPath))
        {
            string json = File.ReadAllText(fullPath);
            score = JsonUtility.FromJson<PlayerScore>(json);
        }
        else 
        {
            Debug.Log("No save file");
        }

        return score;
    }

}
