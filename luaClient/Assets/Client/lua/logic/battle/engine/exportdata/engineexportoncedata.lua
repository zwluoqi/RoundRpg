engineexportoncedata = class("engineexportoncedata")




function engineexportoncedata:initialize()
  --list
  self.attack_values = {}
  self.dead_list = {}
  self.skill = nil
  self.release = nil
  self.buff = nil
  self.source = nil
  self.target = nil
  
end



function engineexportoncedata:readyData()
  
end


function engineexportoncedata:clearData()
  
end









function engineexportoncedata.getOnceDataFromSkill(e_actor_skill)

    


end



function engineexportoncedata.createOnceDataBySkill(e_skill,target)
    local once_data = engineexportoncedata()
    once_data.skill = e_skill
    once_data.release = nil
    once_data.buff = nil
    once_data.source = e_skill.e_actor
    once_data.target  = target
    return once_data

end

function engineexportoncedata.createOnceDataByRelease(e_release,target)
    local once_data = engineexportoncedata()
    once_data.skill = e_release.e_actor_skill
    once_data.release = e_release
    once_data.buff = nil
    once_data.source = e_release.e_actor_skill.e_actor
    once_data.target  = target
    return once_data
end


function engineexportoncedata.createOnceDataByBuff(e_buff,target)
    local once_data = engineexportoncedata()
    once_data.skill = nil
    once_data.release = nil
    once_data.buff = e_buff
    once_data.source = e_buff.e_source
    once_data.target  = target
    return once_data
end

function engineexportoncedata.pushBattleOnceData(once_data)

    battlemanager.instance().battle_engine.export_engine_event_util:exportBattleOnceData(once_data)


end



return engineexportoncedata