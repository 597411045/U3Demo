using System.Collections.Generic;

namespace RPG.Stats
{
    public interface IModifierProvider
    {
        IEnumerable<float> GetAdditiveModifier(ProgressionEnum b);
        IEnumerable<float> GetPercentageModifier(ProgressionEnum b);
    }
}