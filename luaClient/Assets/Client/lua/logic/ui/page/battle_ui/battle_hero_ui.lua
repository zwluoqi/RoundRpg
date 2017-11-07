


local class = require("common/middleclass")
require "logic/ui/uipart/lua_mono_behaviour"

battle_hero_ui = class("battle_hero_ui",lua_mono_behaviour)



function battle_hero_ui:initData(e_actor)
  self.e_actor = e_actor
  self.slide_component_value = nil
  self.anger_slider_obj = self.transform:FindChild("anger").gameObject
  self.blood_transform = self.transform:FindChild("blood")
  self.is_big = false
  self.anger_slider_obj:SetActive(true)
  self.blood_transform.gameObject:SetActive(true)
  self.gameObject:GetComponent("UnityEngine.UI.Image").color = Color(1,1,1,1)
  self.transform.localScale = Vector3(1,1,1)
end

function battle_hero_ui:heroSetSprite()
  local image = self.gameObject:GetComponent("UnityEngine.UI.Image")
  local dict_hero_data = dict_hero.getDataByID(self.e_actor.input_actor.hero_id)
  local sprite1 = dict_hero_data.resoure_name
  image.sprite = TexturePackerManager.Instance:GetSprite("hero_icon",sprite1)
end


function battle_hero_ui:changeHeroBloodSlider()
    local blood_slider_obj = self.blood_transform.gameObject
    local slide_component = blood_slider_obj:GetComponent("UnityEngine.UI.Slider")
    slide_component.value = self.e_actor.e_actor_attribute_manager.hp / self.e_actor.e_actor_attribute_manager.max_hp:getValue()
    self.slide_component_value = slide_component.value
   if slide_component.value  <= 0 then
      self.transform.localScale = Vector3(1,1,1)
      local image = self.gameObject:GetComponent("UnityEngine.UI.Image")
      local tmp_color = Color(62/255,58/255,58/255,1); 
      image.color = tmp_color
      self.blood_transform.gameObject:SetActive(false)
      self.anger_slider_obj:SetActive(false)
    else
      local image = self.gameObject:GetComponent("UnityEngine.UI.Image")
      local tmp_color = Color(1,1,1,1)
      image.color = tmp_color
    end
end

function battle_hero_ui:changeHeroAngerSlider()
  local anger_slider_component = self.anger_slider_obj:GetComponent("UnityEngine.UI.Slider")
  anger_slider_component.value = self.e_actor.e_actor_attribute_manager.anger_val / self.e_actor.e_actor_attribute_manager.max_anger_val
    if anger_slider_component.value >= 1 then
      if  self.slide_component_value == nil or  self.slide_component_value > 0  then
        --local tweener = self.transform:DOScale(Vector3(1.2,1.2,1.2),0.24)
        --tweener:SetEase(DG.Tweening.Ease.Linear)
        if self.is_big == false  then
           local dotween_component = self.gameObject:GetComponent(typeof(DG.Tweening.DOTweenAnimation))
            dotween_component:DORestart(true)
            dotween_component:DOPlay()
            self.is_big = true
        end
        
       
        
      -- coroutine.start(function()
       -- coroutine.wait(0.3)
        --dotween_component:DORestart(true)
        --dotween_component:DOKill()
        --end)
        
       -- print("dotween")
      else
        self.anger_slider_obj:SetActive(false)
      end
    else
      self.is_big = false
      if self.transform.localScale ~= Vector3(1,1,1) then
        local tweener = self.transform:DOScale(Vector3(1,1,1),0.24)
      end
   end
      
  
  
end
 

return battle_hero_ui