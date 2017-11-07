require "logic/common/basemodel"
require "logic/battle/engine/model/attribute/eactorattribute"

local class = require("common/middleclass")

eactorattributemanager = class("eactorattributemanager",basemodel)


function eactorattributemanager:initialize()
  self.attack_speed_atr = eactorattribute()

  self.atk_atr = eactorattribute()
  self.def_atr = eactorattribute()
  self.max_hp = eactorattribute()
end

function eactorattributemanager:init(e_actor)
    self.anger_val = 0
    self.max_anger_val = 100
    self.hp = e_actor.input_actor.hp
  
    self.attack_speed_atr:init(e_actor.input_actor.attack_speed)
    self.atk_atr:init(e_actor.input_actor.attack)
    self.def_atr:init(e_actor.input_actor.phy_def)
    self.max_hp:init(self.hp)
end



return eactorattributemanager