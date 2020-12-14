using UnityEngine.EventSystems;

namespace Lua.UI.Event
{
    public class LuaEventTriggerScroll : LuaEventTriggerBase, IScrollHandler
    {
        public void OnScroll(PointerEventData eventData)
        {
            luaBehaviour.CallLuaFunc<PointerEventData>("onScroll", eventData);
        }
    }
}