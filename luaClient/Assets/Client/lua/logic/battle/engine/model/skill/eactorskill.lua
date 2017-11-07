require "logic/common/basemodel"
require "logic/battle/engine/model/skill/eactorskillrelease"

local class = require("common/middleclass")


eactorskill = class("eactorskill",basemodel)


function eactorskill:initialize()


end

function eactorskill:initData(e_actor,data)
  self.e_actor = e_actor
  self.model_data = data
  self.level = 1
  
  self.e_actor_skill_release_s = {}
  local hero_skill_release_model_s = dict_hero_skill_release.getDataByIndexOne(self.model_data.id)
  if hero_skill_release_model_s  ~= nil then
    for k,v in pairs(hero_skill_release_model_s) do
      self.e_actor_skill_release_s[k] = eactorskillrelease(self,v)
    end
  end

  self.e_actor_skill_release_time_event_s = {}
  self.e_actor_skill_project_time_event_s = {}
  self.e_actor_skill_before_event = nil
  self.e_actor_skill_after_event = nil
  self.e_actor_skill_end_event = nil
  
  self.e_actor_goto_skill_pos_event = nil
  self.e_actor_return_source_pos_event = nil
  self.e_actor_skill_exchange_weather_pos_event = nil
  
  
  self.e_actor_skill_cd_event = nil
  
  self.is_ready = true


  self.reset_cd_ready_fun = function()
      self:resetCDReady()
  end
  
  self.goto_skill_po_fun = function()
      self:gotoSkillPosDone()
  end
  
  self.start_skill0_fun = function()
    self:startSkill0()
end
  self.end_skill0_fun = function()
      self:endSkill0()
  end
  self.end_skill_fun = function()
      self:endSkill()
  end
  
  self.return_source_pos_fun = function()
    self:returnSourcePosDone()
  end
  
  self.kill_exchange_weather_fun = function()
      self:skillExchangeWeatherDone()
  end
  
  
  
  self.current_state = SkillState.None
  

end

function eactorskill:isSkilling()
  if self.current_state == SkillState.Before or self.current_state == SkillState.Skilling or self.current_state == SkillState.After then
    return true
  end
  return false
end


function eactorskill:clearRoundEvent()
    self.e_actor.battle_engine.time_event_manager:delete(self.e_actor_skill_exchange_weather_pos_event)
    self.e_actor.battle_engine.time_event_manager:delete(self.e_actor_goto_skill_pos_event)
    self.e_actor.battle_engine.time_event_manager:delete(self.e_actor_skill_before_event)
    self.e_actor.battle_engine.time_event_manager:delete(self.e_actor_skill_after_event)
    self.e_actor.battle_engine.time_event_manager:delete(self.e_actor_skill_end_event)
    self.e_actor.battle_engine.time_event_manager:delete(self.e_actor_return_source_pos_event)
  
    for k,v in pairs(self.e_actor_skill_release_time_event_s) do
      self.e_actor.battle_engine.time_event_manager:delete(self.e_actor_skill_release_time_event_s[k])
    end
    self.e_actor_skill_release_time_event_s = {}
    
    for k,v in pairs(self.e_actor_skill_project_time_event_s) do
      self.e_actor.battle_engine.time_event_manager:delete(self.e_actor_skill_project_time_event_s[k])
    end
    self.e_actor_skill_project_time_event_s ={}
end







function eactorskill:isNeedMove()
  if self.model_data.move_type == "stand_pos" then
    return false
  else
    return true
  end
end


function eactorskill:moveToTargetTime()
    if self:isNeedMove() then
      return battleconfig.move_to_target_time
  else
    return 0
  end

end

function eactorskill:returnToSourceTime()
  if self:isNeedMove() then
      return battleconfig.return_to_source_time
  else
    return 0
  end
end



