

require "logic/common/basemodel"

local class = require("common/middleclass")

vactoreffect = class("vactoreffect",basemodel)


function vactoreffect:initialize(v_actor)
  self.v_actor = v_actor
  self. skill_effect_duration_events = {}
  self. skill_effect_asyncers = {}
  self.skill_effect_game_objects = {}
end

function vactoreffect:ready()

end




function vactoreffect:bindEffect(effect_resource_id)
  local effect_resource_model_data = dict_effect_resource.getDataByID(effect_resource_id)
  if effect_resource_model_data == nil then
    printError("effect_resource_id:"..effect_resource_id)
      return
  end
  
  self:bindEffect0(effect_resource_model_data)
end



function vactoreffect:bindEffect0(effect_resource_model_data)
  
  self.skill_effect_asyncers[effect_resource_model_data.id] = dynamicloadasyncmanager.loadAsync(effect_resource_model_data.url,function(game_object,user_data,load_asyncer)
      local re_effect_resource_model_data = user_data
      if game_object ~= nil then
        
        self:setSkillEffect(game_object,effect_resource_model_data)
      end
      
    end
    ,effect_resource_model_data)
  
end

function vactoreffect:setSkillEffect(game_object,effect_resource_model_data)
  local parentTrans = self.v_actor.v_actor_model:getActorTrans(effect_resource_model_data.boen_type)
  
  game_object.transform:SetParent(parentTrans);
  unitytools.resetPos(game_object)
  
  game_object:SetActive(false)
  game_object:SetActive(true)
  self.skill_effect_game_objects[effect_resource_model_data.id] =  game_object
   
  self.skill_effect_duration_events[effect_resource_model_data.id] = self.v_actor.battle_manager.time_event_manager:createEvent(function()
      GameObjectPoolManager.Instance:Unspawn(game_object)
      self.skill_effect_game_objects[effect_resource_model_data.id] = nil
    end,"skillEffectDuration",effect_resource_model_data.duration)
end



function vactoreffect:clear()
  for k,v in pairs(self. skill_effect_asyncers) do
    dynamicloadasyncmanager.deleteAsyncer(v)
  end
  self. skill_effect_asyncers = {}
  
  for k,v in pairs(self. skill_effect_duration_events) do
    self.v_actor.battle_manager.time_event_manager:delete(v)
  end
  self. skill_effect_duration_events = {}
  
  for k,v in pairs(self.skill_effect_game_objects) do
      GameObjectPoolManager.Instance:Unspawn(v)
  end
  self.skill_effect_game_objects = {}
  
end








return vactoreffect




