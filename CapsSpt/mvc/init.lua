local ResManager = clr.Capstones.UnityEngineEx.ResManager
local UIResManager = clr.Capstones.UnityEngineEx.UIResManager
local UnityEngine = clr.UnityEngine
-- local LightmapData = UnityEngine.LightmapData
-- local LightmapSettings = UnityEngine.LightmapSettings
local Object = UnityEngine.Object
local GameObject = UnityEngine.GameObject
local Canvas = UnityEngine.Canvas
-- local Image = UnityEngine.UI.Image
-- local CanvasScaler = UnityEngine.UI.CanvasScaler
-- local GraphicRaycaster = UnityEngine.UI.GraphicRaycaster
-- local RectTransform = UnityEngine.RectTransform
-- local Vector2 = UnityEngine.Vector2
-- local Camera = UnityEngine.Camera
local RenderMode = UnityEngine.RenderMode
local CapsUnityLuaBehav = clr.CapsUnityLuaBehav
-- local Time = UnityEngine.Time
-- local StandaloneInputModule = UnityEngine.EventSystems.StandaloneInputModule

local res = require("unity.res")

local unmanagedDialogs = {}
local unmanagedBlockDialogs =
{
    ["Game/UI/Common/Template/Loading/WaitForPost2.prefab"] = true,
}

--#region Basic and Override
function res.IsClrNull(obj)
    return obj == nil or obj == clr.null
end

function res.SetUICamera(obj, camera)
    if obj then
        local canvas = obj:GetComponent(Canvas)
        if canvas and canvas ~= clr.null then
            canvas.worldCamera = camera
        end
    end
end

function res.Instantiate(name)
    local prefab = ResManager.LoadRes(name)
    if prefab then
        local obj = Object.Instantiate(prefab)
        if obj then
            local canvas = obj:GetComponent(Canvas)
            if canvas and canvas ~= clr.null then
                res.SetUICamera(obj, UIResManager.FindUICamera())
            end
            return obj, res.GetLuaScript(obj)
        end
    end
end

function res.GetDialogCamera()
    -- return UIResManager.GetUIDialogCamera()
    return UIResManager.FindUICamera()
end

function res.GetMainCamera()
    return UIResManager.FindUICamera()
end
--#endregion Basic and Override

--#region View Cache Stack
--[[
res.curSceneInfo = {
    view = nil,
    ctrl = nil,
    path = nil,
    blur = true,
    dialogs = {
        {
            view = nil,
            ctrl = nil,
            path = nil,
            order = xxx,
        },
        {
            view = nil,
            ctrl = nil,
            path = nil,
            order = xxx,
        },
        {
            view = nil,
            ctrl = nil,
            path = nil,
            order = xxx,
        },
    },
}
--]]

res.LoadType = {
    Change = "change",
    Push = "push",
    Pop = "pop",
}

res.sceneSeq = 0
res.sceneCache = {}
--[[
{
    path = {
        objs = xxx,
        view = xxx
        seq = 0,
        ctrl = xxx,
    },
    ...
}
--]]

local function GetSceneSeq()
    res.sceneSeq = res.sceneSeq + 1
    return res.sceneSeq
end

local sceneCacheMax = 5

function res.SetSceneCacheMax(cnt)
    if type(cnt) ~= "number" or cnt < 1 then
        cnt = 1
    end
    sceneStackMax = cnt
end

function res.GetSceneCacheMax()
    return sceneCacheMax
end

function res.DestroyGameObjectList(objs)
    local lst = clr.table(objs)
    for i, v in ipairs(lst) do
        Object.Destroy(v)
    end
end

local function TrimSceneCache(isNoCollectGarbage)
    if table.nums(res.sceneCache) <= sceneCacheMax then return end
    local sceneTable = {}
    for k, v in pairs(res.sceneCache) do
        local sceneInfo = v
        sceneInfo.path = k
        table.insert(sceneTable, sceneInfo)
    end
    table.sort(sceneTable, function(a, b) return a.seq > b.seq end)

    res.sceneCache = {}
    for i, v in ipairs(sceneTable) do
        if i <= sceneCacheMax then
            local path = v.path
            res.sceneCache[path] = v
        elseif not res.IsClrNull(v.objs) then
            res.DestroyGameObjectList(v.objs)
            v.objs = nil
            v.view = nil
        end
    end
    -- if isNoCollectGarbage ~= true and #sceneTable > sceneCacheMax then
    --     res.CollectGarbage()
    -- end
end

function res.ClearSceneCache()
    for k, v in pairs(res.sceneCache) do
        res.DestroyGameObjectList(v.objs)
        v.objs = nil
    end
    res.sceneCache = {}
    res.sceneSeq = 0
end

