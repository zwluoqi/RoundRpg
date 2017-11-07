
require "logic/luaresources/localresourcemanager"
require "logic/luaresources/dynamicloadasyncmanager"
require "logic/common/enumdefine"

stagemanager = class("stagemanager")

local _instatnce = nil
function stagemanager.instance()
  return _instatnce
end

function stagemanager:initStage()
    self.stage_dict[StageType.FUBEN] = (require ("logic/stage/"..StageType.FUBEN))()
    self.stage_dict[StageType.TOWN] = (require ("logic/stage/"..StageType.TOWN))()
end


function stagemanager:initialize()
    _instatnce = self
    self.stage_dict = {}
    self.current_stage = nil
    self.local_resource_manager = localresourcemanager()
    
    self:initStage()
end

function stagemanager:addStage(stage_type,stage_base)
    if self.stage_dict[stage_type] ~= nil then
        printError("had stageType:"..stage_type)
    end
    self.stageDict[stage_type] = stage_base
    
end


function stagemanager:switchStage(stage_type)
    if self.current_stage ~= nil then
        self.current_stage.onEnd()
    end
    self.current_stage = self.stage_dict[stage_type]
    printLog(self.current_stage.name)
    self.current_stage:preLoad()
    self.local_resource_manager:startLoad(
      function()
        printWarning("preLoadCallback")
        self.current_stage:onStart()
      end
)
end






return stagemanager

