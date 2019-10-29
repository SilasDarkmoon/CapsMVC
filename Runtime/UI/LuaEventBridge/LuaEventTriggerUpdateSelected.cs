using UnityEngine.EventSystems;

namespace Lua.UI.Event
{
    public class LuaEventTriggerUpdateSelected : LuaEventTriggerBase, IUpdateSelectedHandler
    {
        public void OnUpdateSelected(BaseEventData eventData)
        {
            luaBehaviour.CallLuaFunc<BaseEventData>("onUpdateSelected", eventData);
        }
    }
}