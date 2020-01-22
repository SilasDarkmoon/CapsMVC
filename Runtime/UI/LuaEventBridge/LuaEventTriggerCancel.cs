using UnityEngine.EventSystems;

namespace Lua.UI.Event
{
    public class LuaEventTriggerCancel : LuaEventTriggerBase, ICancelHandler
    {
       
        public void OnCancel(BaseEventData eventData)
        {
            luaBehaviour.CallLuaFunc<BaseEventData>("onCancel", eventData);
        }
    }
}