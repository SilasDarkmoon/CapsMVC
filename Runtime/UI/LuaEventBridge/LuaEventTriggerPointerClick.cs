using UnityEngine.EventSystems;

namespace Lua.UI.Event
{
    public class LuaEventTriggerPointerClick : LuaEventTriggerBase, IPointerClickHandler
    {
        public void OnPointerClick(PointerEventData eventData)
        {
            luaBehaviour.CallLuaFunc<PointerEventData>("onPointerClick", eventData);
        }
    }
}