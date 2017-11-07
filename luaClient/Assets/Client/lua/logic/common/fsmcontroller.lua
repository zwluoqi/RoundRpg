

fsmcontroller = class("fsmcontroller")


function fsmcontroller:initialize()
    self.state_dict = {}
    printLog("fsmcontroller:initialize")
end

function fsmcontroller:addState(state_type,fsm_state)
    self.state_dict[state_type]  = fsm_state
end


function fsmcontroller:getState(state_Type)
    return self.state_dict[state_Type]
end

function fsmcontroller:tick()
    if self.currentState ~= nil then
        self.currentState:tick()
    end
end

function fsmcontroller:switchState(new_state_type)
    if self.currentState ~= nil then
        self.currentState:leave()
    end
    self.currentState = self.state_dict[new_state_type]
    printLog("switchState:"..self.state_dict[new_state_type]:getStateType())

    self.currentState:enter()
end

return fsmcontroller
