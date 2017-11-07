dynamicloadasyncer = class("dynamicloadasyncer")





function dynamicloadasyncer:init(resource_path,load_done_fun)
  self.load_done_fun = load_done_fun
  self.resource_path = resource_path
end


function dynamicloadasyncer:startLoadAsync()
  self.is_cancle = false
  GameObjectPoolManager.Instance:GetGameObjectAsync(self.resource_path,
    function(asset)
        if self.is_cancle then
          GameObjectPoolManager.Instance:Unspawn(asset)
        else
          if self.load_done_fun ~= nil then
            self.load_done_fun(asset)
          end
        end
    end
    )
end

function dynamicloadasyncer:cancelLoadAsync()
  self.is_cancle = true
end



return dynamicloadasyncer