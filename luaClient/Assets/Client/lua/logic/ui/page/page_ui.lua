require "logic/ui/uipart/lua_mono_behaviour"

local class = require("common/middleclass")



page_ui = class("page_ui",lua_mono_behaviour)

function page_ui:initialize(game_obejct)
  lua_mono_behaviour.initialize(self,game_obejct)
  self.page_type = PageType.FULL_SCREEN
  self.always_in_memery = true
  self.option_str = ""
  self.option_dict = {}
  self.is_open = false
  self.is_show = false
  self.init_positon = Vector3.zero
end


function page_ui:initData()

end



function page_ui:getOptionStr()
  self:saveOptionStr()
  self.option_str = ""
  for k,v  in pairs(self.option_dict) do
    self.option_str = self.option_str..k.."="..v.."&"
  end
  return self.option_str 
end

function page_ui:saveOptionStr()
                            printUI(self.page_name.." saveOptionStr")

end


function page_ui:onActive()
                            printUI(self.page_name.." onActive")

end


function page_ui:onUnActive()
                            printUI(self.page_name.." onUnActive")

end

function page_ui:parseOptionStr()
  printUI(self.page_name.." parseOptionStr")

  self.option_dict = {}
  local str_array = StringUtil.GetSplitString(self.option_str,"&")
  local length = str_array.Length
  for i = 0,length-1 do
    local str = tostring(str_array[i])
    local str_s = StringUtil.GetSplitString(str,"=")
    local k = tostring( str_s[0])
    local v = tostring( str_s[1])
    self.option_dict[k] = v
    printUI("parseOptionStr key:"..k.." val:"..v)
  end
end

function page_ui:open(options)
  printUI(self.page_name.." open")

  self.option_str = options
  self.is_open = true
  self:parseOptionStr()
  self:show()
  self:doOpen()
end

function page_ui:doOpen()
                      printUI(self.page_name.." doOpen")

end

function page_ui:onForceClose()
                    printUI(self.page_name.." onForceClose")

end


function page_ui:closeSelf()
  printUI(self.page_name.." closeSelf")


  self.page_manager:onClosePage()
end

--自己不要调用这个API
function page_ui:close()
  printUI(self.page_name.." close")

  self.is_open = false
  self:hide()
  self:doClose()
end


function page_ui:reOpen(options)
                printUI(self.page_name.." reOpen")

  self.option_str = options
  self:parseOptionStr()
  self:doReOpen()
end

function page_ui:doReOpen()
              printUI(self.page_name.." doReOpen")

end


function page_ui:doClose()
            printUI(self.page_name.." doClose")

end


function page_ui:setOptionValue(k,v)
          printUI(self.page_name.." setOptionValue")

  self.option_dict[k] = v
end

function page_ui:hide()
        printUI(self.page_name.." hide")

  self.gameObject:SetActive(false)
  self.is_show = false
end

function page_ui:show()
      printUI(self.page_name.." show")

  self.transform.localPosition = self.init_positon
  self.gameObject:SetActive(true)
  self.is_show = true
end

function page_ui:onCoverPageRemove()
    printUI(self.page_name.." onCoverPageRemove")

end

function page_ui:onCoverPageOpen()
    printUI(self.page_name.." onCoverPageOpen")
end


function page_ui:onMemeryPageDestroy()
  printUI(self.page_name.." onMemeryPageDestroy")
end








return page_ui