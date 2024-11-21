using System;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

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
            Loadable.loadables.ForEach(loadable => loadable.Load(state));
        }
        catch
        {
            NewGame();
        }
    }

    public void SaveGame(string saveFile)
    {
        Loadable.loadables.ForEach(loadable => loadable.Save(ref state));
        File.WriteAllText(saveFile, state.ToString());
    }

    public void UnSaveGame(string saveFile)
    {
        if (File.Exists(saveFile))
            File.Delete(saveFile);
    }
}