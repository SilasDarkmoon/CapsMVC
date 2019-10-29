using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

namespace Lua.UI.Event
{
    public class LuaEventTriggerDeselect : LuaEventTriggerBase, IDeselectHandler
    {
        public void OnDeselect(BaseEventData eventData)
        {
            luaBehaviour.CallLuaFunc<BaseEventData>("onDeselect", eventData);
        }
    }
}