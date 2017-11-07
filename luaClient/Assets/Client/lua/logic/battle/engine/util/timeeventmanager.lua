require "logic/battle/engine/util/timeeventhandler"

timeeventmanager = class("timeeventmanager")


function timeeventmanager:initialize(name)
        self.time_event_manager_name = name
        --[[self.event_list = nil
        self.pause_list = nil
        self.cache_list = nil
        self.death_list = nil
        self.temp_list = nil]]
        self.event_list = {}
        self.pause_list = {}
        self.cache_list = {}
        self.death_list = {}
        self.temp_list = {}
        self. time_line = 0;
        self.paused = false;
        self.handlerCount = 0
end

function timeeventmanager:clear()
  
  --if self.event_list.length > 0 then
    if table.nums(self.event_list) > 0 and self.event_list ~= nil then
   -- local ilist = self.event_list.first
    --local ilist = self.event_list[1]
   --[[ while ilist do
        --local e = ilist.value
        local e = ilist
        if e.state ~= EventLifeCircle.DEATH then
          printError(self.time_event_manager_name.." handler name:"..e.handler_name.." state:"..e.state)
        end
        
        ilist = e._next
    end]]
    for k,v in pairs(self.event_list) do
      if v.state ~= EventLifeCircle.DEATH then
          printError(self.time_event_manager_name.." handler name:"..v.handler_name.." state:"..v.state)
        end
      
    end
    
    
  end
  
 -- if self.pause_list.length > 0 then
    --[[if table.nums(self.pause_list) > 0 then
    local ilist = self.pause_list.first
    while ilist do
        local e = ilist.value
        if e.state ~= EventLifeCircle.DEATH then
          printError(self.time_event_manager_name.." handler name:"..e.handler_name.." state:"..e.state)
        end
        ilist = e._next
    end
  end]]
  
  if table.nums(self.pause_list) > 0 and self.pause_list ~= nil then
    for k,v in pairs (self.pause_list) do
      if v.state ~= EventLifeCircle.DEATH then
          printError(self.time_event_manager_name.." handler name:"..v.handler_name.." state:"..v.state)
        end
    end
    
  end
  
  
   -- if self.cache_list.length > 0 then
     --[[ if table.nums(self.cache_list)  > 0 then
        local ilist = self.cache_list.first
    while ilist do
        local e = ilist.value
        if e.state ~= EventLifeCircle.DEATH then
          printError(self.time_event_manager_name.." handler name:"..e.handler_name.." state:"..e.state)
        end
        ilist = e._next
    end
  end]]
  
  if table.nums(self.cache_list) > 0 and self.cache_list ~= 0 then
    for k,v in pairs(self.cache_list) do
      if v.state ~= EventLifeCircle.DEATH then
          printError(self.time_event_manager_name.." handler name:"..v.handler_name.." state:"..v.state)
        end
    end
    
  end
  
  
  
    --if self.death_list.length > 0 then
     --[[ if table.nums(self.death_list) > 0 then
            local ilist = self.death_list.first
    while ilist do
        local e = ilist.value
        if e.state ~= EventLifeCircle.DEATH then
          printError(self.time_event_manager_name.." handler name:"..e.handler_name.." state:"..e.state)
        end
        ilist = e._next
    end
  end]]
  
  if table.nums(self.death_list) > 0 and self.death_list ~= nil then
    for k,v in pairs(self.death_list) do
      if v.state ~= EventLifeCircle.DEATH then
          printError(self.time_event_manager_name.." handler name:"..v.handler_name.." state:"..v.state)
        end
    end
    
  end
  
  
  --if self.temp_list.length > 0 then
    if table.nums(self.temp_list) > 0 and self.temp_list ~= nil then
    printError(self.time_event_manager_name.." temp_list count >0")
  end
  
   --[[ self.event_list:clear()
    self.pause_list:clear()
    self.cache_list:clear()
    self.death_list:clear()
    self.temp_list:clear()]]
    
    if self.event_list ~= nil and table.nums(self.event_list) > 0 then
      for k,v in pairs(self.event_list) do
        table.remove(self.event_list,k)
      end
    end
    
     if self.pause_list ~= nil and table.nums(self.pause_list) > 0 then
      for k,v in pairs(self.pause_list) do
        table.remove(self.pause_list,k)
      end
    end
     if self.cache_list ~= nil and table.nums(self.cache_list) > 0 then
      for k,v in pairs(self.cache_list) do
        table.remove(self.cache_list,k)
      end
    end
     if self.death_list ~= nil and table.nums(self.death_list) > 0 then
      for k,v in pairs(self.death_list) do
        table.remove(self.death_list,k)
      end
    end
     if self.temp_list ~= nil and table.nums(self.temp_list) > 0 then
      for k,v in pairs(self.temp_list) do
        table.remove(self.temp_list,k)
      end
    end
  
