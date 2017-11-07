fsmstate = class("fsmstate")





function fsmstate:initialize()
    self.state_type = "default state"
      printLog("fsmstate:initialize:"..self.state_type)

end

function fsmstate:getStateType()
    return self.state_type
end


function fsmstate:tick()
    --printLog(self.state_type.." tick")
end


function fsmstate:enter()
    printLog(" enter "..self.state_type)
end


function fsmstate:leave()
    printLog(" leave "..self.state_type)
end

return fsmstate