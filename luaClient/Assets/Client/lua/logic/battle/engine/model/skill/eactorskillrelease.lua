require "logic/common/basemodel"

local class = require("common/middleclass")

eactorskillrelease = class("eactorskillrelease",basemodel)


function eactorskillrelease:initialize(e_actor_skill,data)
  self.e_actor_skill = e_actor_skill
  self.model_data = data
  self.relaseing_time_event = nil
  
  
  self.start_release_fun = function()
    self:startRelease()
  end
  self.start_project_fun = function()
    self:startProject()
  end
  
end


function eactorskillrelease:startRelease()
  printSkill("startRelease.."..self.model_data.id)
  self.e_actor_skill:releaseOnce(self)
end

function eactorskillrelease:startProject()
    printSkill("startPoject.."..self.model_data.id)
    self.e_actor_skill:releasePorject(self)
end



function eactorskillrelease:ready()
  
end


function eactorskillrelease:clear()
  
end


function eactorskillrelease:destroy()
  
end

function eactorskillrelease:tick()

end




return eactorskillrelease