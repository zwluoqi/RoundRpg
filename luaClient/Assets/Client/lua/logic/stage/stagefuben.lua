

local class = require("common/middleclass")

require "logic/stage/stagebase"


stagefuben = class("stagefuben",stagebase)

function stagefuben:initialize()
    self.name = "stagefuben"
end

function stagefuben.requestCopy(copy_id)
  stagefuben.request_copy_id = copy_id
end


function stagefuben:preLoad()
    local stage_manager = stagemanager.instance()
    local battle_manager = battlemanager.instance()
    local input_room = battle_manager:createFubenData(stagefuben.request_copy_id)
    
    for k,v in pairs(input_room.input_atk_actors) do
      for ik,iv in pairs(v) do
        local hero_model_data = dict_hero.getDataByID(iv.hero_id)
        stage_manager.local_resource_manager:addRes(LocalResourceType.Prefab,"models/"..hero_model_data.resoure_name)
      end
    end
    
    for k,v in pairs(input_room.input_def_actors) do
      for ik,iv in pairs(v) do
        local hero_model_data = dict_hero.getDataByID(iv.hero_id)
        stage_manager.local_resource_manager:addRes(LocalResourceType.Prefab,"models/"..hero_model_data.resoure_name)
      end
    end
    
    
    stage_manager.local_resource_manager:addRes(LocalResourceType.Scene,battle_manager.battle_engine.scene_data.sence_res_id)
    
    
end

function stagefuben:onStart()
  
    printWarning("stage onStart ok")
    
    local battle_manager = battlemanager.instance()
    battle_manager:ready()
    
    local random_util = RandomUtil.New()
    
    random_util:SetSeed(9527)
    
    
    printWarning("randomUtil test:"..random_util.value)

end


function stagefuben:onEnd()
  local battle_manager = battlemanager.instance()
  battle_manager:clear()
end



return stagefuben