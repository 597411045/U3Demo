using Newtonsoft.Json.Linq;

namespace RPG.Saving
{
    public interface IJsonSaveable
    {
        JToken CaptureASJToken();
        void RestoreFormJTkoen(JToken state);
    }
}