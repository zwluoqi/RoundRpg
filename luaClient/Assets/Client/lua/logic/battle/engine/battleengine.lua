require "logic/common/basemodel"
require "logic/battle/engine/model/eactor"
require "logic/battle/engine/model/weather/eweather"

require "logic/battle/engine/util/timeeventmanager"

require "logic/battle/engine/exportengineevent"
require "logic/battle/engine/battleengineselect"
require "logic/battle/engine/util/poolmanager"


require ("logic/battle/engine/exportdata/engineexportoncedata")
require ("logic/battle/engine/exportdata/exportvalue")

local class = require("common/middleclass")


battleengine = class("battleengine",basemodel)



--战斗流程
--1.View请求Engine开始工作
--2.View请求Engine开始第N场战斗
--3.View请求Engine开启第N场战斗的第M回合----
--4.1.无操作需要等待大招，Engine通知View在回合中需要等待大招释放---转到6
--4.2.时间到或已无操作，Engine通知View可以结束M回合-------------转到6
--4.3.双方一方全死亡或时间到，则Engine通知View可以结束N战斗-------------转到7
--6.View通知Engine结束第N场战斗的第M回合----转到3
--7.View通知Engine结束第N场战斗----转到2
--8.战斗结束，Engine停止工作



function battleengine:initialize(battle_manager)
    self.battle_manager = battle_manager
  
    self.name = "battleengine"
    
    self.atk_actors_list = nil
    self.def_actors_list = nil
    self.random_seed = -1
    self.copy_id = -1
    self.random_util = RandomUtil.New()
    
    self.ordered_actors = nil
    self.running_actors = nil
    self.running_actors_changed = false
    self.current_atk_team_list = nil
    self.working = false
    
    self.time_event_manager = timeeventmanager("time_event")
    self.round_event_manager = timeeventmanager("round_event")
    self.pool_manager = poolmanager()
    
    self.export_engine_event_util = exportengineevent()
    self.battle_engine_select_util = battleengineselect()
    

    
    self.operationing = false
    self.wait_view_request_end_round  = false
    self.wait_view_request_end_order = false
    
    
    
    self.request_once_operation_fun = function()
      self:requestOnceOperation()
    end
    
    
    
    printLog("battleengine:initialize()")
end


function battleengine:ready()
  self.time_event_manager:ready()
  self.round_event_manager:ready()
  self.pool_manager:ready()
  self.round_count  = 0
  self.order_count  = 0
  self.max_order = table.nums(self.def_actors_list)
  self.max_round = battleconfig.max_round
  
  self.ordered_actors = {}
  self.running_actors = {}
  self.running_actors_changed = false
  self.rounding = false
  self.ordering = false

  self.current_atk_team_list = nil

    
  printLog("num ready...........:".. table.nums(self.atk_actors_list))

  self:registerEvent()
end


function battleengine:clear()
  self:unRegisterEvent()

  for k,v in pairs(self.atk_actors_list) do
    for ik,iv in pairs(v) do
      iv:clear()
    end
  end
  self.atk_actors_list = nil

  for k,v in pairs(self.def_actors_list) do
    for ik,iv in pairs(v) do
      iv:clear()
    end
  end
  self.def_actors_list = nil
  
  self.copy_id = -1
  self.random_seed = -1
  
  self.ordered_actors = nil
  self.running_actors = nil
  self.running_actors_changed = false
  self.current_atk_team_list = nil
  self.time_event_manager:clear()
  self.round_event_manager:clear()
  self.pool_manager:clear()
  
  self.e_weather = nil
  
  self.operationing = false
  self.working = false
  
end


function battleengine:setRoomData(input_room_data)

    
    self.copy_id = input_room_data.copy_id
    
    self.scene_id = input_room_data.scene_id
    self.scene_data = dict_scene.getDataByID(self.scene_id)
    self.e_weather = eweather(self)
    self.e_weather:initData(self.scene_data.default_temperature,self.scene_data.default_humidity)
    
    self.random_seed  = input_room_data.random_seed
    self.random_util:SetSeed(input_room_data.random_seed)
    self.atk_actors_list = {}
    self.def_actors_list = {}
    
    for k,v in pairs(input_room_data.input_atk_actors) do
      self.atk_actors_list[k] = {}
      for ik,iv in pairs(v) do
        local e_actor = eactor(iv)
        self.atk_actors_list[k][ik] = e_actor
      end
    end
    
    for k,v in pairs(input_room_data.input_def_actors) do
      self.def_actors_list[k] = {}
      for ik,iv in pairs(v) do
        local e_actor = eactor(iv)
        self.def_actors_list[k][ik] = e_actor
      end
    end
    printLog("num setRoomData...........:".. table.nums(self.atk_actors_list))

