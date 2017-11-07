require("logic/battle/view/fsm/init")
require "logic/common/enumdefine"
require "logic/common/basemodel"
require "logic/battle/view/model/vactor"
require "logic/battle/view/model/vskillproject/vskillproject"

local class = require("common/middleclass")
battleview = class("battleview",basemodel)


function battleview:initialize(battle_manager)
      printLog("battleview:initialize()")

      self.battle_manager = battle_manager
      self.name = "battleview"
      
      self.delta_time = 0
      
      self.v_project_dicts = {}

      self.battle_actor_pref_game_object = UGameObject.New("battle_actor_pref_game_object")
      self.battle_actor_pref_game_object.transform:SetParent(battle_manager.manager_game_object.transform)
      unitytools.resetPos(self.battle_actor_pref_game_object)
      
      self.battle_particle_pref_game_object = UGameObject.New("battle_particle_pref_game_object")
      self.battle_particle_pref_game_object.transform:SetParent(battle_manager.manager_game_object.transform)
      unitytools.resetPos(self.battle_particle_pref_game_object)

      self:initViewFSM()
end


function battleview:initViewFSM()
      self.view_fsm = fsmcontroller()
      self.view_fsm.state_dict[BattleViewState.EMPTY] = viewfsmemptystate()
      self.view_fsm.state_dict[BattleViewState.LOADING] = viewfsmloadingstate()
      self.view_fsm.state_dict[BattleViewState.RUNNING] = viewfsmrunningstate()
      self.view_fsm.state_dict[BattleViewState.FINISH] = viewfsmfinishstate()
      
      self.view_fsm:switchState(BattleViewState.EMPTY)
      
end


function battleview:tick()
    self.delta_time = self.delta_time+ self.battle_manager.delta_time
    self.view_fsm:tick()
end




function battleview:ready()
    self.scene_relation = UGameObject.Find("scene_relation")
    self.scene_relation_mono = self.scene_relation:GetComponent(typeof(SceneRelation))
    
    self:registerEvent()
    
    
    self.v_actors = {}

    for k,v in pairs( self.battle_manager.battle_engine.atk_actors_list) do
        for ik,iv in pairs(v) do
          local trans = self.scene_relation_mono.attacks_pos[iv.input_actor.pos_id - 1]
          iv.input_actor.position_x = trans.position.x
          iv.input_actor.position_y = trans.position.y
          iv.input_actor.position_z = trans.position.z
          iv.input_actor.euler_y = trans.eulerAngles.y
          
          
          self.v_actors[iv.guid] = vactor(iv)
          self.v_actors[iv.guid]:loadResource()
          self.v_actors[iv.guid].v_actor_ui:setHpState(true)
          self.v_actors[iv.guid]:hide()
        end
    end
    
    for k,v in pairs( self.battle_manager.battle_engine.def_actors_list) do
        for ik,iv in pairs(v) do
          local trans = self.scene_relation_mono.defences_pos[iv.input_actor.pos_id - 1]
          iv.input_actor.position_x = trans.position.x
          iv.input_actor.position_y = trans.position.y
          iv.input_actor.position_z = trans.position.z
          iv.input_actor.euler_y = trans.eulerAngles.y
          
          self.v_actors[iv.guid] = vactor(iv)
          self.v_actors[iv.guid]:loadResource()
          self.v_actors[iv.guid].v_actor_ui:setHpState(false)
          self.v_actors[iv.guid]:hide()
        end
    end
    

    
end


function battleview:clear()
  self:unRegisterEvent()
  
  
  for k,v in pairs(self.v_actors) do
      v:unLoadResource()
      v:clear()
      v:destroy()
  end
  self.v_actors = nil
end


function battleview:showCurrentAttack()
  for k,v in pairs(self.battle_manager.battle_engine.current_atk_team_list) do
    local v_actor = self.v_actors[v.guid]
    if v_actor ~= nil then
      v_actor:show()
    end
  end
end

