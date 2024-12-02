using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;

public abstract class Loadable : MonoBehaviour
{
    public static List<Loadable> loadables = new();
    public string fullName;

    /// <summary>
    /// when overriding make sure to call base.Awake();
    /// </summary>
    protected virtual void Awake()
    {
        fullName = GetFullName(transform);
        loadables.Add(this);
    }

    /// <summary>
    /// when overriding make sure to call base.OnDestroy();
    /// and make sure to save the state
    /// </summary>
    protected virtual void OnDestroy()
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

    public Vector3 JsonToVector(JToken vec) =>
        new((float)vec["x"], (float)vec["y"], (float)vec["z"]);

    public void LoadTransform(JObject state)
    {
        var player = state[fullName];
        var pos = player["pos"];
        var rot = player["rot"];
        transform.SetPositionAndRotation(
            JsonToVector(pos),
            Quaternion.Euler(JsonToVector(rot))
        );
    }

    public void SaveTransform(ref JObject state)
    {
        if (state[fullName] == null)
            state[fullName] = new JObject();
        state[fullName]["pos"] = VectorToJson(transform.position);
        state[fullName]["rot"] = VectorToJson(transform.eulerAngles);
    }

    public string GetFullName(Transform transform)
    {
        if (transform == null) return "";
        return GetFullName(transform.parent) + "/" + transform.name;
    }
}