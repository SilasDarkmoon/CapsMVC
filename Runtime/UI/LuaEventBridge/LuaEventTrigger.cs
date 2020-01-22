using UnityEngine;

namespace Lua.UI.Event
{
    public class LuaEventTriggerBase : MonoBehaviour
    {
        public CapsUnityLuaBehav luaBehaviour;

#if UNITY_EDITOR
        /// <summary>
        /// 默认选择自己或与自己最近父节点的LuaBehavior
        /// </summary>
        protected void Reset()
        {
            Transform trans = this.transform;
            while (trans != null)
            {
                var luaBehaviComp = trans.GetComponent<CapsUnityLuaBehav>();
                if (luaBehaviComp)
                {
                    this.luaBehaviour = luaBehaviComp;
                    break;
                }
                else
                {
                    trans = trans.parent;
                }
            }
        }
#endif
    }

    public class LuaEventTrigger : MonoBehaviour
    {

    }
}