end


function battleengine:actorEnter(e_actor)
  if self.ordered_actors[e_actor.guid]  ~= nil then
    printError("actor enter error:"..e_actor.guid)
    return false
  end
  
  self.ordered_actors[e_actor.guid] = e_actor
  self.running_actors_changed = true
  return true
end



function battleengine:actorLeave(e_actor)
  if self.ordered_actors[e_actor.guid]  == nil then
    printError("actor leave error:"..e_actor.guid)
    return false
  end
  
  self.ordered_actors[e_actor.guid] = nil
  self.running_actors_changed = true
  return true
end

--开启第N场战斗
function battleengine:startOnceOrder()
  if self.ordering then
    printError("本场战斗已经开始 无法再次开启")
    return
  end
  
  self.ordering = true
  self.operationing = false
  self.order_remainder_time = battleconfig.order_remainder_time
  self.wait_view_request_end_order = false

  self.order_count  = self.order_count+1
  self.round_count = 0
  
  for e_guid,e_actor in pairs(self.current_atk_team_list) do
    if self:actorEnter(e_actor) then
      e_actor:startOnceOrder()
    end
  end
  
  self:orderEnemyReady(self.order_count)
end

--结束第N场战斗
function battleengine:endOnceOrder()
  if not self.ordering then
    printError("本场战斗已经结束 无法再次结束")
    return
  end
  
  self.ordering = false
  for e_guid,e_actor in pairs(self.current_atk_team_list) do
    if self:actorLeave(e_actor) then
      e_actor:endOnceOrder()
    end
  end
  
  self:orderEnemyClear(self.order_count)
end
  
--开启第M回合
function battleengine:startOnceRound()
  --TODO
  if self.rounding then
    printError("回合正在进行无法再次开启")
    return
  end
  
  self.round_count = self.round_count+1
  self.rounding = true
  printBattleEngine("startOnceRound:"..tostring(self.round_count))


  self.operationing = false
  self.round_remainder_time = battleconfig.round_remainder_time
  self.wait_view_request_end_round = false

  self.round_event_manager:tick(1)

  
    
  for k,v in pairs(self.running_actors) do
      v:startOnceRound()
  end
    
  
  self:requestOnceOperation()
end

--结束第M回合
function battleengine:endOnceRound()
  if not self.rounding then
    printError("本回合已经结束 无法再次结束")
    return
  end
  
  
    self.rounding = false
    self.e_weather:endOnceRound()
    printBattleEngine("endOnceRound:"..tostring(self.round_count))
    for k,v in pairs(self.running_actors) do
        v:endOnceRound()
    end
end

function battleengine:requestOnceOperation()
    if self.running_actors_changed then
      self.running_actors =  table.simpleCopy(self.ordered_actors)
    end

    
    printBattleEngine("requestOnceOperation:")
    
    --1.check battle state
    local is_complete_battle,win_lose,desc = self:checkCompleteBattle()
    if is_complete_battle then
      self:stopWorking()
      self.export_engine_event_util:exportEndBattlingMsg(win_lose,desc)
      return
    elseif desc == "could_enter_next_order" then
      self.wait_view_request_end_order = true
      self.export_engine_event_util:exportCouldEndOnceOrder()
      return
    else
      --contine
    end
        
    local sort_e_actor = {}
    for k,v in pairs(self.running_actors) do
      if not v.is_dead then
        eactor.sortEActorByAtkSpeed(v,sort_e_actor)
      end
    end
    

  
    --get utl skill
    local wait_util = false
    local operation = nil
    for index,e_actor in pairs(sort_e_actor) do
        local is_ready,ready_skill = e_actor:checkUtlSkillReady()
        if is_ready then
          if e_actor.is_auto then
            operation = {["operaion_type"] = "start_skill",["ready_skill"] = ready_skill}
            break
          elseif e_actor.e_actor_skill_manager.request_utl then
            operation = {["operaion_type"] = "start_skill",["ready_skill"] = ready_skill}
            break
          else
            wait_util = true
          end
        end
    end
    
    --if do not get utl skill,will get skill skill
    if operation == nil then
      for index,e_actor in pairs(sort_e_actor) do
          local is_ready,ready_skill = e_actor:checkNormalSkillReady(Vector3.zero)
          if is_ready then
            operation = {["operaion_type"] = "start_skill",["ready_skill"] = ready_skill}
            break
          end
      end
    end
  
    
    if operation == nil then
      self:doOnceWait(wait_util)
    else
      self:doOnceOperation(operation)
    end
    
