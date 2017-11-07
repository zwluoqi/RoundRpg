require "logic/battle/engine/util/poolobj"

poolmanager = class("poolmanager")


function poolmanager:initialize()

end


function poolmanager:newPool(lua_obj_path,count)
  if self.pool_obj_s[lua_obj_path] == nil then
    self.pool_obj_s[lua_obj_path] = poolobj(lua_obj_path)
  end
  self.pool_obj_s[lua_obj_path]:newObj(count)
end

function poolmanager:spwan(lua_obj_path)
  if self.pool_obj_s[lua_obj_path] == nil then
      self:newPool(lua_obj_path,1)  
  end
  return self.pool_obj_s[lua_obj_path]:spwan()
end

function poolmanager:unSpwan(obj)
  local lua_obj_path = obj.lua_obj_path
  if self.pool_obj_s[lua_obj_path] == nil then
      printError("poolmanager:unSpwan")
      return
  end
  self.pool_obj_s[lua_obj_path]:unSpwan(obj)
end

function poolmanager:log()
  
  for k,v in pairs(self.pool_obj_s) do
    v:log()
  end
  
  
end


function poolmanager:ready()
  self.pool_obj_s = {}
end

function poolmanager:clear()
  if self.pool_obj_s  == nil then
    return 
  end
  
  for k,v in pairs(self.pool_obj_s) do
    v:clear()
  end
  self.pool_obj_s = nil
end




return poolmanager
