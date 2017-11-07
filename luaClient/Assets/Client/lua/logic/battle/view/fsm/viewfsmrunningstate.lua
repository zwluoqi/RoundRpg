
require("logic/common/fsmstate")

local class = require("common/middleclass")

viewfsmrunningstate = class("viewfsmrunningstate",fsmstate)

function viewfsmrunningstate:initialize()
      fsmstate.initialize(self)
        self.state_type = BattleViewState.RUNNING

    printLog("viewfsmrunningstate:initialize"..self.state_type)


end


return viewfsmrunningstate