using Newtonsoft.Json.Linq;

namespace RPG.Saving
{
    public interface IJsonSaveable
    {
        JToken CaptureAsJTokenInInterface();
        void RestoreFormJToken(JToken state);
    }
}