function battleview:readyCurrentAttack()
  for k,v in pairs(self.battle_manager.battle_engine.current_atk_team_list) do
    local v_actor = self.v_actors[v.guid]
    if v_actor ~= nil then
      v_actor:ready()
    end
  end
end



--
function battleview:showOrderEnemy(order)
  for k,v in pairs(self.battle_manager.battle_engine.def_actors_list[order]) do
    local v_actor = self.v_actors[v.guid]
    if v_actor ~= nil then
      v_actor:show()
    end
  end
end

function battleview:readyOrderEnemy(order)
  for k,v in pairs(self.battle_manager.battle_engine.def_actors_list[order]) do
    local v_actor = self.v_actors[v.guid]
    if v_actor ~= nil then
      v_actor:ready()
    end
  end
end

--
function battleview:hideOrderEnemy(order)
  for k,v in pairs(self.battle_manager.battle_engine.def_actors_list[order]) do
    local v_actor = self.v_actors[v.guid]
    if v_actor ~= nil then
      v_actor:hide()
    end
  end
end



function battleview:registerEvent()
  
  self.skill_move_target_fun =
  function(data)
    self:skillMoveTarget(data)
  end
  self.battle_manager.event_manager:addEventListener(EventManagerDefine.battle_logic2view_skill_move_to_target,self.skill_move_target_fun)
  
  self.skill_return_source_fun =
  function(data)
    self:skillReturnSource(data)
  end
  self.battle_manager.event_manager:addEventListener(EventManagerDefine.battle_logic2view_skill_return_to_source,self.skill_return_source_fun)
  
  self.actor_start_skill_fun = 
  function(data)
    self:actorStartSkill(data)
  end
    self.battle_manager.event_manager:addEventListener(EventManagerDefine.battle_logic2view_actor_start_skill,self.actor_start_skill_fun)
    
  self.actor_interrupt_skill_fun = 
  function(data)
    self:actorInterruptSkill(data)
  end
      self.battle_manager.event_manager:addEventListener(EventManagerDefine.battle_logic2view_actor_interrupt_skill,self.actor_interrupt_skill_fun)

  self.actor_end_skill_fun = 
  function(data)
    self:actorEndSkill(data)
  end
      self.battle_manager.event_manager:addEventListener(EventManagerDefine.battle_logic2view_actor_end_skill,self.actor_end_skill_fun)
  
  self.actor_skill_export_fun = 
  function(data)
    self:actorSkillExport(data)
  end
        self.battle_manager.event_manager:addEventListener(EventManagerDefine.battle_logic2view_actor_skill_export,self.actor_skill_export_fun)

            
  self.battle_over_fun = 
  function(data)
    self:battleOver(data)
  end
        self.battle_manager.event_manager:addEventListener(EventManagerDefine.battle_logic2view_battle_over,self.battle_over_fun)
  
  self.skill_release_project_fun = 
  function(data)
    self:skillReleaseProject(data)
  end
  
  self.battle_manager.event_manager:addEventListener(EventManagerDefine.battle_logic2view_skill_release_project,self.skill_release_project_fun)
  
  self.could_endonce_order_fun = 
  function(data)
    self:couldEndOnceOrder(data)
  end
  
  self.battle_manager.event_manager:addEventListener(EventManagerDefine.battle_logic2view_could_endonce_order,self.could_endonce_order_fun)
  
  self.could_endonce_round_fun = 
  function(data)
    self:couldEndOnceRound(data)
  end
  
  self.battle_manager.event_manager:addEventListener(EventManagerDefine.battle_logic2view_could_endonce_round,self.could_endonce_round_fun)
  
    self.exchange_weather_fun = 
  function(data)
    self:exchangeWeather(data)
  end
  
  self.battle_manager.event_manager:addEventListener(EventManagerDefine.battle_logic2view_exchange_weather,self.exchange_weather_fun)
  
end





