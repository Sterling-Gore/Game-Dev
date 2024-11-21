using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Unity.VisualScripting;
using UnityEngine;

public abstract class Loadable : MonoBehaviour
{
    public static List<Loadable> loadables = new();

    /// <summary>
    /// when overriding make sure to call base.Awake();
    /// </summary>
    protected virtual void Awake()
    {
        loadables.Add(this);
    }

    /// <summary>
    /// when overriding make sure to call base.Destroy();
    /// and make sure to save the state
    /// </summary>
    protected virtual void Destroy()
    {
        loadables.Remove(this);
    }
    public abstract void Save(ref JObject state);
    public abstract void Load(JObject state);

    public JObject VectorToJson(Vector3 vec) => new()
    {
        ["x"] = vec.x,
        ["y"] = vec.y,
        ["z"] = vec.z
    };

    public void LoadTransform(JObject state)
    {
        var player = state[name];
        var pos = player["pos"];
        var rot = player["rot"];
        transform.SetPositionAndRotation(
          new Vector3((float)pos["x"], transform.position.y, (float)pos["z"]),
          Quaternion.Euler((float)rot["x"], (float)rot["y"], (float)rot["z"])
        );
    }

    public void SaveTransform(ref JObject state)
    {
        if (state[name] == null)
            state[name] = new JObject();
        state[name]["pos"] = VectorToJson(transform.position);
        state[name]["rot"] = VectorToJson(transform.rotation.eulerAngles);
    }
}