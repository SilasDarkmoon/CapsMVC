using UnityEngine.EventSystems;

namespace Lua.UI.Event
{
    public class LuaEventTriggerPointerEnter : LuaEventTriggerBase, IPointerEnterHandler
    {
        public void OnPointerEnter(PointerEventData eventData)
        {
            luaBehaviour.CallLuaFunc<PointerEventData>("onPointerEnter", eventData);
        }
    }
}