using UnityEngine.EventSystems;

namespace Lua.UI.Event
{
    public class LuaEventTriggerDrop : LuaEventTriggerBase, IDropHandler
    {
        public void OnDrop(PointerEventData eventData)
        {
            luaBehaviour.CallLuaFunc<PointerEventData>("onDrop", eventData);
        }
    }
}