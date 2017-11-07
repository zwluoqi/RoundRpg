lua_mono_behaviour = class("ui_part")







function lua_mono_behaviour:initialize(game_object)
  local tempBehaviour = game_object:GetComponent(typeof(UILuaBehaviour))
  if tempBehaviour == nil then
    tempBehaviour = game_object:AddComponent(typeof(UILuaBehaviour))
  end
  tempBehaviour:Init(self)
end



return lua_mono_behaviour



