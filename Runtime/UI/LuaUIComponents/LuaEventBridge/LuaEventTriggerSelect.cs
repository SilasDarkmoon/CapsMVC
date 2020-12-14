using UnityEngine.EventSystems;

namespace Lua.UI.Event
{
    public class LuaEventTriggerSelect : LuaEventTriggerBase, ISelectHandler
    {
        public void OnSelect(BaseEventData eventData)
        {
             luaBehaviour.CallLuaFunc<BaseEventData>("onSelect", eventData);
        }
    }
}