local function SaveCurrentSceneInfo()
    local dialogObjs = {}
    if type(res.curSceneInfo) == "table" and type(res.curSceneInfo.dialogs) == "table" then
        for i, v in ipairs(res.curSceneInfo.dialogs) do
            table.insert(dialogObjs, v.view.dialog.gameObject)
            res.RestoreDialogOrder(v.view.dialog.currentOrder)
        end
    end
    local dialogObjsArr = clr.array(dialogObjs, GameObject)
    local pack = UIResManager.PackSceneAndDialogs(dialogObjsArr)
    local sgos = pack.SceneObjs
    -- local dialogObjs = pack.DialogObjs
    -- local dgos = clr.table(dialogObjs)
    -- local dgosDisable = {}
    -- local sgoDisable = true
    -- local isTrimSceneCache = false
    local sceneCacheItem
    if type(res.curSceneInfo) == "table" and not res.IsClrNull(res.curSceneInfo.view) then
        sceneCacheItem = res.sceneCache[res.curSceneInfo.path]
        if not sceneCacheItem then
            sceneCacheItem = {
                objs = sgos,
                view = res.curSceneInfo.view,
                seq = GetSceneSeq(),
                ctrl = res.curSceneInfo.ctrl,
                pack = pack,
            }
            res.sceneCache[res.curSceneInfo.path] = sceneCacheItem
            -- isTrimSceneCache = true
            TrimSceneCache()
        else
            sceneCacheItem.seq = GetSceneSeq()
            -- if res.IsClrNull(sceneCacheItem.obj) then
            --     sceneCacheItem.obj = sgos
            --     sceneCacheItem.ctrl = res.curSceneInfo.ctrl
            -- end
        end
        if res.curSceneInfo.ctrl and type(res.curSceneInfo.ctrl.OnExitScene) == "function" then
            res.curSceneInfo.ctrl:OnExitScene()
        end
    else
        sceneCacheItem = {
            objs = sgos,
            pack = pack,
        }
    end

    -- for i, dgo in ipairs(dgos) do
    --     if type(res.curSceneInfo) == "table" and type(res.curSceneInfo.dialogs) == "table" then
    --         local curDialogInfo = res.curSceneInfo.dialogs[i]
    --         if type(curDialogInfo) == "table" and curDialogInfo.view ~= clr.null then
    --             table.insert(dgosDisable, true)
    --             local dgosItem = res.sceneCache[curDialogInfo.path]
    --             if not dgosItem then
    --                 res.sceneCache[curDialogInfo.path] = {
    --                     obj = dgo,
    --                     view = curDialogInfo.view,
    --                     seq = GetSceneSeq(),
    --                     ctrl = curDialogInfo.ctrl,
    --                 }
    --                 isTrimSceneCache = true
    --             else
    --                 dgosItem.seq = GetSceneSeq()
    --                 if res.CacheObjIsClrNull(dgosItem.obj) then
    --                     dgosItem.obj = dgo
    --                     dgosItem.ctrl = curDialogInfo.ctrl
    --                 end
    --             end
    --             if curDialogInfo.ctrl and type(curDialogInfo.ctrl.OnExitScene) == "function" then
    --                 curDialogInfo.ctrl:OnExitScene()
    --             end
    --         else
    --             table.insert(dgosDisable, false)
    --         end
    --     else
    --         table.insert(dgosDisable, false)
    --     end
    -- end

    -- if isTrimSceneCache == true then
    --     TrimSceneCache()
    -- end

    -- local isCacheToDontDestroyRoot = false
    -- local function CacheToDontDestroyRootFun()
    --     isCacheToDontDestroyRoot = true
    --     ResManager.CacheToDontDestroyRoot(sgos)
    --     ResManager.CacheToDontDestroyRoot(dialogObjs)
    -- end

    -- local function DisableOrDestroyCurrentSceneObj(isLoadScene)
    --     if not res.CacheObjIsClrNull(sgos) then
    --         if sgoDisable  then
    --             ResManager.SetCacheActive(sgos,true,false)
    --         else
    --             res.DestroyGameObjectList(sgos)
    --         end
    --     end
    --     if type(dgos) == "table" then
    --         ResManager.SetCacheActive(dialogObjs,true,false)
    --         for i, dgo in ipairs(dgos) do
    --             if not res.IsClrNull(dgo) and not dgosDisable[i] then
    --                GameUtil.Destroy(dgo)
    --             end
    --         end
    --     end
    --      if isLoadScene == true then
    --         res.ClearSceneCache()
    --     end
    -- end
    -- return DisableOrDestroyCurrentSceneObj, CacheToDontDestroyRootFun

    return sceneCacheItem
end

local function CloseDialog()
    if type(res.curSceneInfo) == "table" and type(res.curSceneInfo.dialogs) == "table" and #res.curSceneInfo.dialogs > 0 then
        local maxIndex = 0
        local maxOrder = -1
        for i, v in ipairs(res.curSceneInfo.dialogs) do
            local order = v.order
            if maxOrder < order then
                maxOrder = order
                maxIndex = i
            end
        end
        if maxIndex > 0 then
            local dialog = res.curSceneInfo.dialogs[maxIndex]
            if type(dialog) == "table" and dialog.view and not res.IsClrNull(dialog.view) and type(dialog.view.closeDialog) == "function" then
                dialog.view:closeDialog()
                return true
            end
        end
    end
end

local dontDestroyRootForSavedScene
local function MoveToDontDestroy(sceneCacheItem)
    if sceneCacheItem and sceneCacheItem.pack then
        if res.IsClrNull(dontDestroyRootForSavedScene) then
            dontDestroyRootForSavedScene = GameObject("DontDestroyRootForSavedScene")
            Object.DontDestroyOnLoad(dontDestroyRootForSavedScene)
            --dontDestroyRootForSavedScene:SetActive(false)
        end
        local sgos = clr.table(sceneCacheItem.pack.SceneObjs)
        for i, v in ipairs(sgos) do
            v.transform:SetParent(dontDestroyRootForSavedScene.transform, false)
        end
    end
end

local function DisableCachedScene(sceneCacheItem)
    if sceneCacheItem and sceneCacheItem.pack then
        if sceneCacheItem.view then
            MoveToDontDestroy(sceneCacheItem)
            local sgos = clr.table(sceneCacheItem.pack.SceneObjs)
            for i, v in ipairs(sgos) do
                v:SetActive(false)
            end
        else
            local sgos = clr.table(sceneCacheItem.pack.SceneObjs)
            for i, v in ipairs(sgos) do
                if not res.IsClrNull(v) then
                    Object.Destroy(v)
                end
            end
        end
    end
end

local function EnableCachedScene(sceneCacheItem)
    if sceneCacheItem and sceneCacheItem.view and sceneCacheItem.pack then
        local sgos = clr.table(sceneCacheItem.pack.SceneObjs)
        for i, v in ipairs(sgos) do
            v.transform:SetParent(nil, false)
            v:SetActive(sceneCacheItem.pack.InitialSceneObjsActive[i])
        end
    end