function battleview:unRegisterEvent()
  self.battle_manager.event_manager:removeEventListener(EventManagerDefine.battle_logic2view_skill_move_to_target,self.skill_move_target_fun)
  self.battle_manager.event_manager:removeEventListener(EventManagerDefine.battle_logic2view_skill_return_to_source,self.skill_return_source_fun)
  self.battle_manager.event_manager:removeEventListener(EventManagerDefine.battle_logic2view_actor_start_skill,self.actor_start_skill_fun)
  self.battle_manager.event_manager:removeEventListener(EventManagerDefine.battle_logic2view_actor_interrupt_skill,self.actor_interrupt_skill_fun)
  self.battle_manager.event_manager:removeEventListener(EventManagerDefine.battle_logic2view_actor_end_skill,self.actor_end_skill_fun)
  self.battle_manager.event_manager:removeEventListener(EventManagerDefine.battle_logic2view_actor_skill_export,self.actor_skill_export_fun)
  self.battle_manager.event_manager:removeEventListener(EventManagerDefine.battle_logic2view_battle_over,self.battle_over_fun)
  self.battle_manager.event_manager:removeEventListener(EventManagerDefine.battle_logic2view_skill_release_project,self.skill_release_project_fun)
  self.battle_manager.event_manager:removeEventListener(EventManagerDefine.battle_logic2view_could_endonce_order,self.could_endonce_order_fun)
  self.battle_manager.event_manager:removeEventListener(EventManagerDefine.battle_logic2view_could_endonce_round,self.could_endonce_round_fun)
  
    self.battle_manager.event_manager:removeEventListener(EventManagerDefine.battle_logic2view_exchange_weather,self.exchange_weather_fun)
    
end



function battleview:actorStartSkill(data)
  local msg = data[1]
  
  local v_actor = self.v_actors[msg.skill.e_actor.guid]
  v_actor:startSkill(msg.skill)
end

function battleview:actorInterruptSkill(data)
  local msg = data[1]
  
  local v_actor = self.v_actors[msg.skill.e_actor.guid]
  v_actor:interruptSkill(msg.skill)
end


function battleview:actorEndSkill(data)
  local msg = data[1]

  local v_actor = self.v_actors[msg.skill.e_actor.guid]
  v_actor:endSkill(msg.skill)
end

function battleview:actorSkillExport(data)
  local msg = data[1]
  local engine_export_once_data = msg["once_data"]
 if engine_export_once_data.attack_values ~= nil then
    for k,v in pairs (engine_export_once_data.attack_values) do
      if v.export_class == ExportValueClass.HIT then
         local v_actor = self.v_actors[v.target.guid]
         
         if engine_export_once_data.release ~= nil then
          for k,v in pairs(engine_export_once_data.release.model_data.release_effect_id_s) do
            v_actor.v_actor_effect:bindEffect(v)
          end
         end
         
         
         local play_count = v_actor.v_actor_model:playAnim("hit",UnityEngine.WrapMode.Once)
         local hit_time = v_actor.v_actor_model.anim_time_data_s["hit"]
         coroutine.start(function()
            coroutine.wait(hit_time) 
            if play_count == v_actor.v_actor_model:getCurrentPlayCount() then
              v_actor.v_actor_model:playAnim("idle")
            end
          end)
        
      elseif v.export_class == ExportValueClass.DEMAGE then
        local v_actor = self.v_actors[v.target.guid]
        v_actor.v_actor_ui:freshHp()
      else
        --end
      end
    end
  end
  
  if engine_export_once_data.dead_list ~= nil then
    for k,v in pairs (engine_export_once_data.dead_list) do
    local v_actor = self.v_actors[v]
    v_actor.v_actor_model:playAnim("die",UnityEngine.WrapMode.Once)
    local die_time = v_actor.v_actor_model.anim_time_data_s["die"]
    coroutine.start(function()
        coroutine.wait(die_time) 
       v_actor:hide()
    end)
    end
  end
  
  --[[if engine_export_once_data.skill ~= nil then
    print("skill"..engine_export_once_data.skill)
  end
  if engine_export_once_data.release ~= nil then
    print("release"..engine_export_once_data.release)
  end
  if engine_export_once_data.buff ~= nil then
    print("buff"..engine_export_once_data.buff)
  end
  if engine_export_once_data.source ~= nil then
    print("source"..engine_export_once_data.source)
  end
  if engine_export_once_data.target ~= nil then
    print("target"..engine_export_once_data.target)
  end
  ]]
