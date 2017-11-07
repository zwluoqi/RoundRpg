require("logic/common/fsmstate")

local class = require("common/middleclass")


viewfsmloadingstate = class("viewfsmloadingstate",fsmstate)

function viewfsmloadingstate:initialize()
      fsmstate.initialize(self)
        self.state_type = BattleViewState.LOADING

    printLog("viewfsmloadingstate:initialize"..self.state_type)


end


return viewfsmloadingstate