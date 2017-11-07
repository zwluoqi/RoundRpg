



localresourceitem = class("localresourceitem")
function localresourceitem:initialize(local_resourcetype, local_resourcepath,local_resourcecallback,sys_call_back)
    self.Local_resource_type = local_resourcetype
    self.local_resource_path = local_resourcepath
    self.local_resource_callback = local_resourcecallback
    self.sys_callback = sys_call_back
end

function localresourceitem:startLoad(itemCallback)
  printWarning("localresourceitem:startLoad:"..self.local_resource_path)

  if self.Local_resource_type == LocalResourceType.Prefab then
    ResourceManager.Instance:LoadResourceAsync(self.local_resource_path,
        function(path,asset)
          self:loadFinish()
        end
    )
  elseif self.Local_resource_type == LocalResourceType.Scene then
    SceneLoadManager.Instance:LoadAsync(self.local_resource_path,        
        function(path,asset)
          self:loadFinish()
        end
    )
  else
    printError("error resource load:"..self.local_resource_path)
    self:loadFinish()
  end
end

function localresourceitem:loadFinish()
    printWarning("localresourceitem:loadFinish:"..self.local_resource_path)
      if self.sys_callback ~= nil then
        self.sys_callback(self)
    end
end



