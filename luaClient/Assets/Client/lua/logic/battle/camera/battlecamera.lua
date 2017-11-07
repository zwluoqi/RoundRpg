require "logic/common/basemodel"

local class = require("common/middleclass")

battlecamera = class("battlecamera",basemodel)


function battlecamera:initialize(battle_manager)
    self.battle_manager = battle_manager
          self.name = "battlecamera"

      printLog("battlecamera:initialize()")
end



return battlecamera