end


function timeeventmanager:ready()
      self. time_line = 0;
      self.paused = false;
      --[[self.event_list = list()
      self.pause_list = list()
      self.cache_list = list()
      self.death_list = list()
      self.temp_list = list()]]
      
      self.event_list = {}
      self.pause_list = {}
      self.cache_list = {}
      self.death_list = {}
      self.temp_list = {}
end

function timeeventmanager:tick(delta_time)
  if self.paused then
    return
  end

            self:updateTimeline(delta_time);
            self:updateCacheList();
            self:updatePauseList();
            self:updateLifeCircle();
            self:updateDoingList();
  
end

function timeeventmanager:updateTimeline(delta_time)
  self. time_line = self. time_line + delta_time;
end

function timeeventmanager:updateCacheList()
  --if self.cache_list.length > 0 then
    --[[if table.nums(self.cache_list) > 0 then
    local ilist = self.cache_list.first
    while ilist do
      local v = ilist.value
        if v.state == EventLifeCircle.CREATE or v.state == EventLifeCircle.DOING then
          self:addEvent(v)
        elseif v.state == EventLifeCircle.PAUSE then
          self.pause_list:push(v)
        else
          printError("error event state:"..v.state.." fun_name:"..v.handler_name)
        end
        ilist = ilist._next
    end
    --self.cache_list:clear()
    self.cache_list = nil
  end]]
  
  if self.cache_list ~= nil and  table.nums(self.cache_list) > 0 then
    for k,v in pairs (self.cache_list) do
        if v.state == EventLifeCircle.CREATE or v.state == EventLifeCircle.DOING then
          self:addEvent(v)
        elseif v.state == EventLifeCircle.PAUSE then
          table.insert(self.pause_list, v)
        else
          printError("error event state:"..v.state.." fun_name:"..v.handler_name)
        end
    end
    
    if self.cache_list ~= nil and table.nums(self.cache_list) > 0 then
      for k,v in pairs(self.cache_list) do
        table.remove(self.cache_list,k)
      end
      
    end
    
    
    
  end
  

end


function timeeventmanager:updatePauseList()
  --if self.pause_list.length > 0 then
    --[[if table.nums(self.pause_list) > 0 then
    --self.temp_list:clear()
    self.temp_list = nil
    local ilist = self.pause_list.first
    while ilist do
      local v = ilist.value
      if v.state == EventLifeCircle.DOING then
          addEvent(v)
          self.temp_list:push(v)
      end
      ilist = ilist._next
    end
    
    ilist = self.temp_list.first
    while ilist do
      self.pause_list:erase(ilsit.value)
      ilist = ilist._next
    end
    
    --self.temp_list:clear()
    self.temp_list = nil
  end]]
  
  if table.nums(self.pause_list) > 0 then
    if self.temp_list ~= nil and table.nums(self.temp_list) > 0 then
      for k,v in pairs(self.temp_list) do
        table.remove(self.temp_list,k)
      end
      
    end
    
    
    for k,v in pairs(self.pause_list) do
      if v.state == EventLifeCircle.DOING then
          addEvent(v)
          table.insert(self.temp_list,v)
      end
    end
    
    if self.temp_list ~= nil and self.pause_list ~= nil then
      for k,v in pairs(self.temp_list) do
        for ki,vi in pairs(self.pause_list) do
          if v == vi then
             table.remove(self.pause_list,ki)
          end
        end
      end
    end
    
    
    if  self.temp_list ~= nil and table.nums(self.temp_list) then
      for k,v in pairs(self.temp_list) do
      table.remove(self.temp_list,k)
      end
    end
    
  end
end