function eactorskill:preStartSkill()
  printSkill(self.model_data.id.." preStartSkill:............................"..self.model_data.id)

  
  self.targets = self.e_actor.battle_engine.battle_engine_select_util:getTargets(self.e_actor,self.model_data.select_target_obj_type,self.model_data.select_target_strategy_type)
  for k,v in pairs(self.targets) do
    self.target_actor = v
    break
  end
  
  self:clearRoundEvent()
  
  self.is_ready = false
  self.e_actor_skill_cd_event = self.e_actor.battle_engine.round_event_manager:createEvent(self.reset_cd_ready_fun,"reset_cd_ready_fun",self.model_data.cd_time)
  
  
  if self.model_data.exchange_weather_id > 0 then
    local weather_data = dict_weather.getDataByID(self.model_data.exchange_weather_id)
    self.e_actor.battle_engine.e_weather:exchangeWeather(weather_data)
  end
  
  if self:isNeedMove() then
    self:gotoSkillPos()
  else
    self:gotoSkillPosDone()
  end
  
  
end



function eactorskill:gotoSkillPos()
    printSkill(self.model_data.id.." gotoSkillPos")

    self.current_state = SkillState.GotoPos

    self.e_actor_goto_skill_pos_event = self.e_actor.battle_engine.time_event_manager:createEvent(self.goto_skill_po_fun,"goto_skill_po_fun",battleconfig.move_to_target_time)
    self.e_actor.battle_engine.export_engine_event_util:exportSkillMoveToTarget(self,self.target_actor)
    
end



function eactorskill:gotoSkillPosDone()
  printSkill(self.model_data.id.." gotoSkillPosDone")
  self.current_state = SkillState.Before
  
  if self.model_data.before_time <= 0 then
    self:startSkill0()
  else
    self.e_actor_skill_before_event = self.e_actor.battle_engine.time_event_manager:createEvent(self.start_skill0_fun,"start_skill0_fun",self.model_data.before_time)
  end
  
  self.e_actor.battle_engine.export_engine_event_util:exportBeginSkill(self)
end



function eactorskill:startSkill0()
    printSkill(self.model_data.id.." startSkill0")

    self.current_state = SkillState.Skilling

    self.e_actor_skill_end_event = self.e_actor.battle_engine.time_event_manager:createEvent(self.end_skill0_fun,"end_skill0_fun",self.model_data.animation_time)
    
    for k,v in pairs(self.e_actor_skill_release_s) do
      if v.model_data.project_id ~= "" then
        self.e_actor_skill_project_time_event_s[k] = self.e_actor.battle_engine.time_event_manager:createEvent(v.start_project_fun,"start_project_fun",v.model_data.project_release_time)
      end
    
      self.e_actor_skill_release_time_event_s[k] = self.e_actor.battle_engine.time_event_manager:createEvent(v.start_release_fun,"start_release_fun",v.model_data.release_time)
      
    end
    if self.model_data.kind == SkillKind.utl then
      self.e_actor.e_actor_skill_manager.request_utl = false
      local anger_ev = exportvalue()
      local once_data = engineexportoncedata()
      anger_ev:init(self.e_actor,self.e_actor, -100, ExportValueClass.ANGER, HpInfSpecialType.None)
      self.e_actor.battle_engine:doExportData(anger_ev, once_data)
      engineexportoncedata.pushBattleOnceData(once_data)
    else
      local anger_ev = exportvalue()
      local once_data = engineexportoncedata()
      anger_ev:init(self.e_actor,self.e_actor, 30, ExportValueClass.ANGER, HpInfSpecialType.None)
      self.e_actor.battle_engine:doExportData(anger_ev, once_data)
      engineexportoncedata.pushBattleOnceData(once_data)
    end
    
   
end

function eactorskill:releaseOnce(e_skill_release)
  
end

function eactorskill:releaseProject(e_skill_release)
  
end

function eactorskill:exportTargets(e_skill_release,targets)
  
  local once_data = engineexportoncedata.createOnceDataByRelease(e_skill_release)
  
  self:exportHit(e_skill_release,targets,once_data)
  
  engineexportoncedata.pushBattleOnceData(once_data)
end

