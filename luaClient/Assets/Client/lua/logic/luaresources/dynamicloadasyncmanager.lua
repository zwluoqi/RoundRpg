
require "logic/luaresources/dynamicloadasyncer"


dynamicloadasyncmanager = class("dynamicloadasyncmanager")



function dynamicloadasyncmanager.loadAsync(resource_path,load_done_fun)
  local dynamic_load_asyncer = dynamicloadasyncer()
  dynamic_load_asyncer:init(resource_path,load_done_fun)
  dynamic_load_asyncer:startLoadAsync()
  return dynamic_load_asyncer
end

function dynamicloadasyncmanager.deleteAsyncer(load_asyncer)
  load_asyncer:cancelLoadAsync()
end



return dynamicloadasyncmanager