end

function battleengine:doOnceWait(wait_util )
      self.operationing = false

      if wait_util then 
        --send to view wait view request util
        self.time_event_manager:delete(self.wait_view_request_utl_event)
        self.wait_view_request_utl_event = self.time_event_manager:createEvent(function()
            self:doSendCouldEndOnceRound()
          end
          ,"wait_view_request_utl_event",battleconfig.wait_view_request_utl_time)
        self.export_engine_event_util:exportWaitRequestUtil()
      else
        
        --send to view wait view  request end round
            self:doSendCouldEndOnceRound()
      end
end

function battleengine:doSendCouldEndOnceRound()
        self.wait_view_request_end_round = true
        self.export_engine_event_util:exportCouldEndOnceRound()
end



function battleengine:doOnceOperation(operation)
      self.operationing = true
      
      if operation["operaion_type"] == "start_skill" then
         local ready_skill = operation["ready_skill"]
         ready_skill.e_actor:preStartSkill(ready_skill)
      else
       printError("error operation")
      end
end


function battleengine:tick()
    
    
    if self.working then
      
      self.delta_time = self.battle_manager.delta_time
      
      self.round_remainder_time = self.round_remainder_time  - self.delta_time
      self.order_remainder_time = self.order_remainder_time  - self.delta_time
      self.battle_remainder_time = self.battle_remainder_time  - self.delta_time

      if self.running_actors_changed then
        self.running_actors =  table.simpleCopy(self.ordered_actors)
      end
    
      self.time_event_manager:tick(self.battle_manager.delta_time)
    end
  
end





function battleengine:checkCompleteBattle()

  if self:isCurrentAttackerDead() then
    return true,"lose","attack_dead"
  end
  
  local could_enter_next_order = false
  if self:isCurrentDefenceDead() then
    if self.order_count == self.max_order then
        return true,"win","defence_dead"
    else
      could_enter_next_order = true
    end
  end
  
  if self.battle_remainder_time < 0 then
    return true,"lose","battle_remainder_time"
  end
  
  if self.order_remainder_time < 0 then
    return true,"lose","order_remainder_time"
  end
  
  if self.round_count == self.max_round then
    return true,"lose","max_round"
  end
  
  if could_enter_next_order then
    return false,"continue","could_enter_next_order"
  else
    return false,"continue","continue"
  end
  
  
end


function battleengine:isCurrentAttackerDead()
    for k,v in pairs( self.current_atk_team_list) do
      if v.is_dead == false then
        return false
      end
    end
    return  true
end

function battleengine:isCurrentDefenceDead()
    local order_enemys = self.def_actors_list[self.order_count]
    for k,v in pairs( order_enemys) do
      if v.is_dead == false then
        return false
      end
    end
    return  true
end


function battleengine:orderEnemyReady(order)
  printLog("orderEnemyBorn"..order)
    local order_enemys = self.def_actors_list[order]
    for k,v in pairs( order_enemys) do
      if self:actorEnter(v) then
        v:startOnceOrder()
        v.is_auto = true
      end
    end
    
end

function battleengine:orderEnemyClear(order)
    printLog("orderEnemyDead"..order)
    local order_enemys = self.def_actors_list[order]
    for k,v in pairs( order_enemys) do
      if self:actorLeave(v) then
        v:endOnceOrder()
      end
    end
    
end



function battleengine:getHeroByCamps(logic_camp_types)
  
  local select_targets = {}
  for k,v in pairs(self.running_actors) do
    if v.is_dead == false then
      
      if logic_camp_types[v.input_actor.logic_camp_type] then
          select_targets[k] = v
      end
      
    end
    
  end
  return select_targets
end

function battleengine:stopWorking()

  
  
  for k,v in pairs(self.atk_actors_list) do
    for ik,iv in pairs(v) do
      iv:clear()
    end
  end


  for k,v in pairs(self.def_actors_list) do
    for ik,iv in pairs(v) do
      iv:clear()
    end
  end
  
  self.time_event_manager:clear()
  self.round_event_manager:clear()
  self.pool_manager:clear()
  
  
  self.operationing = false
  self.working = false
  
end



