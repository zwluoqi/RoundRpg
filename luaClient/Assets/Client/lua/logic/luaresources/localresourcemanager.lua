require "logic/luaresources/localresourceitem"

localresourcemanager = class("localresourcemanager")

function localresourcemanager:initialize()
    self.load_res_items = {}
end


function localresourcemanager:addRes(Local_resource_type,path,callback)
    self.load_res_items[path] = localresourceitem(Local_resource_type,path,callback,  
      
      function(localresourceitem)
          self:removeLoadingItem(localresourceitem)
          if localresourceitem.local_resource_callback ~= nil then
              self.localresourceitem.local_resource_callback()
          end
          
          printWarning("itemLoadFinish remaind count:"..table.nums(self.load_res_items))
          if table.nums(self.load_res_items) > 0 then
            return
          end
          
          self:finishLoad()
      end
      )
end




function localresourcemanager:startLoad(callback)
    self.load_finish_callback = callback
    
    if table.nums(self.load_res_items) <= 0 then
      self:finishLoad()
      return
    end
    
    local tmpResItems = {}
    for k,v in pairs(self.load_res_items) do
        tmpResItems[k] = v
    end
    
    for k,v in pairs(tmpResItems) do
        v:startLoad()
    end

end

function localresourcemanager:removeLoadingItem(localresourceitem)
    self.load_res_items[localresourceitem.local_resource_path] = nil
end




function localresourcemanager:finishLoad()
  if self.load_finish_callback ~= nil then
    self.load_finish_callback()
  end
end



return localresourcemanager