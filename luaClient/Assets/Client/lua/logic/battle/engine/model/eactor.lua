


require "logic/common/basemodel"
require "logic/battle/engine/model/eactorskillmanager"
require "logic/battle/engine/model/eactorattributemanager"

local class = require("common/middleclass")

eactor = class("eactor",basemodel)


function eactor:initialize(input_actor)
  self.battle_manager = battlemanager.instance()
  self.battle_engine = self.battle_manager.battle_engine
  
  self.guid = input_actor.guid
  self.input_actor = input_actor
  self.is_dead = false
  self.is_auto = false
  self.enemy_logic_camp_types = battletools.reverse_camp(self.input_actor.logic_camp_type)
  
  self.e_actor_skill_manager = eactorskillmanager()
  self.e_actor_skill_manager :init(self)
  
  self.e_actor_attribute_manager = eactorattributemanager()
  self.e_actor_attribute_manager :init(self)

end

function eactor:setVActor(t_game_object)
    self.transform = t_game_object.transform
end

function eactor:getLocalPosition()
    return self.transform.localPosition
end

function eactor:getForward()
    return self.transform.forward
end

function eactor:setLocalPosition(pos)
  self.transform.localPosition = pos
end

function eactor:setForward(vec)
    self.transform.forward = vec
end


function eactor:clear()
  self.e_actor_skill_manager:clear()
  
end

function eactor:startOnceOrder()
  --TODO
  self.e_actor_skill_manager:startOnceOrder()


end

function eactor:endOnceOrder()
  self.e_actor_skill_manager:endOnceOrder()
  
end

function eactor:startOnceRound()
  --TODO
  self.e_actor_skill_manager:startOnceRound()


end

function eactor:endOnceRound()
  self.e_actor_skill_manager:endOnceRound()
  
end



function eactor:tick()

end





function eactor:preStartSkill(ready_skill)
  if self.e_actor_skill_manager.current_skill ~= nil then
    if self.e_actor_skill_manager.current_skill:isSkilling() then
        self.e_actor_skill_manager.current_skill:interruptSkill()
    end
  end
  
  
  printActor("guid:"..self.guid.." start skill:"..ready_skill.model_data.id)
  self.e_actor_skill_manager.current_skill = ready_skill
  self.e_actor_skill_manager.current_skill:preStartSkill()
end


-- <summary>
-- 所有技能,符合施法条件返回true,不符合返回false,
-- </summary>
-- <param name="ready_skill"></param>
-- <returns></returns>
function eactor:checkAnySkillReady(dis)
    local ready_skill = nil
    local is_ready = false
    
    is_ready,ready_skill = self:checkSkillReady(dis,SkillKind)

    return is_ready,ready_skill
end

    
function eactor:checkNormalSkillReady(dis)
    local ready_skill = nil
    local is_ready = false
    
    
    is_ready,ready_skill = self:checkSkillReady(dis,
      {
      normal = "normal",
    })

    return is_ready,ready_skill
end

function eactor:checkSkillOrUtlSkillReady(dis)
    local ready_skill = nil
    local is_ready = false
    
    is_ready,ready_skill = self:checkSkillReady(dis,{
      skill = "skill",
      utl = "utl",
    })

    return is_ready,ready_skill
end

function eactor:checkSkillSkillReady(dis)
    local ready_skill = nil
    local is_ready = false
    
    is_ready,ready_skill = self:checkSkillReady(dis,{
      skill = "skill",
    })

    return is_ready,ready_skill
end

function eactor:checkUtlSkillReady(dis)
    local ready_skill = nil
    local is_ready = false
    
    is_ready,ready_skill = self:checkSkillReady(dis,{
      utl = "utl",
    })

    return is_ready,ready_skill
end
        

