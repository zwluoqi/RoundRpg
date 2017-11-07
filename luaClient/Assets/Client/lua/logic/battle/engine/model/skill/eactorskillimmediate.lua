require "logic/battle/engine/model/skill/eactorskill"

local class = require("common/middleclass")

eactorskillimmediate = class("eactorskillimmediate",eactorskill)


function eactorskillimmediate:releasePorject(e_actor_skill_release)
    printSkill("eactorskillimmediate:projectOnce.."..e_actor_skill_release.model_data.id)
    
    self.e_actor.battle_engine.export_engine_event_util:exportSkillReleaseProject(e_actor_skill_release,self.target_actor)

    
end



function eactorskillimmediate:releaseOnce(e_actor_skill_release)
  
  printSkill("eactorskillimmediate:releaseOnce.."..e_actor_skill_release.model_data.id)
  --self.targets = self.e_actor.battle_engine.battle_engine_select_util:getTargets(self.e_actor,self.model_data.select_target_obj_type,self.model_data.select_target_strategy_type)
  self:exportTargets(e_actor_skill_release,self.targets)
  
end


return eactorskillimmediate