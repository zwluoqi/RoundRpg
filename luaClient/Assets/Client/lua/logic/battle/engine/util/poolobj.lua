poolobj = class("pool")

local pool_obj_guid = 9527

function poolobj:initialize(lua_obj_path)
  self.lua_obj_path = lua_obj_path
  self.active_objs = {}
  self.un_active_objs_list = {}--list
  self.un_active_objs_list_num = 0
  self.active_objs_num = 0
end


function poolobj:newObj(count)
  for i=1,count do
    local obj = (require (self.lua_obj_path))()
    obj.lua_obj_path = self.lua_obj_path
    obj.guid = pool_obj_guid
    pool_obj_guid = pool_obj_guid+1

    table.insert(self.un_active_objs_list,obj)
    self.un_active_objs_list_num = self.un_active_objs_list_num+1
  end
end

function poolobj:spwan()
    if(self.un_active_objs_list_num > 0) then
      local obj = self.un_active_objs_list[self.un_active_objs_list_num]
      table.remove(self.un_active_objs_list,self.un_active_objs_list_num)
      
      self.active_objs[obj.guid] = obj
      self.un_active_objs_list_num = self.un_active_objs_list_num - 1
      self.active_objs_num = self.active_objs_num + 1
      obj:readyData()
      return obj
    else
      local obj = (require (self.lua_obj_path))()
      obj.guid = pool_obj_guid
      obj.lua_obj_path = self.lua_obj_path
      pool_obj_guid = pool_obj_guid+1

      self.active_objs[obj.guid] = obj
      self.un_active_objs_list_num = self.un_active_objs_list_num - 1
      self.active_objs_num = self.active_objs_num + 1
      obj:readyData()
      return obj
    end    
end

function poolobj:unSpwan(obj)
    if self.active_objs[obj.guid] == nil then
      printError("error unSpwan obj:"..obj.lua_obj_path)
      return
    end
    
    table.insert(self.un_active_objs_list,obj)
    self.active_objs[obj.guid] = nil
    self.active_objs_num = self.active_objs_num -1
    self.un_active_objs_list_num = self.un_active_objs_list_num + 1

    obj:clearData()
end


function poolobj:log()
  printBattleEngine("path:"..self.lua_obj_path.." active num:"..self.active_objs_num.." self.un_active_objs_list_num"..self.un_active_objs_list_num)
end

function poolobj:clear()
  for k,v in pairs( self.active_objs) do
    v:clearData()
  end
  self.active_objs = nil
  self.un_active_objs_list = nil
end



return poolobj
