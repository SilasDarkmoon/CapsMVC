using UnityEngine.EventSystems;

namespace Lua.UI.Event
{
    public class LuaEventTriggerSubmit : LuaEventTriggerBase, ISubmitHandler
    {
        public void OnSubmit(BaseEventData eventData)
        {
            luaBehaviour.CallLuaFunc<BaseEventData>("onSubmit", eventData);
        }
    }
}