end

local function ClearCurrentSceneInfo()
    res.curSceneInfo = nil
end
--#endregion View Cache Stack

--#region Controller Cache Stack
res.ctrlStack = {}
--[[
{
    {
        path = xxx,
        args = xxx,
        argc = xxx,
        blur = true,
        dialogs = {
            {
                path = nil,
                order = xxx,
                args = xxx,
                argc = xxx,
            },
            {
                path = nil,
                order = xxx,
                args = xxx,
                argc = xxx,
            },
            {
                path = nil,
                order = xxx,
                args = xxx,
                argc = xxx,
            },
        },
    },
}
--]]

local ctrlStackMax = math.max_int32 

function res.SetCtrlStackMax(cnt)
    if type(cnt) ~= "number" or cnt < 1 then
        cnt = 1
    end
    ctrlStackMax = cnt
end

function res.GetCtrlStackMax()
    return ctrlStackMax
end

local function TrimCtrlStack()
    if #res.ctrlStack > ctrlStackMax then
        for i = ctrlStackMax + 1, #res.ctrlStack do
            res.ctrlStack[i] = nil
        end
    end
end

function res.ClearCtrlStack()
    res.ctrlStack = {}
end

function res.GetLastCtrlPath()
    if #res.ctrlStack > 0 then
        return res.ctrlStack[#res.ctrlStack].path
    end
end

-- function res.GetTopCtrl()
--     if type(res.curSceneInfo) == "table" then
--         if type(res.curSceneInfo.dialogs) == "table" then
--             if #res.curSceneInfo.dialogs > 0 then
--                 return res.curSceneInfo.dialogs[#res.curSceneInfo.dialogs].ctrl
--             end
--         end
--         return res.curSceneInfo.ctrl
--     end
-- end

-- function res.RemoveLastCtrlData()
--     if #res.ctrlStack > 0 then
--         res.ctrlStack[#res.ctrlStack] = nil
--     end
-- end

local function SaveCurrentStatusData()
    -- 如果之前的场景是有ctrl的prefab，则保存其信息
    if type(res.curSceneInfo) == "table" and res.curSceneInfo.ctrl then
        -- 保存ctrl恢复的数据信息
        local args = {res.curSceneInfo.ctrl:GetStatusData()}
        local argc = select('#', res.curSceneInfo.ctrl:GetStatusData())

        local ctrlInfo = {
            path = res.curSceneInfo.path,
            args = args,
            argc = argc,
            blur = res.curSceneInfo.blur,
        }

        table.insert(res.ctrlStack, ctrlInfo)
        TrimCtrlStack()
    end

    if type(res.curSceneInfo) == "table" and type(res.curSceneInfo.dialogs) == "table" and #res.curSceneInfo.dialogs > 0 then
        res.ctrlStack[#res.ctrlStack].dialogs = {}
        for i, dialogInfo in ipairs(res.curSceneInfo.dialogs) do
            local args = {dialogInfo.ctrl:GetStatusData()}
            local argc = select('#', dialogInfo.ctrl:GetStatusData())

            local ctrlInfo = {
                path = dialogInfo.path,
                args = args,
                argc = argc,
                order = dialogInfo.order,
            }

            table.insert(res.ctrlStack[#res.ctrlStack].dialogs, ctrlInfo)
        end
    end
end
--#endregion Controller Cache Stack

--#region Load Prefab/Scene as Scene
local function LoadPrefabScene(loadType, ctrlPath, extra, ...)
    --require("ui.control.button.LuaButton").frameCount = clr.UnityEngine.Time.frameCount
    -- 记录当前场景信息res.curSceneInfo
    res.curSceneInfo = {
        path = ctrlPath,
    }
    local cachedSceneInfo = res.sceneCache[ctrlPath]
    res.sceneCache[ctrlPath] = nil
    local ctrlClass = require(ctrlPath)

    local args = {...}
    local argc = select('#', ...)

    -- local function CreateDialogs()
    --     if type(dialogData) == "table" then
    --         table.sort(dialogData, function(a, b) return tonumber(a.order) < tonumber(b.order) end)
    --         for i, v in ipairs(dialogData) do
    --             LoadPrefabDialog(loadType, v.path, v.order, unpack(v.args, 1, v.argc))
    --         end
    --     end
    -- end

    local function CreateScene()
        local viewPath = ctrlClass.viewPath
        clr.coroutine(function()
            if string.sub(viewPath, -6) == '.unity' then
                local loadinfo = ResManager.LoadSceneAsync(viewPath)
                if loadinfo then
                    coroutine.yield(loadinfo)
                    unity.waitForNextEndOfFrame()
                    res.curSceneInfo.view = cache.removeGlobalTempData("MainManager")
                else
                    local mainManager
                    local waitFrames = 0
                    repeat
                        mainManager = cache.removeGlobalTempData("MainManager")
                        unity.waitForNextEndOfFrame()
                        waitFrames = waitFrames + 1
                    until mainManager or waitFrames > 10
                    res.curSceneInfo.view = mainManager
                end
            else
                local prefab = res.LoadRes(viewPath)
                if prefab then
                    local obj = Object.Instantiate(prefab)
                    local camera = UIResManager.CreateCameraAndEventSystem()
                    res.SetUICamera(obj, camera)
                    res.curSceneInfo.view = res.GetLuaScript(obj)
                end
            end

            res.curSceneInfo.ctrl = ctrlClass.new(res.curSceneInfo.view, unpack(args, 1, argc))
            res.curSceneInfo.ctrl.__loadType = loadType
            res.curSceneInfo.ctrl:Refresh(unpack(args, 1, argc))
            -- if isBlur then
            --     res.curSceneInfo.blur = true
            --     if res.NeedDialogCameraBlur() then
            --         res.SetMainCameraBlur()
            --     end
            -- end
            -- CreateDialogs()

            if res.curSceneInfo.ctrl and type(res.curSceneInfo.ctrl.OnLoadComplete) == "function" then
                res.curSceneInfo.ctrl:OnLoadComplete()
            end
        end)
    end

    if type(cachedSceneInfo) == "table" then
        if not res.IsClrNull(cachedSceneInfo.objs) then
            res.curSceneInfo.ctrl = cachedSceneInfo.ctrl
            res.curSceneInfo.ctrl.__loadType = loadType
            EnableCachedScene(cachedSceneInfo)
            res.curSceneInfo.view = cachedSceneInfo.view

            -- local isDialogData = res.IsClrNull(res.GetLastSCDAndUDs()) and type(dialogData) == "table" and #dialogData > 0
            -- res.curSceneInfo.ctrl.IsCheckCurrUIDialog = isDialogData
            -- res.curSceneInfo.ctrl:Refresh(unpack(args, 1, argc))
            -- res.curSceneInfo.ctrl.IsCheckCurrUIDialog = false
            
            -- if res.NeedDialogCameraBlur() then
            --     if not isDialogData then
            --         res.SetMainCameraBlurOver()
            --     else
            --         res.SetMainCameraBlur()
            --     end
            -- end

            -- CreateDialogs()
            if res.curSceneInfo.ctrl and type(res.curSceneInfo.ctrl.OnLoadComplete) == "function" then
                res.curSceneInfo.ctrl:OnLoadComplete()
            end
        else
            CreateScene()
        end
    else
        CreateScene()
    end

    return res.curSceneInfo.ctrl
end

local function LoadPrefabSceneAsync(loadType, ctrlPath, extra, ...)
    -- require("ui.control.button.LuaButton").frameCount = math.max_int32 - 3
    -- 记录当前场景信息res.curSceneInfo
    res.curSceneInfo = {
        path = ctrlPath,
    }
    local cachedSceneInfo = res.sceneCache[ctrlPath]
    res.sceneCache[ctrlPath] = nil
    local ctrlClass = require(ctrlPath)

    local args = {...}
    local argc = select('#', ...)

    local waitHandle = {}

    -- local function CreateDialogs()
    --     if type(dialogData) == "table" then
    --         table.sort(dialogData, function(a, b) return a.order < b.order end)
    --         for i, v in ipairs(dialogData) do
    --             LoadPrefabDialog(loadType, v.path, v.order, unpack(v.args, 1, v.argc))
    --         end
    --     end
    -- end

    local function CreateScene()
        clr.coroutine(function()
            unity.waitForEndOfFrame()
            local viewPath = ctrlClass.viewPath
            local isLoadScene = string.sub(viewPath, -6) == ".unity" 
            if isLoadScene then
                if extra and extra.cacheItem then
                    MoveToDontDestroy(extra.cacheItem)
                end
                local loadinfo = ResManager.LoadSceneAsync(viewPath)
                if loadinfo then
                    coroutine.yield(loadinfo)
                    unity.waitForNextEndOfFrame()
                    res.curSceneInfo.view = cache.removeGlobalTempData("MainManager")
                    res.curSceneInfo.ctrl = ctrlClass.new(res.curSceneInfo.view, unpack(args, 1, argc))
                    res.curSceneInfo.ctrl.__loadType = loadType
                    res.curSceneInfo.ctrl:Refresh(unpack(args, 1, argc))
                    waitHandle.ctrl = res.curSceneInfo.ctrl
                else
                    local mainManager
                    local waitFrames = 0
                    repeat
                        mainManager = cache.removeGlobalTempData("MainManager")
                        unity.waitForNextEndOfFrame()
                        waitFrames = waitFrames + 1
                    until mainManager or waitFrames > 10

                    res.curSceneInfo.view = mainManager
                    res.curSceneInfo.ctrl = ctrlClass.new(res.curSceneInfo.view, unpack(args, 1, argc))
                    res.curSceneInfo.ctrl.__loadType = loadType
                    res.curSceneInfo.ctrl:Refresh(unpack(args, 1, argc))
                    waitHandle.ctrl = res.curSceneInfo.ctrl
                end
            else
                local loadinfo = ResManager.LoadResAsync(ctrlClass.viewPath)
                if loadinfo then
                    coroutine.yield(loadinfo)
                    unity.waitForNextEndOfFrame()
                    local prefab = loadinfo.asset
                    if prefab then
                        local obj = Object.Instantiate(prefab)
                        local camera = UIResManager.CreateCameraAndEventSystem()
                        res.SetUICamera(obj, camera)
                        res.curSceneInfo.view = res.GetLuaScript(obj)
                        res.curSceneInfo.ctrl = ctrlClass.new(res.curSceneInfo.view, unpack(args, 1, argc))
                        res.curSceneInfo.ctrl.__loadType = loadType
                        res.curSceneInfo.ctrl:Refresh(unpack(args, 1, argc))
                        waitHandle.ctrl = res.curSceneInfo.ctrl
                    end
                end
            end

            -- if isBlur then
            --     res.curSceneInfo.blur = true
            --     if res.NeedDialogCameraBlur() then
            --         res.SetMainCameraBlur()
            --     end
            -- end

            if extra and extra.cacheItem then
                DisableCachedScene(extra.cacheItem)
            end

            -- CreateDialogs()

            waitHandle.done = true

            if res.curSceneInfo.ctrl and type(res.curSceneInfo.ctrl.OnLoadComplete) == "function" then
                res.curSceneInfo.ctrl:OnLoadComplete()
            end

            -- require("ui.control.button.LuaButton").frameCount = clr.UnityEngine.Time.frameCount
        end)
    end

    if type(cachedSceneInfo) == "table" then
        if not res.IsClrNull(cachedSceneInfo.objs) then
            EnableCachedScene(cachedSceneInfo)
            if extra and extra.cacheItem then
                DisableCachedScene(extra.cacheItem)
            end
            res.curSceneInfo.view = cachedSceneInfo.view
            res.curSceneInfo.ctrl = cachedSceneInfo.ctrl
            res.curSceneInfo.ctrl.__loadType = loadType

            -- local isDialogData = res.IsClrNull(res.GetLastSCDAndUDs()) and type(dialogData) == "table" and #dialogData > 0
            -- res.curSceneInfo.ctrl.IsCheckCurrUIDialog = isDialogData
            -- res.curSceneInfo.ctrl:Refresh(unpack(args, 1, argc))
            -- res.curSceneInfo.ctrl.IsCheckCurrUIDialog = false

            -- if res.NeedDialogCameraBlur() then
            --     if not isDialogData then
            --         res.SetMainCameraBlurOver()
            --     else
            --         res.SetMainCameraBlur()
            --     end
            -- end
            -- CreateDialogs()
            waitHandle.done = true
            waitHandle.ctrl = res.curSceneInfo.ctrl

            if res.curSceneInfo.ctrl and type(res.curSceneInfo.ctrl.OnLoadComplete) == "function" then
                res.curSceneInfo.ctrl:OnLoadComplete()
            end
        else
            CreateScene()
        end
    else
        CreateScene()
    end

    return waitHandle
end
--#endregion Load Prefab/Scene as Scene

--#region Push/Pop Scenes
function res.LoadViewImmediate(name, ...)
    SaveCurrentStatusData()
    local cacheItem = SaveCurrentSceneInfo()
    ClearCurrentSceneInfo()
    if string.sub(name, -6) == '.unity' then
        -- cacheItem = nil
        ResManager.LoadScene(name)
        -- DisableCachedScene(cacheItem)
    else
        local prefab = res.LoadRes(name)
        if prefab then
            local obj = Object.Instantiate(prefab)
            DisableCachedScene(cacheItem)
            local camera = UIResManager.CreateCameraAndEventSystem()
            res.SetUICamera(obj, camera)
            return res.GetLuaScript(obj)
        end
    end
end

function res.LoadViewAsync(name, ...)
    SaveCurrentStatusData()
    local cacheItem = SaveCurrentSceneInfo()
    ClearCurrentSceneInfo()
    local waitHandle = {}
    clr.coroutine(function()
        unity.waitForEndOfFrame()
        local isLoadScene = string.sub(name, -6) == ".unity" 
        if isLoadScene then
            cacheItem = nil
            -- MoveToDontDestroy(cacheItem)
            local loadinfo = ResManager.LoadSceneAsync(name)
            if loadinfo then
                coroutine.yield(loadinfo)
                unity.waitForNextEndOfFrame()
            end
        else
            local prefab
            local loadinfo = ResManager.LoadResAsync(name)
            if loadinfo then
                coroutine.yield(loadinfo)
                unity.waitForNextEndOfFrame()
                prefab = loadinfo.asset
            end
            if prefab then
                local obj = Object.Instantiate(prefab)
                local camera = UIResManager.CreateCameraAndEventSystem()
                res.SetUICamera(obj, camera)
            end
        end
        DisableCachedScene(cacheItem)
        waitHandle.done = true
    end)
    return waitHandle
end

function res.LoadView(name, ...)
    local args = {...}
    local argc = select('#', ...)
    clr.coroutine(function()
        unity.waitForNextEndOfFrame()
        SaveCurrentStatusData()
        local cacheItem = SaveCurrentSceneInfo()
        ClearCurrentSceneInfo()
        if string.sub(name, -6) == ".unity" then
            -- cacheItem = nil
            ResManager.LoadScene(name)
            unity.waitForNextEndOfFrame()
            -- DisableCachedScene(cacheItem)
        else
            local prefab = res.LoadRes(name)
            if prefab then
                local obj = Object.Instantiate(prefab)
                local camera = UIResManager.CreateCameraAndEventSystem()
                unity.waitForNextEndOfFrame()
                DisableCachedScene(cacheItem)
                res.SetUICamera(obj, camera)
                return res.GetLuaScript(obj)
            end
        end
    end)
end

function res.PushSceneImmediate(ctrlPath, ...)
    SaveCurrentStatusData()
    local cacheItem = SaveCurrentSceneInfo()
    DisableCachedScene(cacheItem)
    ClearCurrentSceneInfo()

    return LoadPrefabScene(res.LoadType.Push, ctrlPath, nil, ...)
end

function res.PushSceneAsync(ctrlPath, ...)
    SaveCurrentStatusData()
    local cacheItem = SaveCurrentSceneInfo()
    ClearCurrentSceneInfo()

    return LoadPrefabSceneAsync(res.LoadType.Push, ctrlPath, { cacheItem = cacheItem }, ...)
end

function res.PushScene(ctrlPath, ...)
    local args = {...}
    local argc = select('#', ...)
    clr.coroutine(function()
        unity.waitForEndOfFrame()
        res.PushSceneImmediate(ctrlPath, unpack(args, 1, argc))
    end)
end

-- 如果当前最上层的是一个窗口，则只关闭这个窗口，否则关闭整个场景
function res.PopSceneImmediate(...)
    --if not CloseDialog() then
        return res.PopSceneWithCurrentSceneImmediate(...)
    --end
end

function res.PopSceneAsync(...)
    --if not CloseDialog() then
        return res.PopSceneWithCurrentSceneAsync(...)
    --end
end

function res.PopScene(...)
    if not CloseDialog() then
        print("res.PopScene step 2")
        return res.PopSceneWithCurrentScene(...)
    end
end

function res.PopSceneWithCurrentSceneImmediate(...)
    if #res.ctrlStack == 0 then return end

    local cacheItem = SaveCurrentSceneInfo()
    DisableCachedScene(cacheItem)

    ClearCurrentSceneInfo()


    -- restore old info
    local ctrlInfo = table.remove(res.ctrlStack)
    local ctrlPath = ctrlInfo.path
    local argc = select('#', ...)
    local args = {...}
    if argc == 0 then
        args = ctrlInfo.args
        argc = ctrlInfo.argc
    end
    --local isBlur = ctrlInfo.blur

    return LoadPrefabScene(res.LoadType.Pop, ctrlPath, nil, unpack(args, 1, argc))
end

function res.PopSceneWithCurrentSceneAsync(...)
    if #res.ctrlStack == 0 then return end

    local cacheItem = SaveCurrentSceneInfo()

    ClearCurrentSceneInfo()

    -- restore old info
    local ctrlInfo = table.remove(res.ctrlStack)
    local ctrlPath = ctrlInfo.path
    local argc = select('#', ...)
    local args = {...}
    if argc == 0 then
        args = ctrlInfo.args
        argc = ctrlInfo.argc
    end
    --local isBlur = ctrlInfo.blur

    return LoadPrefabSceneAsync(res.LoadType.Pop, ctrlPath, { cacheItem = cacheItem }, unpack(args, 1, argc))
end

function res.PopSceneWithCurrentScene(...)
    local args = {...}
    local argc = select('#', ...)
    clr.coroutine(function()
        unity.waitForEndOfFrame()
        res.PopSceneWithCurrentSceneImmediate(unpack(args, 1, argc))
    end)
end

function res.PopSceneWithoutCurrentImmediate(...)
    if #res.ctrlStack == 0 then return end
    local sgos = UIResManager.PackSceneObj()
    res.DestroyGameObjectList(sgos)
    -- restore old info
    local ctrlInfo = table.remove(res.ctrlStack)
    local ctrlPath = ctrlInfo.path
    local argc = select('#', ...)
    local args = {...}
    if argc == 0 then
        args = ctrlInfo.args
        argc = ctrlInfo.argc
    end
    --local isBlur = ctrlInfo.blur

    return LoadPrefabScene(res.LoadType.Pop, ctrlPath, nil, unpack(args, 1, argc))
end

function res.PopSceneWithoutCurrentAsync(...)
    if #res.ctrlStack == 0 then return end

    local pack = UIResManager.PackSceneAndDialogs()
    local cacheItem = { pack = pack, objs = pack.SceneObjs }
    -- restore old info
    local ctrlInfo = table.remove(res.ctrlStack)
    local ctrlPath = ctrlInfo.path
    local argc = select('#', ...)
    local args = {...}
    if argc == 0 then
        args = ctrlInfo.args
        argc = ctrlInfo.argc
    end
    -- local isBlur = ctrlInfo.blur

    return LoadPrefabSceneAsync(res.LoadType.Pop, ctrlPath, { cacheItem = cacheItem }, unpack(args, 1, argc))
end

function res.PopSceneWithoutCurrent(...)
    local args = {...}
    local argc = select('#', ...)
    clr.coroutine(function()
        unity.waitForEndOfFrame()
        res.PopSceneWithoutCurrentImmediate(unpack(args, 1, argc))
    end)
end

function res.ChangeSceneImmediate(ctrlPath, ...)
    SaveCurrentStatusData()
    local cacheItem = SaveCurrentSceneInfo()
    DisableCachedScene(cacheItem)
    res.ClearSceneCache()
    ClearCurrentSceneInfo()
    return LoadPrefabScene(res.LoadType.Change, ctrlPath, nil, ...)
end

function res.ChangeSceneAsync(ctrlPath, ...)
    SaveCurrentStatusData()
    local cacheItem = SaveCurrentSceneInfo()
    res.ClearSceneCache()
    ClearCurrentSceneInfo()

    return LoadPrefabSceneAsync(res.LoadType.Change, ctrlPath, { cacheItem = cacheItem }, ...)
end

function res.ChangeScene(ctrlPath, ...)
    local args = {...}
    local argc = select('#', ...)
    clr.coroutine(function()
        unity.waitForEndOfFrame()
        res.ChangeSceneImmediate(ctrlPath, unpack(args, 1, argc))
    end)
end
--#endregion Push/Pop Scenes




local function LoadPrefabDialog(loadType, ctrlPath, order, ...)
    cache.setGlobalTempData(true, "LoadingPrefabDialog")

    -- 记录当前窗口信息
    local dialogInfo = {}
    if type(res.curSceneInfo.dialogs) ~= "table" then
        res.curSceneInfo.dialogs = {}
    end
    table.insert(res.curSceneInfo.dialogs, dialogInfo)
    dialogInfo.path = ctrlPath

    local cachedSceneInfo = res.sceneCache[ctrlPath]
    res.sceneCache[ctrlPath] = nil
    local ctrlClass = require(ctrlPath)

    local args = {...}
    local argc = select('#', ...)

    local function CreateDialog()
        local viewPath = ctrlClass.viewPath
        local dialog, dialogcomp = res.ShowDialog(viewPath, "camera", ctrlClass.dialogStatus.touchClose, ctrlClass.dialogStatus.withShadow, ctrlClass.dialogStatus.unblockRaycast, true)
        dialogInfo.view = dialogcomp.contentcomp
        dialogInfo.order = dialog:GetComponent(Canvas).sortingOrder
        dialogInfo.ctrl = ctrlClass.new(dialogInfo.view, unpack(args, 1, argc))
        dialogInfo.ctrl.__loadType = loadType
        dialogInfo.ctrl:Refresh(unpack(args, 1, argc))
        res.GetLuaScript(dialog).OnExitScene = function ()
            if type(dialogInfo.ctrl.OnExitScene) == "function" then
                dialogInfo.ctrl:OnExitScene()
            end
        end
    end

    if type(cachedSceneInfo) == "table" then
        if not res.CacheObjIsClrNull(cachedSceneInfo.obj) then
            dialogInfo.view = cachedSceneInfo.view
            dialogInfo.ctrl = cachedSceneInfo.ctrl
            dialogInfo.order = order
            dialogInfo.view.dialog:setOrder(order)
            dialogInfo.ctrl.__loadType = loadType
            dialogInfo.ctrl:Refresh(unpack(args, 1, argc))

            local scd, uds = res.GetLastSCDAndUDs(false)

            ResManager.UnpackSceneObj(cachedSceneInfo.obj,true)
            
            res.AdjustDialogCamera(scd, uds, dialogInfo.view.gameObject, dialogInfo.view.dialog.withShadow)
        else
            res.sceneCache[ctrlPath] = nil
            CreateDialog()
        end
    else
        CreateDialog()
    end

    cache.removeGlobalTempData("LoadingPrefabDialog")
    return dialogInfo.ctrl
end

function res.CacheHandle()
    SaveCurrentStatusData()
    local disableOrDestroySceneFunc = SaveCurrentSceneInfo()
    ClearCurrentSceneInfo()
end

function res.PushDialogImmediate(ctrlPath, ...)
    return LoadPrefabDialog(res.LoadType.Push, ctrlPath, nil, ...)
end

function res.PushDialog(ctrlPath, ...)
    local args = {...}
    local argc = select('#', ...)
    clr.coroutine(function()
        unity.waitForEndOfFrame()
        res.PushDialogImmediate(ctrlPath, unpack(args, 1, argc))
    end)
end

-- function res.RemoveCurrentSceneDialogsInfo()
--     if type(res.curSceneInfo) == "table" then
--         res.curSceneInfo.dialogs = nil
--     end
-- end


function res.ChangeGameObjectLayer(dialog, layer)
    UIResManager.ChangeGameObjectLayer(dialog, layer)
end

function res.ShowDialog(content, renderMode, touchClose, withShadow, unblockRaycast, withCtrl, overlaySortingOrder)
    local loadingType = cache.getGlobalTempData("LoadingPrefabDialog")
    local loadingInfo = { dialog = {} }
    if not loadingType then
        if unmanagedBlockDialogs[content] then
            loadingInfo.block = true
        end
        for i = #unmanagedDialogs, 1, -1 do
            local dialog = unmanagedDialogs[i].dialog
            if not dialog or res.IsClrNull(dialog) then
                table.remove(unmanagedDialogs, i)
            end
        end

        unmanagedDialogs[#unmanagedDialogs + 1] = loadingInfo
    end

    local dialog, dialogSpt, dummydialog, blockdialog, diagcomp, dummycomp, scd, uds

    if renderMode and renderMode ~= "overlay" then
        scd, uds = res.GetLastSCDAndUDs(false)
        dialog, dialogSpt = res.Instantiate("Game/UI/Common/Dialog/CameraDialog.prefab")

        cache.setGlobalTempData(true, "isDummyDialog")
        dummydialog = res.Instantiate("Game/UI/Common/Dialog/OverlayDialog.prefab")
        cache.removeGlobalTempData("isDummyDialog")
        local dummycanvas = dummydialog:GetComponent(Canvas)
        res.GetLuaScript(dummycanvas):setShadow(withShadow)
        diagcomp = res.GetLuaScript(dialog)

        dummycomp = res.GetLuaScript(dummydialog)
        diagcomp.withCtrl = withCtrl
        if withShadow then
            diagcomp:setShadow(true)
            if res.NeedDialogCameraBlur() then
                -- res.SetMainCameraBlur()
                diagcomp:enableShadow()
            else
                res.GetLuaScript(dummycanvas):enableShadow()
                diagcomp:enableShadow()
            end
        else
            diagcomp:setShadow(false)
        end
    else
        if overlaySortingOrder then
            cache.setGlobalTempData(overlaySortingOrder, "overlaySortingOrder")
        end
        dialog = res.Instantiate("Game/UI/Common/Dialog/OverlayDialog.prefab")
        cache.removeGlobalTempData("overlaySortingOrder")
        diagcomp = res.GetLuaScript(dialog)
        diagcomp.withCtrl = withCtrl
        if withShadow then
            diagcomp:setShadow(true)
            diagcomp:enableShadow()
        else
            diagcomp:setShadow(false)
        end
    end

    local objcontent
    if type(content) == "string" then
        -- TODO 这部分在全冠也没有用到，之后需要再查一下
        -- objcontent = res.InstanceCache[content]

        -- if objcontent and not res.IsClrNull(objcontent) then
        --     objcontent = Object.Instantiate(objcontent)
        -- else
        --     objcontent = res.Instantiate(content)
        -- end

        objcontent = res.Instantiate(content)
        objcontent.transform:SetParent(dummydialog and dummydialog.transform or dialog and dialog.transform, false)
        if objcontent then
            diagcomp.content = objcontent
            local compcontent = res.GetLuaScript(objcontent)
            if compcontent then
                diagcomp.contentcomp = compcontent
                compcontent.dialog = diagcomp
                compcontent.closeDialog = diagcomp.closeDialog
            end
        end
    end

    if dummydialog then
        clr.coroutine(function()
            coroutine.yield(UnityEngine.WaitForEndOfFrame())
            if dialog ~= nil and not res.IsClrNull(dialog) then
                if objcontent then
                    objcontent.transform:SetParent(dialogSpt.safeArea or dialog.transform, false)
                    res.AdjustDialogCamera(scd, uds, dialog, withShadow)
                end
            else
                clr.GameUtil.Destroy(objcontent)
            end
            clr.GameUtil.Destroy(dummydialog)
        end)
    end

    if touchClose then
        -- 点击之后关闭当前dialog
        if type(diagcomp.contentcomp.Close) == "function" then
            diagcomp:regOnButtonClick(function ()
                diagcomp.contentcomp:Close()
            end)
        else
            diagcomp:regOnButtonClick(diagcomp.closeDialog)
        end
    end
    if unblockRaycast then
        diagcomp.___ex.canvasGroup.blocksRaycasts = false
    end

    if not loadingType then
        for i = #unmanagedDialogs, 1, -1 do
            local dialog = unmanagedDialogs[i].dialog
            if not dialog or res.IsClrNull(dialog) then
                table.remove(unmanagedDialogs, i)
            end
        end

        loadingInfo.dialog = diagcomp
    end

    return dialog, diagcomp
end

local UILayers = 5
function res.ChangeCameraDialogToUI(dialog)
    res.SetUICamera(dialog, UIResManager.FindUICamera())
    res.ChangeGameObjectLayer(dialog, UILayers)
end

local DialogLayers = 23
function res.ChangeCameraDialogToDialog(dialog)
    res.SetUICamera(dialog, res.GetDialogCamera())
    res.ChangeGameObjectLayer(dialog, DialogLayers)
end

function res.AdjustDialogCamera(scd, uds, dialog, withShadow)
    if withShadow then
        res.ChangeCameraDialogToDialog(dialog)
        if scd and not res.IsClrNull(scd) then
            res.ChangeCameraDialogToUI(scd)
        end
        for i, v in ipairs(uds) do
            if v and not res.IsClrNull(v) then
                res.ChangeCameraDialogToUI(v)
            end
        end
    else
        if scd and not res.IsClrNull(scd) then
            res.ChangeCameraDialogToDialog(dialog)
        else
            res.ChangeCameraDialogToUI(dialog)
        end
    end
end

-- 获取最上层的带有shadow的camera dialog及其上面的所有不带shadow的camera dialog，
-- 并且不带shadow的camera dialog是按照order从小到大排好序的
-- withoutCurrent代表是否不包括当前最顶层CameraDialog
-- 这个方法应该只在顶层是带有shadow的camera dialog是调用才有意义
function res.GetLastSCDAndUDs(withoutCurrent)
    local DialogCacheObjLayer = ResManager.DialogCacheObjLayer
    local SceneCacheObjLayer = ResManager.SceneCacheObjLayer
    local canvases = clr.table(Object.FindObjectsOfType(Canvas))
    local cameraCanvas = {}
    local layer, parent
    for i, v in ipairs(canvases) do
        if v ~= nil and (not res.IsClrNull(v.gameObject)) then
            layer = v.gameObject.layer
            if layer == DialogCacheObjLayer or layer == SceneCacheObjLayer then
            else
                parent = v.transform.parent
                if (parent == nil or res.IsClrNull(parent)) and v.renderMode == RenderMode.ScreenSpaceCamera and v.sortingLayerName == "Dialog" then
                    table.insert(cameraCanvas, v)
                end
            end
        end
    end

    table.sort(cameraCanvas, function(a, b) return a.sortingOrder > b.sortingOrder end)

    local scd = nil
    local uds = {}
    local startIndex = withoutCurrent and 2 or 1
    for i = startIndex, #cameraCanvas do
        local v = cameraCanvas[i]
        if res.GetLuaScript(v).withShadow then
            scd = v.gameObject
            break
        else
            table.insert(uds, 1, v.gameObject)
        end
    end

    return scd, uds
end

function res.ChangeCameraDialogToDialog(dialog)
    res.SetUICamera(dialog, res.GetDialogCamera())
    -- set object to Dialog Layer
    res.ChangeGameObjectLayer(dialog, 23)
end

-- 是否需要开启弹出窗口背景模糊效果
function res.NeedDialogCameraBlur()
    return true -- device.level ~= "low"
end

-- 设置由MainCamera渲染的UI界面模糊
-- function res.SetMainCameraBlur()
--     local loadingType = cache.getGlobalTempData("LoadingPrefabDialog")
--     if loadingType then
--         if type(res.curSceneInfo) == "table" then
--             res.curSceneInfo.blur = true
--         end
--     end
--     return res.SetCameraBlur(res.GetMainCamera())
-- end

-- function res.SetCameraBlur(camera)
--     local rapidBlurEffect = camera.gameObject:GetComponent(clr.RapidBlurEffect)
--     if not rapidBlurEffect then
--         rapidBlurEffect = camera.gameObject:AddComponent(clr.RapidBlurEffect)
--         rapidBlurEffect.DownSampleNum = 3
--         rapidBlurEffect.BlurSpreadSize = 6
--         rapidBlurEffect.BlurIterations = 2
--         local blurColor = {0, 0, 0, 0}
--         rapidBlurEffect.color = UnityEngine.Color(blurColor[1], blurColor[2], blurColor[3], blurColor[4])
--     end
--     rapidBlurEffect.enabled = true
-- end

-- 关闭模糊特效
-- function res.SetMainCameraBlurOver()
--     if type(res.curSceneInfo) == "table" then
--         res.curSceneInfo.blur = nil
--     end
--     return res.SetCameraBlurOver(res.GetMainCamera())
-- end

-- function res.SetCameraBlurOver(camera)
--     assert(camera and not res.IsClrNull(camera))
--     local rapidBlurEffect = camera.gameObject:GetComponent(clr.RapidBlurEffect)
--     if not rapidBlurEffect then
--         return
--     end

--     rapidBlurEffect.enabled = false
-- end

return res
