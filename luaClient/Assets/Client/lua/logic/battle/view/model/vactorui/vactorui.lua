require "logic/common/basemodel"

local class = require("common/middleclass")

vactorui = class("vactorui",basemodel)


function vactorui:initialize(v_actor)
  self.v_actor = v_actor
end


function vactorui:LateUpdate()
  local camera = self.v_actor.battle_view.scene_relation_mono.mainCamera
  self.ui_parent.transform.rotation = camera.transform.rotation
  
end


function vactorui:loadResource()
    self.ui_parent = UGameObject.New("ui")
    self.ui_parent.transform:SetParent(self.v_actor.unity_v_actor_game_object.transform)
    unitytools.resetPos(self.ui_parent)
    self.ui_parent.transform.localScale = Vector3.New(0.01,0.01,0.01)
    self.ui_parent.transform.localPosition = Vector3.New(0,1.5,0)
    
    local lua_behaviour = self.ui_parent:AddComponent(typeof(LuaBehaviour))
    lua_behaviour:Init(self)
    
    self.hp_ui = uitools.loadUIObject("ui/component/battle_hero_title",self.ui_parent.transform)
    self.hp_ui.layer = LayerMask.NameToLayer("Default");
    self.hp_ui:SetActive(false)
    self:freshHp()
end

function vactorui:unLoadResource()
  if self.ui_parent ~= nil then
    UGameObject.Destroy(self.ui_parent)
  end
  self.ui_parent = nil
end

function vactorui:hide()
  self.is_show = false
  if self.ui_parent ~= nil then
    self.ui_parent:SetActive(self.is_show )
  end
end

function vactorui:show()
  self.is_show = true
  if self.ui_parent ~= nil then
    self.ui_parent:SetActive(self.is_show )
  end
end

function vactorui:setHpState(is_attack)
  
  if self.hp_fore_sprite == nil then
    self.hp_fore_sprite = self.hp_ui.transform:FindChild("hp/fore/fore_sprite").gameObject
  end
  
  if self.hp_fore_sprite == nil then
      return
  end
  
  if is_attack then
    self.hp_fore_sprite:GetComponent("UnityEngine.UI.Image").sprite = TexturePackerManager.Instance:GetSprite("battle_ui","tiao7")
  else
    self.hp_fore_sprite:GetComponent("UnityEngine.UI.Image").sprite = TexturePackerManager.Instance:GetSprite("battle_ui","tiao6")
  end

end


function vactorui:freshHp()
  if self.hp_slider_obj == nil then
    self.hp_text_obj = self.hp_ui.transform:FindChild("hp_text").gameObject
    self.hp_slider_obj = self.hp_ui.transform:FindChild("hp").gameObject
  end
  
  if self.hp_slider_obj == nil then
      return
  end
  
  self.hp_slider_obj:GetComponent("UnityEngine.UI.Slider").value = self.v_actor.e_actor.e_actor_attribute_manager.hp/self.v_actor.e_actor.e_actor_attribute_manager.max_hp:getValue()
  self.hp_text_obj:GetComponent("UnityEngine.UI.Text").text = self.v_actor.e_actor.e_actor_attribute_manager.hp.."/"..self.v_actor.e_actor.e_actor_attribute_manager.max_hp:getValue()
end
