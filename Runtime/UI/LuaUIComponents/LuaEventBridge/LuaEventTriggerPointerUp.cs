using UnityEngine.EventSystems;

namespace Lua.UI.Event
{
    public class LuaEventTriggerPointerUp : LuaEventTriggerBase, IPointerUpHandler
    {
        public void OnPointerUp(PointerEventData eventData)
        {
            luaBehaviour.CallLuaFunc<PointerEventData>("onPointerUp", eventData);
        }
    }
}