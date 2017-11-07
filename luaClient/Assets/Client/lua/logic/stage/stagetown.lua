

local class = require("common/middleclass")

require "logic/stage/stagebase"


stagetown = class("stagetown",stagebase)



function stagetown:initialize()
    self.name = "stagetown"
end

function stagetown:preLoad()
    stagemanager.instance().local_resource_manager:addRes(LocalResourceType.Scene,"space")
end

function stagetown:onStart()
      ui_manager:getPageManager():openPage("main_ui")

end


return stagetown