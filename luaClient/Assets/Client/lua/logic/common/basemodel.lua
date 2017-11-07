


basemodel = class("basemodel")
function basemodel:initialize()
    
    self.name = "basemodel"
end


function basemodel:destroy()
          --printLog("basemodel:destroyData:"..self.name)

end

function basemodel:tick()
      --printLog("basemodel:tick:"..self.name)

end



function basemodel:ready()
        --printLog("basemodel:ready:"..self.name)

end

function basemodel:clear()
          --printLog("basemodel:clear:"..self.name)

end
return basemodel