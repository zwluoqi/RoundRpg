


pagemanager = class("pagemanager")


function pagemanager:initialize(ui_manager,game_obejct)
  self.ui_manager = ui_manager
    self.unity_page_root_game_object = game_obejct

  --<dict>已经加载的页面，不管是否在队列中
  self.page_dict = {}
  --<list>页面堆栈信息，1号位为最前面的页面
  self.page_history_strs = {}
  
  --<list>当前显示的页面
  self.active_pages = {}
  
  
  
  self.current_page = nil
  
  self.full_screen_current_sort_order = 101
  self.cover_current_sort_order = 501  
  
end

function pagemanager:openPage(page_name,option)
  if option == nil then
      option = ""
  end
  
  if self.current_page ~= nil and self.current_page.page_name == page_name then
    self.current_page:reOpen(option)
    return
  end
  
  local page_at_top = self:getPage(page_name)
  
  if page_at_top == nil then
    printError("can not find the page:"..page_name)
    return
  end
  
  if page_at_top.is_show then
    printError("page_at_top page is_show:"..page_at_top.page_name)
    return
  end
  
  if self.current_page ~= nil then
    self.current_page:onUnActive()
    local current_page_option_str = self.current_page.page_name.."?"..self.current_page:getOptionStr()
    table.insert(self.page_history_strs,1,current_page_option_str)
    if page_at_top.page_type == PageType.FULL_SCREEN then
      for k,v in pairs(self.page_history_strs) do
        local strs = StringUtil.GetSplitString(v,"?")
        local p_name = tostring( strs[0])
        if self.page_dict[p_name] then
          
          if self.page_dict[p_name].is_open then
            self.page_dict[p_name]:onForceClose()
            self:simpleClosePage(self.page_dict[p_name])
          else
            break
          end
        else
          break
        end
      end
      
    else
      self.current_page:onCoverPageOpen()
    end
  end
  
  
  self.current_page = page_at_top
  self:simpleOpenPage(page_at_top,option)
  
  page_at_top:onActive()  
  
end





function pagemanager:simpleOpenPage(page,option)
  if page.page_type == PageType.FULL_SCREEN then
    self.ui_manager:autoSetCanvasSortOrder(page.gameObject,self.full_screen_current_sort_order)
    self.full_screen_current_sort_order = self.full_screen_current_sort_order+1
  else
    self.ui_manager:autoSetCanvasSortOrder(page.gameObject,self.cover_current_sort_order)
    self.cover_current_sort_order = self.cover_current_sort_order +1
  end

  if page.is_open then
    page:reOpen(option)
  else
    page:open(option)
  end
  table.insert(self.active_pages,page)
end

function pagemanager:simpleClosePage(page)
  for k,v in pairs(self.active_pages) do
    if v == page then
      table.remove(self.active_pages,k)
    end
  end
  
  for k,v in pairs(self.active_pages) do
    if v == page then
      printError("simpleClose Error."..page.page_name)
      return
    end
  end
  
  page:close()
  
  if page.page_type == PageType.FULL_SCREEN then
    self.full_screen_current_sort_order = self.full_screen_current_sort_order-1
    --self.ui_manager:autoSetCanvasSortOrder(page.gameObject,self.full_screen_current_sort_order)
  else
    self.cover_current_sort_order = self.cover_current_sort_order -1
    --self.ui_manager:autoSetCanvasSortOrder(page.gameObject,self.cover_current_sort_order)
  end
  if page.always_in_memery  == false then
    self.page_dict[page.page_name] = nil
    uGameObejct.Destroy(page.gameObject)
  end
end

function pagemanager:onClosePage()
  local is_cover = (self.current_page.page_type == PageType.COVER)
  local current_page_name = self.current_page.page_name
  local page_to_close = self.current_page
  
  page_to_close:onUnActive()
  self:simpleClosePage(page_to_close)

  local history_num = table.nums(self.page_history_strs)
  if history_num == 0 then
    printError("history empty!!!!!!!!!!!!!!!!")
    return
  end
  
  local page_and_option = self.page_history_strs[1]
  table.remove(self.page_history_strs,1)
  
  local strs = StringUtil.GetSplitString(page_and_option,"?")
  local page_name = tostring(strs[0])
  local page_option =""
  if strs.Length >1 then
    page_option = tostring(strs[1])
  end
  
  
  
  
  local pre_page = self:getPage(page_name)
  self.current_page = pre_page
  self.current_page:onActive()  

  if is_cover then
    self.current_page:onCoverPageRemove()
    return
  end
  
  if pre_page ~= nil then
    self:simpleOpenPage(pre_page,page_option)
    if pre_page.page_type ~= PageType.FULL_SCREEN then
      for k,v in pairs( self.page_history_strs) do
        local h_strs = StringUtil.GetSplitString(v,"?")
        local h_page_name = tostring(h_strs[0])
        
        local h_page_option =""
        if h_strs.Length >1 then
          h_page_option = tostring(h_strs[1])
        end
      
        local h_page = self:getPage(h_page_name)
        self:simpleOpenPage(h_page,h_page_option)
        if h_page.page_type == PageType.FULL_SCREEN then
          break
        end
      end
    end
  else
    printError("page_history error")
  end
  
end


function pagemanager:getPage(page_name)
  local page = nil
  if self.page_dict[page_name] ~= nil then
    page = self.page_dict[page_name]
  else
    page = self:createPageObject(page_name)
    if page ~= nil then
      self.page_dict[page_name] = page
      page.page_manager = self
      page.page_name = page_name
    end
  end
  return page
  
end


function pagemanager:getOpenedPage(page_name)
  local page = nil
  if self.page_dict[page_name] ~= nil then
    page = self.page_dict[page_name]
  else
    
  end
  return page
end

function pagemanager:closeAllPage()

  self.current_page:onUnActive()
  self.current_page:onForceClose()
  self:simpleClosePage(self.current_page)
  self.current_page = nil
  
  for k,v in pairs(self.page_history_strs) do
    local strs = StringUtil.GetSplitString(v,"?")
    local p_name = tostring( strs[0])
    if self.page_dict[p_name] ~= nil  then
      
      if self.page_dict[p_name].is_open then
        self.page_dict[p_name]:onForceClose()
        self:simpleClosePage(self.page_dict[p_name])
      end
      
    end
  end
  self.page_history_strs = {}
end




function pagemanager:createPageObject(page_name)
    local go = uitools.loadUIObject("ui/page/"..page_name,self.unity_page_root_game_object)
    
    local ui_page = (require ("logic/ui/page/"..page_name))(go)
    ui_page:initData()
    
    printUI("CreatePage::>> " .. page_name .. " " .. go.name);
    return ui_page
end


return pagemanager