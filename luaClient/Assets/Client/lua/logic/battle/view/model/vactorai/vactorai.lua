require "logic/common/basemodel"

local class = require("common/middleclass")

vactorai = class("vactorai",basemodel)


function vactorai:initialize()
  
end


function vactorai:loadResource()


end

function vactorai:unLoadResource()
  

end

function vactorai:hide()
  self.is_show = false

end

function vactorai:show()
  self.is_show = true

end