-- <summary>
-- 按序查询合适类型的技能是否满足条件
-- </summary>
-- <param name="ready_skill"></param>
-- <returns></returns>
function eactor:checkSkillReady(dis,has_skill_kinds)
  local is_ready = false
  for index,skill_id in pairs(self.e_actor_skill_manager.skill_id_sort) do
    
    local skill = self.e_actor_skill_manager.skills[skill_id];
    if self:isSkillReady(skill) and has_skill_kinds[skill.model_data.kind] then
      --条件满足
      
    return true,skill;
      
    end
  end
  
  return false,nil;
end
        
function eactor:isSkillReady(ready_skill)
  if self.e_actor_attribute_manager.anger_val < self.e_actor_attribute_manager.max_anger_val and ready_skill.model_data.kind == SkillKind.utl then
    return false
  end
  return ready_skill:isReady()
end



function eactor:pathToPosition()

end
local rotation_radiu_speed = 12

function eactor:simpleMoveToPosition(target_position,move_speed)
  
  local distance = MathUtil.DistanceNoY(target_position,self:getLocalPosition())
  if distance*distance <= MathUtil.zeroDisSqrt then
    return true
  end
  
  
  local target_dir = target_position - self:getLocalPosition()
  target_dir.y = 0
  local forward_length = Mathf.Sqrt(target_dir.x*target_dir.x+target_dir.z*target_dir.z)
  target_dir = target_dir/ forward_length
  local move_dis = move_speed*self.battle_engine.delta_time
  local cur_dir = self:getForward()
  local turn_dir = Vector3.RotateTowards(cur_dir,target_dir,rotation_radiu_speed*self.battle_engine.delta_time,0)
  self:setForward(turn_dir)
  
  local speed_dir = Vector3.Project(turn_dir,target_dir)*move_dis
  
  if speed_dir.magnitude > distance then
    target_position = self:getLocalPosition()+target_dir*distance
    self:setLocalPosition(target_position)
    return false

  else
    target_position = self:getLocalPosition()+speed_dir
    self:setLocalPosition(target_position)
    return true
  end
  
end

function eactor:dead()
  printActor("e_actor dead:"..self.guid)
  self.is_dead = true
  self:clear()
end


function eactor:doHpChanged(export_data,once_data)
  export_data.value = math.round(export_data.value)
  self.e_actor_attribute_manager.hp = self.e_actor_attribute_manager.hp - export_data.value
  if self.e_actor_attribute_manager.hp <=0 then
    self:dead()
    table.insert(once_data.dead_list,self.guid)
  end
end

function eactor:doAGChanged(export_data)
  export_data.value = math.round(export_data.value)

      self.e_actor_attribute_manager.anger_val = self.e_actor_attribute_manager.anger_val + export_data.value
      if self.e_actor_attribute_manager.anger_val >= self.e_actor_attribute_manager.max_anger_val then
        self.e_actor_attribute_manager.anger_val = self.e_actor_attribute_manager.max_anger_val
      elseif self.e_actor_attribute_manager.anger_val <= 0 then
        self.e_actor_attribute_manager.anger_val = 0
      end
end

function eactor:doExportData(export_data,once_data)
  --TODO
    printActor("export_data: source guid:"..export_data.source.guid.."target guid:"..export_data.target.guid.." export_class:"..export_data.export_class.." value:"..export_data.value)

  if export_data.export_class == ExportValueClass.DEMAGE then
    self:doHpChanged(export_data,once_data)
  elseif export_data.export_class == ExportValueClass.ANGER then
    self:doAGChanged(export_data)
  end

end



function eactor.sortEActorByAtkSpeed(e_actor,sort_e_actor)
    --速度大到小排序
    
    for index,index_e_actor in pairs(sort_e_actor) do
      if e_actor.e_actor_attribute_manager.attack_speed_atr:getValue() > index_e_actor.e_actor_attribute_manager.attack_speed_atr:getValue() then
        table.insert(sort_e_actor,index,e_actor)
        return
      end
    end
    
    table.insert(sort_e_actor,e_actor)

end




