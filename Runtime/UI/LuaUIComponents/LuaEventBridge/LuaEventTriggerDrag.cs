using UnityEngine.EventSystems;

namespace Lua.UI.Event
{
    public class LuaEventTriggerDrag : LuaEventTriggerBase, IDragHandler
    {
        public void OnDrag(PointerEventData eventData)
        {
            luaBehaviour.CallLuaFunc<PointerEventData>("onDrag", eventData);
        }
    }
}