function battleengine:viewMsgStartWroking()
  printLog("viewMsgStartWroking")
  
  printLog("num viewMsgStartWroking...........:".. table.nums(self.atk_actors_list))
  self.working = true

  self.current_atk_team_list = self.atk_actors_list[1]
  self.battle_remainder_time = battleconfig.battle_remainder_time
end


function battleengine:viewMsgStartOnceOrder()
  self:startOnceOrder()
end


function battleengine:viewMsgEndOnceOrder()
  self:endOnceOrder()
end


function battleengine:viewMsgStartOnceRound()
  self:startOnceRound()
end


function battleengine:viewMsgEndOnceRound()
  self:endOnceRound()
end

function battleengine:viewMsgRequestUtlSkillCilck(data)
  msg = data[1]
  local e_actor = msg["e_actor"]

  if not e_actor:checkUtlSkillReady() then
    return
  end
  
  self.time_event_manager:delete(self.wait_view_request_utl_event)


  
  e_actor.e_actor_skill_manager.request_utl = true
  
  if not self.rounding then
    return
  end
  if self.wait_view_request_end_round then
    return
  end
  if self.wait_view_request_end_order then
    return
  end

  
  
  if self.operationing == false then 
    self:requestOnceOperation()
  end

end

function battleengine:viewMsgRequestAuto()

  for k,v in pairs(self.current_atk_team_list) do
    v.is_auto = not v.is_auto
  end
  
  if not self.rounding then
    return
  end
  if self.wait_view_request_end_round then
    return
  end
  if self.wait_view_request_end_order then
    return
  end
  
  if self.operationing == false then 
    self:requestOnceOperation()
  end
end


function battleengine:registerEvent()
  self.start_working_fun =  
    function()
      self:viewMsgStartWroking()
    end
    
    self.start_round_fun =  
    function()
      self:viewMsgStartOnceRound()
    end
    
    self.end_round_fun =  
    function()
      self:viewMsgEndOnceRound()
    end
    
    self.start_order_fun =  
    function()
      self:viewMsgStartOnceOrder()
    end
    
    self.end_order_fun =  
    function()
      self:viewMsgEndOnceOrder()
    end
    
    
    self.request_utl_skill_click =  
    function(data)
      self:viewMsgRequestUtlSkillCilck(data)
    end
    
       self.request_auto_fun =  
    function()
      self:viewMsgRequestAuto()
    end 
    
  self.battle_manager.event_manager:addEventListener(EventManagerDefine.battle_view2logic_startworking,
self.start_working_fun)
  self.battle_manager.event_manager:addEventListener(EventManagerDefine.battle_view2logic_startonce_round,
self.start_round_fun)
  self.battle_manager.event_manager:addEventListener(EventManagerDefine.battle_view2logic_endonce_round,
self.end_round_fun)

  self.battle_manager.event_manager:addEventListener(EventManagerDefine.battle_view2logic_startonce_order,
self.start_order_fun)
  self.battle_manager.event_manager:addEventListener(EventManagerDefine.battle_view2logic_endonce_order,
self.end_order_fun)

  self.battle_manager.event_manager:addEventListener(EventManagerDefine.battle_view2logic_request_utl_skill_click,
self.request_utl_skill_click)
    self.battle_manager.event_manager:addEventListener(EventManagerDefine.battle_view2logic_request_auto,
self.request_auto_fun)
end

function battleengine:unRegisterEvent()
    self.battle_manager.event_manager:removeEventListener(EventManagerDefine.battle_view2logic_startworking,self.start_working_fun)
    self.battle_manager.event_manager:removeEventListener(EventManagerDefine.battle_view2logic_startonce_round,
self.start_round_fun)
  self.battle_manager.event_manager:removeEventListener(EventManagerDefine.battle_view2logic_endonce_round,
self.end_round_fun)

  self.battle_manager.event_manager:removeEventListener(EventManagerDefine.battle_view2logic_startonce_order,
self.start_order_fun)
  self.battle_manager.event_manager:removeEventListener(EventManagerDefine.battle_view2logic_endonce_order,
self.end_order_fun)

  self.battle_manager.event_manager:removeEventListener(EventManagerDefine.battle_view2logic_request_utl_skill_click,
self.request_utl_skill_click)
    self.battle_manager.event_manager:removeEventListener(EventManagerDefine.battle_view2logic_request_auto,
self.request_auto_fun)
end





function battleengine:doExportData(export_data,once_data)
  table.insert(once_data.attack_values,export_data)

  export_data.target:doExportData(export_data,once_data)
end








return battleengine