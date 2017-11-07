

timeeventhandler = class("timeeventhandler")

function timeeventhandler:initialize()
  self:initData()
end

function timeeventhandler:initBaseData()
  self.guid = getGUID()
  self.handler = nil;
  self.handler_name = nil
  self.end_handler = nil;
  
  self.trigger_time = 0;
  self.space_time = 0;
  self.count = 0;
  self.remainder_time  = 0;

  
  self.event_type = EventType.None;
  self.state = EventLifeCircle.CREATE;
  self.data = {}  
end


function timeeventhandler:initData(handler,handler_name, trigger_time)
    self:initBaseData()

            self.handler = handler;
            self.handler_name = handler_name;
            self.trigger_time = trigger_time;

            self.event_type = EventType.Once;
end

function timeeventhandler:initCountLoop(handler,handler_name, end_handler, trigger_time, space_time, count)
    self:initBaseData()

            self.handler = handler;
                        self.handler_name = handler_name;

            self.end_handler = end_handler;
            self.trigger_time = trigger_time;
            self.space_time = space_time;
            self.count = count;

            self.event_type = EventType.Count_Loop;
end

function timeeventhandler:initDataLoop(handler,handler_name, trigger_time, space_time)
    self:initBaseData()

            self.handler = handler;
                        self.handler_name = handler_name;

            self.trigger_time = trigger_time;
            self.space_time = space_time;

            self.event_type = EventType.Infinity_loop;
end


return timeeventhandler