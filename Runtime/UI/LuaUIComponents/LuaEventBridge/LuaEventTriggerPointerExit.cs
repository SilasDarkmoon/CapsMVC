using UnityEngine.EventSystems;

namespace Lua.UI.Event
{
    public class LuaEventTriggerPointerExit : LuaEventTriggerBase, IPointerExitHandler
    {
        public void OnPointerExit(PointerEventData eventData)
        {
            luaBehaviour.CallLuaFunc<PointerEventData>("onPointerExit", eventData);
        }
    }
}