end


function battleview:battleOver(data)
  local msg = data[1]
  printBattleView("win_lose:"..msg.win_lose.." desc:"..msg.desc)
  uimanager.instance():getPageManager():getOpenedPage("battle_ui"):showBattleOver(msg.win_lose,msg.desc)
end

function battleview:skillMoveTarget(data)
  local msg = data[1]
  local v_actor = self.v_actors[msg.e_actor_skill.e_actor.guid]
  local e_target_actor = msg["e_target_actor"]
  --v_actor.unity_v_actor_game_object.transform:DOLocalMove(Vector3.New(1,1,-2),3,false)
   v_actor.v_actor_model:playAnim("move")
   local dir = nil
local target_pos = MathUtil.TransPosition(Vector3.New(0,0,battleconfig.reduce_distance),e_target_actor:getLocalPosition(),e_target_actor:getForward(), dir)
  local tweener = v_actor.unity_v_actor_game_object.transform:DOLocalMove(target_pos,battleconfig.move_to_target_time,false)
  tweener:SetEase(DG.Tweening.Ease.Linear)
 -- - Vector3.New(0,0,-0.5)
end

function battleview:skillReturnSource(data)
  local msg = data[1]
  local v_actor = self.v_actors[msg.e_actor_skill.e_actor.guid]
  local e_source_actor = msg.e_actor_skill.e_actor.input_actor
  --v_actor.unity_v_actor_game_object.transform:DOLocalMove(Vector3.New(1,1,3),3,false)
  local play_count = v_actor.v_actor_model:playAnim("move")
  local tweener = v_actor.unity_v_actor_game_object.transform:DOLocalMove(Vector3.New(e_source_actor.position_x,e_source_actor.position_y,e_source_actor.position_z),battleconfig.return_to_source_time,false)
  tweener:SetEase(DG.Tweening.Ease.Linear)
  
  coroutine.start(function()
        coroutine.wait(battleconfig.return_to_source_time)
        local current_play_count= v_actor.v_actor_model:getCurrentPlayCount()
        if play_count == current_play_count then
          v_actor.v_actor_model:playAnim("idle")
        end
  end)
end

function battleview:skillReleaseProject(data)
    local msg = data[1]
    
    local e_actor_skill_release = msg["e_actor_skill_release"]
    local e_target_actor = msg["e_target_actor"]
    local e_source_skill = e_actor_skill_release.e_actor_skill
    local e_source_actor = e_source_skill.e_actor
    
    local project_id = e_actor_skill_release.model_data.project_id
    local project_data = dict_hero_skill_release_project.getDataByID(project_id)
    local effect_data = dict_effect_resource.getDataByID( project_data.effect_res_id)
    local project_uri = effect_data.url
    local duration = e_actor_skill_release.model_data.release_time - e_actor_skill_release.model_data.project_release_time
    --local source_pos = e_source_actor:getLocalPosition() + Vector3(0,1,-1)
   -- local target_pos = e_target_actor:getLocalPosition() + Vector3(0,1,1)
    local dir = nil
    local source_pos = MathUtil.TransPosition(Vector3(project_data.projectile_default_x,project_data.projectile_default_y,project_data.projectile_default_z),e_source_actor:getLocalPosition(),e_source_actor:getForward(), dir)
    local target_pos = MathUtil.TransPosition(Vector3(project_data.target_x,project_data.target_y,project_data.target_z),e_target_actor:getLocalPosition(),e_target_actor:getForward(), dir)
    self:addProject(source_pos,target_pos,project_uri,duration)
end


