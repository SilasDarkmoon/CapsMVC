using UnityEngine.EventSystems;

namespace Lua.UI.Event
{
    public class LuaEventTriggerInitializePotentialDrag : LuaEventTriggerBase, IInitializePotentialDragHandler
    {
        public void OnInitializePotentialDrag(PointerEventData eventData)
        {
            luaBehaviour.CallLuaFunc<PointerEventData>("onInitializePotentialDrag", eventData);
        }
    }
}