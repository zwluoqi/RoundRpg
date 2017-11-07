require "logic/common/basemodel"


local class = require("common/middleclass")

vactormodel = class("vactormodel",basemodel)


function vactormodel:initialize(v_actor)
  self.v_actor = v_actor
  local dict_hero_data = dict_hero.getDataByID(self.v_actor.e_actor.input_actor.hero_id)
  local dict_hero_anim_data_s  = dict_hero_animation.getDataByIndexOne(dict_hero_data.resoure_name)
  self.anim_time_data_s = {}
  for k,v in pairs(dict_hero_anim_data_s) do
      self.anim_time_data_s[v.animName] = v.animLength
  end
  
end



function vactormodel:loadResource()
  local dict_hero_data = dict_hero.getDataByID(self.v_actor.e_actor.input_actor.hero_id)
  
  self.load_asyncer = dynamicloadasyncmanager.loadAsync("models/"..dict_hero_data.resoure_name,function(game_object)
      self.unity_hero_model_game_object = game_object
      self.unity_hero_model_game_object.transform:SetParent(self.v_actor.unity_v_actor_game_object.transform)
      unitytools.resetPos(self.unity_hero_model_game_object)
      
      self.i_actor_trans = self.unity_hero_model_game_object:GetComponent(typeof(I_ActorTrans))
      
      self.anim_controller = self.unity_hero_model_game_object:AddComponent(typeof(AnimController))
      self.anim_controller:Init(self.i_actor_trans.actionAnimation,"models/hero/"..dict_hero_data.resoure_name)
      
      if self.is_show then
        self:show()
        self:playAnim(AnimName.IDLE)
      else
        self:hide()
      end
    end
    ,nil)

end

function vactormodel:unLoadResource()
  if self.load_asyncer ~= nil then
    dynamicloadasyncmanager.deleteAsyncer(self.load_asyncer)
    self.load_asyncer = nil
  end
  GameObjectPoolManager.Instance:Unspawn(self.unity_hero_model_game_object)
  self.unity_hero_model_game_object = nil
end

function vactormodel:hide()
  self.is_show = false
  if self.unity_hero_model_game_object == nil then
    return
  end
  self.unity_hero_model_game_object:SetActive(false)
end

function vactormodel:show()
  self.is_show = true
  if self.unity_hero_model_game_object == nil then
    return
  end
  self.unity_hero_model_game_object:SetActive(true)
end


function vactormodel:getCurrentPlayCount()
  if self.anim_controller == nil then
    return 0
  end
  return self.anim_controller:getCurrentPlayCount()
end


function vactormodel:playAnim(anim_name,wrap_mode)
  if self.anim_controller == nil then
    return 0 
  end
  if wrap_mode == nil then
    wrap_mode = UnityEngine.WrapMode.Loop
  end
  
  return self.anim_controller:playAnim(anim_name,wrap_mode,true)
end

function vactormodel:getActorTrans(bone_type)
  if bone_type == HangPointType.HEAD then
      return self.i_actor_trans.headTra
  elseif bone_type == HangPointType.LEFTHAND then
      return self.i_actor_trans.lefthandTra
  elseif bone_type == HangPointType.RIGHTHAND then
      return self.i_actor_trans.righthandTra
  elseif bone_type == HangPointType.HEADNUB then
      return self.i_actor_trans.headnubTra
  else
      return self.v_actor.unity_v_actor_game_object.transform
  end

end