function battleview:addProject(source_pos,target_pos,project_uri,duration)
  local v_skill_project = vskillproject()
  v_skill_project:initData(source_pos,target_pos,project_uri,duration)
  self.v_project_dicts[v_skill_project.guid] = v_skill_project
  v_skill_project:startProject()
  

end


function battleview:removeProject(guid)
  self.v_project_dicts[guid] = nil
end

function battleview:couldEndOnceOrder()
  

  --wait one second
  coroutine.start(function()
          local resume_weather,resume_weather_data = self.battle_manager.battle_engine.e_weather:getEndRoundResumeNormalWeather()
        if resume_weather then
          print("还原天气 delta_time:"..self.delta_time)
          self:endWeather(self.battle_manager.battle_engine.e_weather.current_weather_data.weather_id)
          self:startWeather(resume_weather_data.weather_id)
          coroutine.wait(4) 
        end
        
        print("couldEndOnceOrder delta_time:"..self.delta_time)
        self:hideOrderEnemy(self.battle_manager.battle_engine.order_count)

        coroutine.wait(1) 
        print("requestEndOnceRound delta_time:"..self.delta_time)
        --结束当前回合
        self.battle_manager.event_manager:triggerEvent(EventManagerDefine.battle_view2logic_endonce_round)

        coroutine.wait(1) 
        print("requestEndOnceOrder delta_time:"..self.delta_time)
        --结束当前战斗
        self.battle_manager.event_manager:triggerEvent(EventManagerDefine.battle_view2logic_endonce_order)
        
        
        coroutine.wait(1) 
        print("requestStartOnceOrder delta_time:"..self.delta_time) 
        --开启下一场战斗
        self:showOrderEnemy(self.battle_manager.battle_engine.order_count+1)
        self:readyOrderEnemy(self.battle_manager.battle_engine.order_count+1)
        self.battle_manager.event_manager:triggerEvent(EventManagerDefine.battle_view2logic_startonce_order)

        coroutine.wait(1) 
        --开启下一场战斗的回合
        print("requestStartOnceRound delta_time:"..self.delta_time) 
        self.battle_manager.event_manager:triggerEvent(EventManagerDefine.battle_view2logic_startonce_round)
        
    end)
  
end

function battleview:couldEndOnceRound()
  --wait one second
  coroutine.start(function()
      
        local resume_weather,resume_weather_data = self.battle_manager.battle_engine.e_weather:getEndRoundResumeNormalWeather()
        if resume_weather then
          print("还原天气 delta_time:"..self.delta_time)
          self:endWeather(self.battle_manager.battle_engine.e_weather.current_weather_data.weather_id)
          self:startWeather(resume_weather_data.weather_id)
          coroutine.wait(4) 
        end
        
        
      
      
        print("requestEndOnceRound delta_time:"..self.delta_time)
        --结束当前回合
        self.battle_manager.event_manager:triggerEvent(EventManagerDefine.battle_view2logic_endonce_round)

        coroutine.wait(1) 
        --开启下一个的回合
        
        --print(self.delta_time)
        print("requestStartOnceRound delta_time:"..self.delta_time) 
        self.battle_manager.event_manager:triggerEvent(EventManagerDefine.battle_view2logic_startonce_round)
    end)
end


function battleview:exchangeWeather(data)
    local msg = data[1]

    printBattleView("exchangeWeather from:"..msg.pre_weather_id.." to:"..msg.weather_id)
    --TODO
    
    self:endWeather(msg.pre_weather_id)
    self:startWeather(msg.weather_id)
end

