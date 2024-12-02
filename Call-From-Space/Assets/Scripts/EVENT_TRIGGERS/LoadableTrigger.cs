using Newtonsoft.Json.Linq;
public abstract class LoadableTrigger : Loadable
{
    public override void Load(JObject state)
    {
        bool active = (bool)state[fullName]["isActive"];
        gameObject.SetActive(active);
    }

    public override void Save(ref JObject state)
    {
        state[fullName] = new JObject()
        {
            ["isActive"] = gameObject.activeSelf
        };
    }
}
