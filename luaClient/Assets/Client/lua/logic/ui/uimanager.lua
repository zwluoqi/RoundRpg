

require "logic/ui/pagemanager"
require "logic/ui/boxmanager"
require "logic/ui/popmanager"
require "logic/ui/uguieventlistener"
require "logic/ui/uitools"

uimanager = class("uimanager")

local _instatnce = nil
function uimanager.instance()
  return _instatnce
end

function uimanager:initialize()
    _instatnce = self
    self.unity_game_object = UGameObject.Find("ui_layer")
    
    if self.unity_game_object == nil then
      local obj = ResourceManager.Instance:LoadResourceBlock("ui/ui_layer")
      self.unity_game_object = UGameObject.Instantiate(obj)
    end
  
      
    UGameObject.DontDestroyOnLoad(self.unity_game_object)
end



function uimanager:getPageManager()
  if self.page_manager == nil then
    self.page_manager = pagemanager(self,self:createLayerRoot("pagemanager",100))
  end
  return self.page_manager
end

function uimanager:getBoxManager()
  if self.box_manager == nil then
    self.box_manager = boxmanager(self,self:createLayerRoot("boxmanager",1000))
  end
  return self.box_manager
end

function uimanager:getPopManager()
  if self.pop_manager == nil then
    self.pop_manager = popmanager(self,self:createLayerRoot("boxmanager",5000))
  end
  return self.pop_manager
end

function uimanager:autoSetCanvasSortOrder(tmp_go,sort_order)
    local tmp_canvas = tmp_go:GetComponent(typeof(UnityEngine.Canvas))
    if tmp_canvas == nil then
      tmp_canvas = tmp_go:AddComponent(typeof(UnityEngine.Canvas))
    end
  
    tmp_canvas.overrideSorting = true
    tmp_canvas.sortingOrder = sort_order
    
    local tmp_ray = tmp_go:GetComponent(typeof(UnityEngine.UI.GraphicRaycaster))
    if tmp_ray == nil then
      tmp_ray = tmp_go:AddComponent(typeof(UnityEngine.UI.GraphicRaycaster))
    end
end


function uimanager:createLayerRoot(go_name, sort_order)
    local tmp_go = UGameObject(go_name)

    local rt_transform = tmp_go:AddComponent(typeof(UnityEngine.RectTransform))

    rt_transform:SetParent(self.unity_game_object.transform)
    unitytools.resetPos(tmp_go)

    self:changeChildLayer(tmp_go, LayerMask.NameToLayer("UI"))

    rt_transform.anchorMin = Vector2.zero
    rt_transform.anchorMax = Vector2.one

    rt_transform.offsetMax = Vector2.zero
    rt_transform.offsetMin = Vector2.zero

    local tmp_canvas = tmp_go:AddComponent(typeof(UnityEngine.Canvas))
    tmp_canvas.overrideSorting = true
    tmp_canvas.sortingOrder = sort_order
    tmp_go:AddComponent(typeof(UnityEngine.UI.GraphicRaycaster))

    return rt_transform
end


function uimanager:changeChildLayer(go, layer)
    go.layer = layer
    local tmp_tr = go.transform
    for i = 0, tmp_tr.childCount - 1 do
        local child = tmp_tr:GetChild(i)
        child.gameObject.layer = layer
        self:changeChildLayer(child, layer)
    end
end


return uimanager

