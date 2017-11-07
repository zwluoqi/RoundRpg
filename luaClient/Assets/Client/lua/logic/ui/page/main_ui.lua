require "logic/ui/page/page_ui"
local totem_h1_pb = require "protol/totem_h1_pb"

local class = require("common/middleclass")
main_ui = class("main_ui",page_ui)



function main_ui:doOpen()
  page_ui.doOpen(self)
  
  self.start_btn1 = self.transform:FindChild("start_btn1").gameObject


  self.start_btn_fun1 = function(go)
      uimanager.instance():getPageManager():closeAllPage()
      stagefuben.requestCopy(90001)
      stagemanager.instance():switchStage(StageType.FUBEN)
  end

  uguieventlistener.addUGUIOnClickListener(UGUIEventListener.Get(self.start_btn1), self.start_btn_fun1)
  
  self.start_btn2 = self.transform:FindChild("start_btn2").gameObject


  self.start_btn_fun2 = function(go)
      uimanager.instance():getPageManager():closeAllPage()
      stagefuben.requestCopy(90003)
      stagemanager.instance():switchStage(StageType.FUBEN)
  end

  uguieventlistener.addUGUIOnClickListener(UGUIEventListener.Get(self.start_btn2), self.start_btn_fun2)
  
end



function main_ui:doClose()
  uguieventlistener.removeUGUIOnClickListener(UGUIEventListener.Get(self.start_btn1), self.start_btn_fun1)
  uguieventlistener.removeUGUIOnClickListener(UGUIEventListener.Get(self.start_btn2), self.start_btn_fun2)
end


return main_ui
