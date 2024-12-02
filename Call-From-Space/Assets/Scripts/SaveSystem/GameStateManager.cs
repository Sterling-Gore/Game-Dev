using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEngine;

class GameStateManager
{
    public const string saveFilePath = "saveData.json";
    public const string checkPointFilePath = "checkPoint.json";
    public JObject state = new JObject();
    private static GameStateManager _instance = null;
    public static GameStateManager instance =>
        _instance ??= new();
    public static bool startedNewGame = false;

    public void NewGame()
    {
        state = new JObject();
        startedNewGame = true;
    }

    public void LoadGame(string saveFile)
    {
        try
        {
            state = JObject.Parse(File.ReadAllText(saveFile));
        }
        catch
        {
            Debug.Log("starting new game");
            NewGame();
        }
        for (int i = 0; i < Loadable.loadables.Count; i++)
        {
            try
            {
                Debug.Log(Loadable.loadables[i].fullName);
                Loadable.loadables[i].Load(state);
            }
            catch (Exception e)
            {
                Debug.LogWarning($"error loading {Loadable.loadables[i].fullName}: {e}");
            }
        };
    }

    public void SaveGame(string saveFile)
    {
        foreach (Loadable loadable in Loadable.loadables)
        {
            try
            {
                loadable.Save(ref state);
            }
            catch (Exception e)
            {
                Debug.LogWarning($"error saving {loadable.fullName}: {e}");
            }
        };
        File.WriteAllText(saveFile, state.ToString());
    }

    public void UnSaveGame(string saveFile)
    {
        if (File.Exists(saveFile))
            File.Delete(saveFile);
    }
}