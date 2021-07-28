local BaseCtrl = class()

BaseCtrl.viewPath = nil

BaseCtrl.dialogStatus = {
    touchClose = true,
    withShadow = true,
    unblockRaycast = false,
    noNeedSafeArea = false,
}

BaseCtrl.withoutPop = false        -- true:当这个界面显示时，清除掉之前的ctrl的堆栈
BaseCtrl.needSceneCache = true     -- view显示对象是否需要缓存，且不需要添加到res.ctrlStack堆栈里

function BaseCtrl:GetLoadType()
    return self.__loadType
end

function BaseCtrl:ctor(view, ...)
    self.view = view
    self:Init(...)
end

function BaseCtrl:Init(...)

end

function BaseCtrl:Refresh(...)
    if self.withoutPop == true then
        res.ClearCtrlStack()
    end
    self:OnEnterScene()
end

function BaseCtrl:GetEventList()
    return {}
end

function BaseCtrl:OnEnterScene()
    local eventList = self:GetEventList()
    for eventName, func in pairs(eventList) do
        require("EventSystem").AddEvent(eventName, self, func)
    end
end

function BaseCtrl:OnExitScene()
    local eventList = self:GetEventList()
    for eventName, func in pairs(eventList) do
        require("EventSystem").RemoveEvent(eventName, self, func)
    end
end

function BaseCtrl:GetStatusData()

end

setmetatable(BaseCtrl, unity.asyncmeta)

_G["BaseCtrl"] = BaseCtrl
-- return BaseCtrl
