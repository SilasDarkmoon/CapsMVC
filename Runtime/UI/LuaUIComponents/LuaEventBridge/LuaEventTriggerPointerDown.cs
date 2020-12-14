using UnityEngine.EventSystems;

namespace Lua.UI.Event
{
    public class LuaEventTriggerPointerDown : LuaEventTriggerBase, IPointerDownHandler
    {
        public void OnPointerDown(PointerEventData eventData)
        {
            luaBehaviour.CallLuaFunc<PointerEventData>("onPointerDown", eventData);
        }
    }
}