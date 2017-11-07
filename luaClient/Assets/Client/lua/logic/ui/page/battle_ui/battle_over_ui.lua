


local class = require("common/middleclass")
require "logic/ui/uipart/lua_mono_behaviour"

battle_over_ui = class("battle_over_ui",lua_mono_behaviour)



function battle_over_ui:initData()
  self.win_desc_text = self.transform:FindChild("Image (6)/win_lose_state"):GetComponent("UnityEngine.UI.Text")
  
end

function battle_over_ui:showBattleOver(win_lose,desc)
  if win_lose == "win" then
    self.win_desc_text.text = dict_language["battle_win"]
  else
    self.win_desc_text.text = dict_language["battle_lose"]
  end
  
end



return battle_over_ui