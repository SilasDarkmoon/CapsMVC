using UnityEngine.EventSystems;

namespace Lua.UI.Event
{
    public class LuaEventTriggerBeginDrag : LuaEventTriggerBase, IBeginDragHandler
    {
        public void OnBeginDrag(PointerEventData eventData)
        {
            luaBehaviour.CallLuaFunc<PointerEventData>("onBeginDrag", eventData);
        }
    }
}
