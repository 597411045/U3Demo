using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;

public static class JsonStatics
{
    public static JToken ToToken(this Vector3 v)
    {
        JObject state = new JObject();
        IDictionary<string, JToken> stateDict = state;
        stateDict["x"] = v.x;
        stateDict["y"] = v.y;
        stateDict["z"] = v.z;
        return state;
    }

    public static Vector3 ToVector3(this JToken s)
    {
        Vector3 v = new Vector3();
        if (s is JObject j)
        {
            IDictionary<string, JToken> stateDict = j;
            if (stateDict.TryGetValue("x", out JToken x))
            {
                v.x = x.ToObject<float>();
            }
            if (stateDict.TryGetValue("y", out JToken y))
            {
                v.y = y.ToObject<float>();
            }
            if (stateDict.TryGetValue("z", out JToken z))
            {
                v.z = z.ToObject<float>();
            }
        }

        return v;
    }
}