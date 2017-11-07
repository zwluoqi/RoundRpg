require "logic/common/basemodel"

local class = require("common/middleclass")

eactorskillmanager = class("eactorskillmanager",basemodel)


function eactorskillmanager:initialize()
        
        self.e_actor = e_actor

        --<int,es>
        self.skills = {}
        
        self.skill_id_sort = {}

        self.current_skill = nil

        self.clicked_skill_id = -1
        
        self.utl_skill = nil
        
        self.request_utl = false
end

function eactorskillmanager:init(e_actor)
        
  for k,v in pairs( e_actor.input_actor.hero_skills) do
  
      local e_actor_skill = (require ("logic/battle/engine/model/skill/eactorskill"..v.logic_type))();
      e_actor_skill:initData(e_actor,v);

      self.skills[e_actor_skill.model_data.id] = e_actor_skill;
      self:addSkillIDToSortTable(e_actor_skill.model_data.id)
  end
  
  self.clicked_skill_id = -1;
  
end

function eactorskillmanager:addSkillIDToSortTable(skill_id)
  
    for index,v in pairs(self.skill_id_sort) do
        if self.skills[skill_id].model_data.weight > self.skills[v].model_data.weight  then
          table.insert(self.skill_id_sort,index,skill_id)
          return
        end
        
    end 
  
    table.insert(self.skill_id_sort,skill_id)
    
end




function eactorskillmanager:getRequestSkillId()
    if (self.clickedSkillId > 0) 
    then
        return self.clickedSkillId;
    end
    
    
    return -1;
end
        


function eactorskillmanager:isSkilling()
    for k,v in pairs(self.skills) do
      if v:isSkilling() then
        return true
      end
    end
    return false
end





function eactorskillmanager:startOnceRound()
  --self.request_utl = false
  for k,v in pairs(self.skills) do
    v:startOnceRound()
  end
  
end

function eactorskillmanager:endOnceRound()
  --self.request_utl = false

  for k,v in pairs(self.skills) do
    v:endOnceRound()
  end
  
end

function eactorskillmanager:startOnceOrder()
  --self.request_utl = false
  for k,v in pairs(self.skills) do
    v:startOnceOrder()
  end
  
end

function eactorskillmanager:endOnceOrder()
  --self.request_utl = false

  for k,v in pairs(self.skills) do
    v:endOnceOrder()
  end
  
end


function eactorskillmanager:clear()
  self.request_utl = false

  for k,v in pairs(self.skills) do
    v:clear()
  end
  
end

return eactorskillmanager