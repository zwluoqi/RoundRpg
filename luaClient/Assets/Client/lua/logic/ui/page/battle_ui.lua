require "logic/ui/page/page_ui"
require "logic/ui/page/battle_ui/battle_hero_ui"
require "logic/ui/page/battle_ui/battle_over_ui"
require "logic/ui/page/battle_ui/battle_controller_ui"

local class = require("common/middleclass")
battle_ui = class("battle_ui",page_ui)


function battle_ui:initData()

  local battle_over_obj = self.transform:FindChild("battle_over").gameObject
  
  self.battle_over_logic = battle_over_ui(battle_over_obj)
  self.battle_over_logic:initData()
  
  self.battle_controller_ui = battle_controller_ui(self.gameObject)
  self.battle_controller_ui:initData()
  
  
  --self.raw_image = self.transform:FindChild("RawImage").gameObject
  self.weather_obj = self.transform:FindChild("weather").gameObject
  self.weather_trans = self.transform:FindChild("ImageTrans").gameObject
  self.weather_icon_prefab = nil
  self.source_weather_pos = self.weather_obj.transform.localPosition
  self:startWeatherIcon(battlemanager.instance().battle_engine.e_weather.current_weather_data.weather_id)
end 



function battle_ui:registerEventListener()
  
  self.battle_view2logic_change_blood_value_fun = 
  function(data)
    self:changeBloodValue(data)
  end
  battlemanager.instance().event_manager:addEventListener(EventManagerDefine.battle_logic2view_actor_skill_export,self.battle_view2logic_change_blood_value_fun)
  
  self.battle_view2logic_exchange_weather_fun = 
  function(data)
    self:exchangeWeather(data)
  end
  battlemanager.instance().event_manager:addEventListener(EventManagerDefine.battle_logic2view_exchange_weather,self.battle_view2logic_exchange_weather_fun)
end

function battle_ui:unRegisterEventListener()

  battlemanager.instance().event_manager:removeEventListener(EventManagerDefine.battle_logic2view_actor_skill_export,self.battle_view2logic_change_blood_value_fun)
   battlemanager.instance().event_manager:removeEventListener(EventManagerDefine.battle_logic2view_exchange_weather,self.battle_view2logic_exchange_weather_fun)
end



function battle_ui:doOpen()
 
  self:registerEventListener()
  
  self.battle_hero_list = {}

  
  for k,v in pairs(battlemanager.instance().battle_engine.atk_actors_list[1]) do
    local hero = self.transform:FindChild("hero_"..k).gameObject
    hero.gameObject:SetActive(true)
    local battle_hero = battle_hero_ui(hero)
    battle_hero:initData(v)
    battle_hero:heroSetSprite()
    table.insert(self.battle_hero_list,battle_hero.e_actor.guid,battle_hero)	
    
    self.transform:FindChild("hero_"..k):FindChild("anger").gameObject:GetComponent("UnityEngine.UI.Slider").value = 0
    self.transform:FindChild("hero_"..k):FindChild("blood").gameObject:GetComponent("UnityEngine.UI.Slider").value = 1
  end
  
  
  self.hero_btn_fun = function(lua_mono_behaviour)
    printUI("UGUILuaEventListener:"..lua_mono_behaviour.e_actor.guid)
    battlemanager.instance().event_manager:triggerEvent(EventManagerDefine.battle_view2logic_request_utl_skill_click,{["e_actor"] = lua_mono_behaviour.e_actor })
  end

  for k,v in pairs( self.battle_hero_list ) do
    uguieventlistener.addUGUIOnClickListener(UGUILuaEventListener.Get(v), self.hero_btn_fun)
  end
  
  

  self.battle_over_logic.gameObject:SetActive(false)
  --IconManager.Instance:SetUITexture(self.raw_image:GetComponent("UnityEngine.UI.RawImage"),"texture/hero_icon/0009_00_00",true)
  self.battle_controller_ui:doOpen()
  
end

function battle_ui:exchangeWeather(data)
    local msg = data[1]
    local pre_weather_id = msg.pre_weather_id
    local cur_weather_id = msg.weather_id
    local desc_text = self.weather_obj.transform:FindChild("desc_text")
    local text_component = desc_text:GetComponent(typeof(UnityEngine.UI.Text))
    text_component.text = dict_weather.getDataByID(cur_weather_id).desc
    --
    local tweener = self.weather_obj.transform:DOLocalMove(Vector3(-500,80,0) ,battleconfig.move_to_target_time,false)
    tweener:SetEase(DG.Tweening.Ease.Linear)
    
    self:exchangeWeatherIcon(pre_weather_id,cur_weather_id)
  
  
  coroutine.start(function()
    coroutine.wait(battleconfig.weather_news_apper_time) 
       local tweener1 = self.weather_obj.transform:DOLocalMove(self.source_weather_pos,battleconfig.move_to_target_time,false)
       tweener1:SetEase(DG.Tweening.Ease.Linear)
  end)
end

function battle_ui:exchangeWeatherIcon(pre_weather_id,cur_weather_id)
  local prfab = self.weather_trans.transform:GetChild(0).gameObject
  if  prfab ~= nil then
    UGameObject.Destroy(prfab)  
  end
  
  local weather_url = dict_weather.getDataByID(cur_weather_id).prefab_url
  self.weather_icon_prefab = uitools.loadUIObject(weather_url,self.weather_trans.transform)
  --local rect = self.weather_icon_prefab.transform:GetComponent(typeof(UnityEngine.RectTransform))
 -- rect.sizeDelta = Vector2(50,50)
  
end

function battle_ui:startWeatherIcon(cur_weather_id)
  local weather_url = dict_weather.getDataByID(cur_weather_id).prefab_url
  self.weather_icon_prefab = uitools.loadUIObject(weather_url,self.weather_trans.transform)
  --local rect = self.weather_icon_prefab.transform:GetComponent(typeof(UnityEngine.RectTransform))
  -- rect.sizeDelta = Vector2(45,45)
end
  
function battle_ui:changeBloodValue(data)
  local msg = data[1]
  local engine_export_once_data = msg["once_data"]
  if engine_export_once_data.attack_values ~= nil then
    for k,v in pairs (engine_export_once_data.attack_values) do
      
      if v.export_class == ExportValueClass.DEMAGE then
        local battle_hero = self.battle_hero_list[v.target.guid]
        if battle_hero ~= nil then
          battle_hero:changeHeroBloodSlider()
        end
      end
      
      if v.export_class == ExportValueClass.ANGER then
        local battle_hero = self.battle_hero_list[v.source.guid]
        if battle_hero ~= nil then
          battle_hero:changeHeroAngerSlider()
        end
        
        local battle_hero1 = self.battle_hero_list[v.target.guid]
        if battle_hero1 ~= nil then
          battle_hero1:changeHeroAngerSlider()
        end
      end

      end
    end
end



function battle_ui:doClose()
  self.battle_controller_ui:doClose()
  self:unRegisterEventListener()
  
  for k,v in pairs( self.battle_hero_list ) do
    uguieventlistener.removeUGUIOnClickListener(UGUILuaEventListener.Get(v), self.hero_btn_fun)
  end
  self.battle_hero_list = nil
 

end



function battle_ui:showBattleOver(win_lose,desc)
  self.battle_over_logic.gameObject:SetActive(true)
  self.battle_over_logic:showBattleOver(win_lose,desc)
end

return battle_ui
