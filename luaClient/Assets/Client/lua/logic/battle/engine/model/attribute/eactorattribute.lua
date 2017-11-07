

eactorattribute = class("eactorattribute")

function eactorattribute:initialize()
  
  self.base_val = 1
  self.add_val = 0
  self.add_factor_val = 0
  
  self.current_val = 0;
  self.changed = false
  
end


function eactorattribute:init(base_val)
  self.base_val = base_val
  self.changed = true
end


function eactorattribute:getValue()
    if self.changed then
      self.current_val = self.base_val*(1+self.add_factor_val)+self.add_val
    end
    
    return self.current_val
end



return eactorattribute