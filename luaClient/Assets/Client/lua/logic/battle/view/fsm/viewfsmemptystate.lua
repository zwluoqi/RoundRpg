require("logic/common/fsmstate")

local class = require("common/middleclass")


viewfsmemptystate = class("viewfsmemptystate",fsmstate)

function viewfsmemptystate:initialize()
    fsmstate.initialize(self)
    self.state_type = BattleViewState.EMPTY

  printLog("viewfsmemptystate:initialize"..self.state_type)

end


return viewfsmnonestate