function eactorskill:exportHit(e_skill_release,targets,once_data)
  
  for k,v in pairs(targets) do
    
      local damage_ev = exportvalue()
      local damage_cal = (self.e_actor.e_actor_attribute_manager.atk_atr:getValue() - v.e_actor_attribute_manager.def_atr:getValue())
      damage_cal = math.max(0,damage_cal)
      damage_ev:init(self.e_actor,v, e_skill_release.model_data.default_dmg_num*(1+self.e_actor.battle_engine.random_util.value)*damage_cal, ExportValueClass.DEMAGE, HpInfSpecialType.None);
      self.e_actor.battle_engine:doExportData(damage_ev, once_data);

      local hit_ev = exportvalue()
      hit_ev:init(self.e_actor,v, 1, ExportValueClass.HIT, HpInfSpecialType.None);
      self.e_actor.battle_engine:doExportData(hit_ev, once_data);
      
      local anger_ev = exportvalue()
      anger_ev:init(self.e_actor, v , 30, ExportValueClass.ANGER, HpInfSpecialType.None);
      self.e_actor.battle_engine:doExportData(anger_ev, once_data);
  end
  
  
end




function eactorskill:endSkill0()
  printSkill(self.model_data.id.." endSkill0")

  self.current_state = SkillState.After
  
  if self.model_data.after_time <= 0 then
    self:endSkill()
  else
    self.e_actor_skill_after_event = self.e_actor.battle_engine.time_event_manager:createEvent(self.end_skill_fun,"end_skill_fun",self.model_data.after_time)
  end
  
end

function eactorskill:endSkill()
    printSkill(self.model_data.id.." endSkill")
    self.current_state = SkillState.End
    
    self.e_actor.battle_engine.export_engine_event_util:exportEndSkill(self)
    
    if self:isNeedMove()  then
      self:returnSourcePos()
    else
      self:returnSourcePosDone()
    end
    
end

function eactorskill:returnSourcePos()
    printSkill(self.model_data.id.." returnSourcePos")

      self.current_state = SkillState.RertunPos
      self.e_actor_return_source_pos_event = self.e_actor.battle_engine.time_event_manager:createEvent(self.return_source_pos_fun,"return_source_pos_fun",battleconfig.return_to_source_time)
      self.e_actor.battle_engine.export_engine_event_util:exportSkillReturnToSource(self)
      
end


function eactorskill:returnSourcePosDone()
  printSkill(self.model_data.id.." returnSourcePosDone")
  self.current_state = SkillState.None
  
  self.e_actor.battle_engine:requestOnceOperation()
end




function eactorskill:interruptSkill()
    
  self.current_state = SkillState.None
  
  self:clearRoundEvent()
  
  self.e_actor.battle_engine.export_engine_event_util:exportInterruptSkill(self)

end


function eactorskill:resetCDReady()
  printSkill(self.model_data.id.." resetCDReady")

  self.is_ready = true
end


function eactorskill:isReady()
  return self.is_ready
end



function eactorskill:startOnceRound()
    
end

function eactorskill:endOnceRound()
    
end

function eactorskill:startOnceOrder()
    self.is_ready = true

    self:clearRoundEvent()
    self.e_actor.battle_engine.round_event_manager:delete(self.e_actor_skill_cd_event)
self.e_actor_skill_cd_event = nil
end

function eactorskill:endOnceOrder()
    self.is_ready = true

    self.e_actor.battle_engine.round_event_manager:delete(self.e_actor_skill_cd_event)
    self.e_actor_skill_cd_event = nil
    self:clearRoundEvent()
end

function eactorskill:clear()
    self.e_actor.battle_engine.round_event_manager:delete(self.e_actor_skill_cd_event)
    self.e_actor_skill_cd_event = nil
    self:clearRoundEvent()
end



function eactorskill:getSkillTime()
    return self.model_data.before_time+self.model_data.after_time+self.model_data.animation_time+self:moveToTargetTime()+self:returnToSourceTime();
end




return eactorskill