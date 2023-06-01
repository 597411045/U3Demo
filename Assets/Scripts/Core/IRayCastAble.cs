using RPG.Control;
using UnityEngine;

namespace RPG.Core
{
    public interface IRayCastAble
    {
        bool HandleRaycaset(PlayerController p, RaycastHit h);
    }
}