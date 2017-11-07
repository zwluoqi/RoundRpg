require "logic/ui/page/page_ui"

local class = require("common/middleclass")
hero_desc_pop_ui = class("hero_desc_pop_ui",page_ui)


function hero_desc_pop_ui:initData()
  self.page_type = PageType.COVER
end


function hero_desc_pop_ui:doOpen()
    self.btn1 = self.transform:FindChild("content/btnClose").gameObject
    self.btn_center = self.transform:FindChild("content/btnCenter").gameObject
    self.btnLeft = self.transform:FindChild("content/btnLeft").gameObject
    
self.btnLeft_fun = function(go)
  self.page_manager:openPage("main_ui")
end

self.btn1_fun = function(go)
  self.page_manager.current_page:closeSelf()
end
self.btn_center_fun = function(go)
  self.page_manager:openPage("battle_ui")
end
  
  uguieventlistener.addUGUIOnClickListener(UGUIEventListener.Get(self.btn1), self.btn1_fun)
  uguieventlistener.addUGUIOnClickListener(UGUIEventListener.Get(self.btn_center), self.btn_center_fun)
  uguieventlistener.addUGUIOnClickListener(UGUIEventListener.Get(self.btnLeft), self.btnLeft_fun)
end


function hero_desc_pop_ui:doClose()
    uguieventlistener.removeUGUIOnClickListener(UGUIEventListener.Get(self.btn1), self.btn1_fun)

      uguieventlistener.removeUGUIOnClickListener(UGUIEventListener.Get(self.btn_center), self.btn_center_fun)
    uguieventlistener.removeUGUIOnClickListener(UGUIEventListener.Get(self.btnLeft), self.btnLeft_fun)
  
end


return hero_desc_pop_ui
