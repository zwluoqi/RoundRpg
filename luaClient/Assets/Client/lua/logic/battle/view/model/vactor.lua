


require "logic/common/basemodel"
require "logic/battle/view/model/vactorai/vactorai"
require "logic/battle/view/model/vactorui/vactorui"
require "logic/battle/view/model/vactormodel/vactormodel"
require "logic/battle/view/model/vactoreffect/vactoreffect"


local class = require("common/middleclass")

vactor = class("vactor",basemodel)




function vactor:initialize(e_actor)
  self.battle_manager = battlemanager.instance()
  self.battle_view = self.battle_manager.battle_view
  self.e_actor = e_actor

  local dict_hero_data = dict_hero.getDataByID(self.e_actor.input_actor.hero_id)

  self.unity_v_actor_game_object = UGameObject.New("guid:"..e_actor.guid.." hero_name "..dict_hero_data.name.." copy_enemy_id:"..e_actor.input_actor.copy_enemy_id.." order:"..e_actor.input_actor.order.." pos:"..e_actor.input_actor.pos_id)
  self.unity_v_actor_game_object.transform:SetParent(self.battle_manager.battle_view.battle_actor_pref_game_object.transform)
  
  self.unity_v_actor_game_object.transform.localPosition = Vector3.New(self.e_actor.input_actor.position_x,self.e_actor.input_actor.position_y,self.e_actor.input_actor.position_z)
  self.unity_v_actor_game_object.transform.localEulerAngles = Vector3.New(0,self.e_actor.input_actor.euler_y,0)
  
  self.v_actor_model = vactormodel(self)
  self.v_actor_ai = vactorai(self)
  self.v_actor_ui = vactorui(self)
  self.v_actor_effect = vactoreffect(self)

  
end

function vactor:destroy()
  self.v_actor_model:destroy()
  self.v_actor_ai:destroy()
  
  UGameObject.Destroy(self.unity_v_actor_game_object)
  self.unity_v_actor_game_object = nil
  self.e_actor = nil
end


function vactor:loadResource()
  self.v_actor_model:loadResource()
  self.v_actor_ai:loadResource()
  self.v_actor_ui:loadResource()  

  self.e_actor:setVActor(self.unity_v_actor_game_object)

end

function vactor:unLoadResource()
  self.v_actor_model:unLoadResource()
  self.v_actor_ai:unLoadResource()
  self.v_actor_ui:unLoadResource()  
  
end


function vactor:hide()
  self.is_show = false
  self.v_actor_model:hide()
  self.v_actor_ai:hide()
    self.v_actor_ui:hide()  

end

function vactor:show()
  self.is_show = true
  self.v_actor_model:show()
  self.v_actor_ai:show()
  self.v_actor_ui:show()  

end




function vactor:ready()
  self.v_actor_effect:ready()
  self.v_actor_model:playAnim(AnimName.IDLE)
end


function vactor:clear()
  self.v_actor_effect:clear()
  
end


function vactor:destroy()
    self.e_actor = nil
    self.v_actor_model:destroy()
    self.v_actor_ai:destroy()
end

function vactor:tick()

end

function vactor:startSkill(e_skill)
  self.v_actor_model:playAnim(e_skill.model_data.animation_name)
  
  
  
  for k,v in pairs(e_skill.model_data.skill_effect_id) do
    self.v_actor_effect:bindEffect(v)
  end
  
end

function vactor:endSkill(e_skill)
  self.v_actor_model:playAnim(AnimName.IDLE)
    
end

function vactor:interruptSkill(e_skill)


end




return vactor