function timeeventmanager:updateLifeCircle()
--[[  local ilist = self.event_list.first
  
  while ilist do
    local v = ilist.value
    if v.state == EventLifeCircle.CREATE then
      self.cache_list:push( v)
    elseif v.state == EventLifeCircle.PAUSE then
      self.pause_list:push( v)
    elseif v.state == EventLifeCircle.DEATH then
      self.death_list:push( v)
    else
      --not todo
    end
    ilist = ilist._next
  end
  
  ilist = self.cache_list.first
  while ilist do
    self.event_list:erase(ilist.value)
    ilist = ilist._next
  end
  
  ilist = self.pause_list.first
  while ilist do
    self.event_list:erase(ilist.value)
    ilist = ilist._next
  end

  ilist = self.death_list.first
  while ilist do
    self.event_list:erase(ilist.value)
    ilist = ilist._next
  end
 -- self.death_list:clear()
  self.death_list = nil]]
  
  
  if self.event_list ~= nil then
    for k,v in pairs(self.event_list) do
      if v.state == EventLifeCircle.CREATE then
        table.insert(self.cache_list,v)
      elseif v.state == EventLifeCircle.PAUSE then
         table.insert(self.pause_list,v)
      elseif v.state == EventLifeCircle.DEATH then
        table.insert(self.death_list,v)
      else
        --not todo
      end
    end
  end

  
  
  if self.cache_list ~= nil   and self.event_list ~= nil then
    for k,v in pairs(self.cache_list) do
      for ki,vi in pairs(self.event_list) do
        if v == vi then
          table.remove(self.event_list,ki)
        end
        
      end
      
    end
  end

  
  if self.pause_list ~= nil and self.event_list ~= nil then
    for k,v in pairs(self.pause_list) do
      for ki,vi in pairs(self.event_list) do
        if v == vi then
           table.remove(self.event_list,ki)
        end
        
      end
    end
  end

  
  if self.death_list ~= nil and self.event_list ~= nil then
    
    for k,v in pairs(self.death_list) do
      for ki,vi in pairs(self.event_list) do
        if v == vi then
          table.remove(self.event_list,ki)
        end
        
      end
    end
  end

  if self.death_list ~= nil and table.nums(self.death_list) > 0 then
    for k,v in pairs(self.death_list) do
      table.remove(self.death_list,k)
    end
    
  end
  
end




function timeeventmanager:updateDoingList()
  --[[local ilist = self.event_list.first
  
  while ilist do
      local e = ilist.value
      
      if self. time_line >= e.trigger_time then
        
        if e.state == EventLifeCircle.DOING then
          
          if e.event_type == EventType.Once then
            self.handlerCount = self.handlerCount +1
            e.state = EventLifeCircle.DEATH
            e.handler(e)
          elseif e.event_type == EventType.Count_Loop then
            self.handlerCount = self.handlerCount +1

            e.count = e.count - 1
            e.trigger_time = e.trigger_time+e.space_time
            if e.count == 0 then
              e.handler(e)
              e.end_handler(e)
              e.state = EventLifeCircle.DEATH
            else
              e.handler(e)
              e.state = EventLifeCircle.CREATE
            end
            
          elseif e.event_type == EventType.Infinity_loop then
            self.handlerCount = self.handlerCount +1

            e.trigger_time = e.trigger_time+e.space_time
            e.handler(e)
            e.state = EventLifeCircle.CREATE
          else
            e.state = EventLifeCircle.DEATH
          end
        
          
          
          
        end
        
        
        
        
      elseif e.trigger_time > 1000000000 then
        printError("严重逻辑时间错误:"..tostring(e.handler))
        break
      else
          break
      end
      ilist = ilist._next
  end]]
  
  
  
  for k,v in pairs(self.event_list) do
    if self. time_line >= v.trigger_time then
        
        if v.state == EventLifeCircle.DOING then
          
          if v.event_type == EventType.Once then
            self.handlerCount = self.handlerCount +1
            v.state = EventLifeCircle.DEATH
            v.handler(v)
          elseif v.event_type == EventType.Count_Loop then
            self.handlerCount = self.handlerCount +1

            v.count = v.count - 1
            v.trigger_time = v.trigger_time+e.space_time
            if v.count == 0 then
              v.handler(v)
              v.end_handler(v)
              v.state = EventLifeCircle.DEATH
            else
              v.handler(v)
              v.state = EventLifeCircle.CREATE
            end
            
          elseif v.event_type == EventType.Infinity_loop then
            self.handlerCount = self.handlerCount +1

            v.trigger_time = v.trigger_time+v.space_time
            v.handler(v)
            v.state = EventLifeCircle.CREATE
          else
            e.state = EventLifeCircle.DEATH
          end
        
          
          
          
        end
        
        
        
        
      elseif v.trigger_time > 1000000000 then
        printError("严重逻辑时间错误:"..tostring(v.handler))
        break
      else
          break
      end
     
  end
  
  
  
  
end

