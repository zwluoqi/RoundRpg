exportvalue = class("exportvalue")


function exportvalue:initialize()
  self.source = nil
  self.target = nil
  self.value = 0
  self.export_class = nil
  self.export_sub_class = nil
  
end

function exportvalue:init(source_actor,target_actor,val,export_class,export_sub_class)
  self.source = source_actor
  self.target = target_actor
  self.value = val
  self.export_class = export_class
  self.export_sub_class = export_sub_class
  
end


function exportvalue:readyData()
    self.source = nil
  self.target = nil
  self.value = 0
  self.export_class = nil
  self.export_sub_class = nil
end


function exportvalue:clearData()
  self.source = nil
  self.target = nil
  self.value = 0
  self.export_class = nil
  self.export_sub_class = nil
end

return exportvalue