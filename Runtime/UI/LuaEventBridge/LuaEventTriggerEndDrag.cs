using UnityEngine.EventSystems;

namespace Lua.UI.Event
{
    public class LuaEventTriggerEndDrag : LuaEventTriggerBase, IEndDragHandler
    {
        public void OnEndDrag(PointerEventData eventData)
        {
            luaBehaviour.CallLuaFunc<PointerEventData>("onEndDrag", eventData);
        }
    }
}