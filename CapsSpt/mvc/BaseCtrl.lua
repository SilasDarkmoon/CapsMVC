local BaseCtrl = class()

BaseCtrl.viewPath = nil

BaseCtrl.dialogStatus = {
    touchClose = true,
    withShadow = true,
    unblockRaycast = false,
    noNeedSafeArea = false,
}

BaseCtrl.withoutPop = false
BaseCtrl.needSceneCache = true     -- ctrl是否需要缓存

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

_G["BaseCtrl"] = BaseCtrl
-- return BaseCtrl
