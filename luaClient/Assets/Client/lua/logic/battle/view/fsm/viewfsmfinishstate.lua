require("logic/common/fsmstate")

local class = require("common/middleclass")

viewfsmfinishstate = class("viewfsmfinishstate",fsmstate)

function viewfsmfinishstate:initialize()
      fsmstate.initialize(self)
        self.state_type = BattleViewState.FINISH

    printLog("viewfsmfinishstate:initialize"..self.state_type)


end

return viewfsmfinishstate