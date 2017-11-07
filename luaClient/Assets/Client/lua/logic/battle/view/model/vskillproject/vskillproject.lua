

require "logic/common/basemodel"

local class = require("common/middleclass")

vskillproject = class("vskillproject",basemodel)



function vskillproject:initData(source_pos,target_pos,project_uri,duration)
  self.source_pos = source_pos
  self.target_pos = target_pos
  self.project_uri = project_uri
  self.duration = duration
  self.unity_game_obejct = nil
  self.guid = getGUID()
end


function vskillproject:startProject()
  self.unity_game_obejct = GameObjectPoolManager.Instance:GetGameObjectDirect(self.project_uri)
  
  self.unity_game_obejct.transform:SetParent(battlemanager.instance().battle_view.battle_particle_pref_game_object.transform)
  unitytools.resetPos(self.unity_game_obejct)
  self.unity_game_obejct.transform.localPosition = self.source_pos 

  self.unity_game_obejct.transform:DOLocalMove(self.target_pos ,self.duration,false)
  
  
  self.delete_time_event =  battlemanager.instance().time_event_manager:createEvent(function()
      GameObjectPoolManager.Instance:Unspawn(self.unity_game_obejct)
      self.unity_game_obejct = nil
      battlemanager.instance().battle_view:removeProject(self.guid)

    end,"startProject",self.duration)
  
end

function vskillproject:clear()
  if self.unity_game_obejct ~= nil then
      GameObjectPoolManager.Instance:Unspawn(self.unity_game_obejct,self.duration)
  end
  self.unity_game_obejct = nil
  
  battlemanager.instance().time_event_manager:delete(self.delete_time_event)
  self.delete_time_event = nil
end








return vskillproject