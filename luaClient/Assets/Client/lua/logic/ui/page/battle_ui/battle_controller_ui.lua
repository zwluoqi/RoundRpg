


local class = require("common/middleclass")
require "logic/ui/uipart/lua_mono_behaviour"

battle_controller_ui = class("battle_controller_ui",lua_mono_behaviour)



function battle_controller_ui:initData()
    self.btn_auto = self.transform:FindChild("auto_btn").gameObject


  self.order_count_obj = self.transform:FindChild("order_count").gameObject
  self.round_count_obj = self.transform:FindChild("round_count").gameObject
  self.round_remainder_time_obj = self.transform:FindChild("round_remainder_time").gameObject
  
  self.pause_btn = self.transform:FindChild("Image/pause_btn").gameObject
  self.pause_btn_fun = function()
    self:pauseBtnEvent()
  end
  
  
end

function battle_controller_ui:doOpen()
  self.round_remainder_time_obj:SetActive(false)
  
  
  uguieventlistener.addUGUIOnClickListener(UGUIEventListener.Get(self.pause_btn),self.pause_btn_fun)
  
  self.btn_auto_fun = function(go)
    battlemanager.instance().event_manager:triggerEvent(EventManagerDefine.battle_view2logic_request_auto)
    printUI("btn_auto_fun")
  end
  uguieventlistener.addUGUIOnClickListener(UGUIEventListener.Get(self.btn_auto), self.btn_auto_fun)
    
    
  self.battle_view2logic_startonce_round_fun = function()
    
    self.round_count_obj:GetComponent("UnityEngine.UI.Text").text = battlemanager.instance().battle_engine.round_count.."/"..battlemanager.instance().battle_engine.max_round..dict_language["round_desc"]
    
  end
  
  battlemanager.instance().event_manager:addEventListener(EventManagerDefine.battle_view2logic_startonce_round,self.battle_view2logic_startonce_round_fun)
  
  
  
  self.battle_view2logic_startonce_order_fun = function()
    
    self.order_count_obj:GetComponent("UnityEngine.UI.Text").text = battlemanager.instance().battle_engine.order_count.."/"..battlemanager.instance().battle_engine.max_order..dict_language["order_desc"]
    
  end
  
  battlemanager.instance().event_manager:addEventListener(EventManagerDefine.battle_view2logic_startonce_order,self.battle_view2logic_startonce_order_fun)
 
 

    
  self.battle_logic2view_could_use_utl_fun = function()
    self.destroy_remainder_fun()
    self.round_remainder_time_obj:SetActive(true)
    local round_remainder_time_text = self.round_remainder_time_obj:GetComponent("UnityEngine.UI.Text")
    local remainder_timer = battleconfig.wait_view_request_utl_time
    self.remainder_cor = coroutine.start(function()
        while remainder_timer > 0 do
            round_remainder_time_text.text = tostring(remainder_timer)
            remainder_timer = remainder_timer - 1
            coroutine.wait(1)
        end
      end
    )
  end
  battlemanager.instance().event_manager:addEventListener(EventManagerDefine.battle_logic2view_could_use_utl,self.battle_logic2view_could_use_utl_fun)
 
  self.destroy_remainder_fun = function()
    self.round_remainder_time_obj:SetActive(false)
    coroutine.stop(self.remainder_cor)
    self.remainder_cor = nil
  end
  
  battlemanager.instance().event_manager:addEventListener(EventManagerDefine.battle_logic2view_actor_start_skill,self.destroy_remainder_fun)
  battlemanager.instance().event_manager:addEventListener(EventManagerDefine.battle_logic2view_could_endonce_round,self.destroy_remainder_fun)
    battlemanager.instance().event_manager:addEventListener(EventManagerDefine.battle_logic2view_could_endonce_order,self.destroy_remainder_fun)
  battlemanager.instance().event_manager:addEventListener(EventManagerDefine.battle_logic2view_battle_over,self.destroy_remainder_fun)
end

function battle_controller_ui:doClose()
  uguieventlistener.removeUGUIOnClickListener(UGUIEventListener.Get(self.pause_btn),self.pause_btn_fun)
  uguieventlistener.removeUGUIOnClickListener(UGUIEventListener.Get(self.btn_auto), self.btn_auto_fun)
  
    battlemanager.instance().event_manager:removeEventListener(EventManagerDefine.battle_logic2view_actor_start_skill,self.destroy_remainder_fun)
  battlemanager.instance().event_manager:removeEventListener(EventManagerDefine.battle_logic2view_could_endonce_round,self.destroy_remainder_fun)
    battlemanager.instance().event_manager:removeEventListener(EventManagerDefine.battle_logic2view_could_endonce_order,self.destroy_remainder_fun)
  battlemanager.instance().event_manager:removeEventListener(EventManagerDefine.battle_logic2view_battle_over,self.destroy_remainder_fun)
  
  
  battlemanager.instance().event_manager:removeEventListener(EventManagerDefine.battle_view2logic_startonce_round,self.battle_view2logic_startonce_round_fun)
  battlemanager.instance().event_manager:removeEventListener(EventManagerDefine.battle_view2logic_startonce_order,self.battle_view2logic_startonce_order_fun)
  battlemanager.instance().event_manager:removeEventListener(EventManagerDefine.battle_logic2view_could_use_utl,self.battle_logic2view_could_use_utl_fun)
end



function battle_controller_ui:pauseBtnEvent()
    stagemanager.instance():switchStage(StageType.TOWN)
end


return battle_controller_ui
