using UnityEngine.EventSystems;

namespace Lua.UI.Event
{
    public class LuaEventTriggerMove : LuaEventTriggerBase, IMoveHandler
    {
        public void OnMove(AxisEventData eventData)
        {
            luaBehaviour.CallLuaFunc<AxisEventData>("onMove", eventData);
        }
    }
}