function battleview:endWeather(weather_id)
  printBattleView("endWeather:"..weather_id)
  local target_weather_data = dict_weather.getDataByID(weather_id)
  
  --切出特效
  for k,v in pairs(target_weather_data.weather_exchange_particle_id_s) do
      
      local target_weather_exchange_particle_data = dict_weather_exchange_particle.getDataByID(v)
      
      local effect_data = dict_effect_resource.getDataByID(target_weather_exchange_particle_data.effect_id)
      
      local paricle_exchange = self.scene_relation_mono:StartUnLoadResource(effect_data.url,target_weather_exchange_particle_data.leave_particle_unload_time)
           
      local args_num = table.nums(target_weather_exchange_particle_data.leave_particle_args_name_s)

      if args_num > 0 then
        for i=1,args_num do
          local str = System.String.New(target_weather_exchange_particle_data.leave_particle_args_name_s[i])
          if str:Contains('_color') then
            paricle_exchange:StartExchangeParticleColor(target_weather_exchange_particle_data.leave_particle_args_name_s[i],target_weather_exchange_particle_data.leave_particle_args_target_value_s[i],target_weather_exchange_particle_data.leave_particle_args_start_time_s[i],target_weather_exchange_particle_data.leave_particle_args_duration_s[i])
          else
            paricle_exchange:StartLeaveWeatherParticle(target_weather_exchange_particle_data.leave_particle_args_name_s[i],target_weather_exchange_particle_data.leave_particle_args_target_value_s[i],target_weather_exchange_particle_data.leave_particle_args_start_time_s[i],target_weather_exchange_particle_data.leave_particle_args_duration_s[i])
          end
        
        end
      end
      
    end
    
end


function battleview:startWeather(weather_id)
    printBattleView("startWeather:"..weather_id)

    self.scene_relation_mono:PreExchangeWeather()
    
    local target_weather_data = dict_weather.getDataByID(weather_id)
    
    --目标天气
    for k,v in pairs(target_weather_data.weather_echange_light_id) do
      
      local target_weather_exchange_light_data = dict_weather_exchange_light.getDataByID(v)
      
      local args_num = table.nums(target_weather_exchange_light_data.light_args_name_s)
      
      if args_num > 0 then
        for i=1,args_num do
          local str = System.String.New(target_weather_exchange_light_data.light_args_name_s[i])
          
          if str:Contains('_color') then
            local tmp_color = Color(target_weather_exchange_light_data.light_args_target_value_s[i]/255,target_weather_exchange_light_data.light_args_target_value_s[i+1]/255,target_weather_exchange_light_data.light_args_target_value_s[i+2]/255)
            self.scene_relation_mono:StartExchangeColor(target_weather_exchange_light_data.light_args_name_s[i],tmp_color,target_weather_exchange_light_data.light_args_start_time_s[i],target_weather_exchange_light_data.light_args_duration_s[i])
            i = i + 2
          else
            self.scene_relation_mono:StartExchangeLight(target_weather_exchange_light_data.light_type,target_weather_exchange_light_data.light_args_name_s[i],target_weather_exchange_light_data.light_args_target_value_s[i],target_weather_exchange_light_data.light_args_start_time_s[i],target_weather_exchange_light_data.light_args_duration_s[i])
          end
        end
      end
      
    end
    
    --雾效
    
    
    --切入特效
    for k,v in pairs(target_weather_data.weather_exchange_particle_id_s) do
      
      local target_weather_exchange_particle_data = dict_weather_exchange_particle.getDataByID(v)
      
      local effect_data = dict_effect_resource.getDataByID(target_weather_exchange_particle_data.effect_id)
      
      local args_num = table.nums(target_weather_exchange_particle_data.enter_particle_args_name_s)
      local paricle_exchange = self.scene_relation_mono:StartLoadResource(effect_data.url,target_weather_exchange_particle_data.enter_particle_show_time)
                
      if args_num > 0 then
        for i=1,args_num do
          paricle_exchange:StartEnterWeatherParticle(target_weather_exchange_particle_data.enter_particle_args_name_s[i],target_weather_exchange_particle_data.enter_particle_args_start_value_s[i],target_weather_exchange_particle_data.enter_particle_args_target_value_s[i],target_weather_exchange_particle_data.enter_particle_args_start_time_s[i],target_weather_exchange_particle_data.enter_particle_args_duration_s[i])
        end
      end
    
    end
end




return battleview