function timeeventmanager:addEvent(teh)
  teh.state = EventLifeCircle.DOING
  
 --[[ local ilist = self.event_list.first
  while ilist do
    if  teh.trigger_time < ilist.value.trigger_time then
      self.event_list:insertBefore(teh,ilist)
      return
    end
    ilist = ilist._next
  end
  
  self.event_list:push(teh)]]
  if self.event_list ~= nil then
    for k,v in pairs(self.event_list) do
      if  teh.trigger_time < v.trigger_time then
        if k ~= 1 then
        table.insert(self.event_list,k-1,teh)
        return
        else
         table.insert(self.event_list,1,teh)
        return
        end
        
      end
    end
  end
  table.insert(self.event_list,teh)
end

function timeeventmanager:delete(teh)
    if teh == nil then
        return
    end
    --self.cache_list:erase(teh)
    if self.cache_list ~= nil then
      for k,v in pairs(self.cache_list) do
        if v == teh then
          table.remove(self.cache_list,k)
        end
        
      end
    end
    
    teh.state = EventLifeCircle.DEATH    
end

function timeeventmanager:pause(teh)
    if teh == nil then
      
      return
    end
    
    if teh.state == EventLifeCircle.DEATH then
      return
    end
    
    teh.state = EventLifeCircle.PAUSE
    teh.remainder_time = teh.trigger_time - self. time_line
end

function timeeventmanager:continue(teh)
    if teh == nil then
      return
    end
    
    if teh.state ~= EventLifeCircle.PAUSE then
      return
    end
    
    teh.state = EventLifeCircle.DOING
    teh.trigger_time = self. time_line+teh.remainder_time
end

function timeeventmanager:getEventRemainderTime(teh)
  if teh == nil then
    return maxValue
  end
  
  if teh.state == EventLifeCircle.DEATH then
    return maxValue
  end
  
  if teh.state == EventLifeCircle.PAUSE then
    return teh.remainder_time
  end
  
  return teh.trigger_time - self. time_line
end

function timeeventmanager:changeEventRemainderFactor(teh,factor)
  if teh == nil then
    return
  end
  
  if teh.state == EventLifeCircle.DEATH then
    return
  end
  
  if teh.state == EventLifeCircle.PAUSE then
    teh.remainder_time = teh.remainder_time*factor
    return
  end
  
  if teh.state == EventLifeCircle.DOING or teh.state == EventLifeCircle.CREATE then
    teh.state = EventLifeCircle.CREATE
    teh.trigger_time = (teh.trigger_time - self. time_line)*factor + self. time_line
    if teh.event_type ~= EventType.Once then
        teh.space_time = teh.space_time*factor
    end
    
  end
  
  
  
end

function timeeventmanager:addEventRemainderTime(teh,time)
  if teh == nil then
    return
  end
  
  if teh.state == EventLifeCircle.DEATH then
    return
  end
  
  if teh.state == EventLifeCircle.PAUSE then
    teh.remainder_time = teh.remainder_time+time
    return
  end
  
  if teh.state == EventLifeCircle.DOING or teh.state == EventLifeCircle.CREATE then
    teh.state = EventLifeCircle.CREATE
    teh.trigger_time = teh.trigger_time + time
  end
end



function timeeventmanager:createEvent(eh,eh_name, delay)
  local teh = timeeventhandler();
  teh:initData(eh,eh_name, delay + self. time_line)
  --self.cache_list:push(teh);
  table.insert(self.cache_list,teh)

  printTimeRoundEvent(self.time_event_manager_name.."createEvent.."..eh_name.." current time_line:"..self.time_line.." trigger_time:"..teh.trigger_time)
  return teh;
end
        


function timeeventmanager:createEventLoop( eh,eh_name,  delay,  spaceTime)
        
    local teh = timeeventhandler();
    teh:initDataLoop(eh,eh_name, delay + self. time_line, spaceTime)
    --self.cache_list:push (teh);
    table.insert(self.cache_list,teh)
      printTimeRoundEvent(self.time_event_manager_name.."createEventLoop.."..eh_name.." current time_line:"..self.time_line.." trigger_time:"..teh.trigger_time)

    return teh;
end
        

function timeeventmanager:createEventCountLoop( eh,eh_name, eeh,  delay,  spaceTime, count)
        
    local teh =  timeeventhandler();
    teh:initCountLoop(eh,eh_name, eeh, delay + self. time_line, spaceTime, count)
    --self.cache_list:push (teh);
    table.insert(self.cache_list,teh)
      printTimeRoundEvent(self.time_event_manager_name.."createEventCountLoop.."..eh_name.." current time_line:"..self.time_line.." trigger_time:"..teh.trigger_time)

    return teh;
end
        



return timeeventmanager