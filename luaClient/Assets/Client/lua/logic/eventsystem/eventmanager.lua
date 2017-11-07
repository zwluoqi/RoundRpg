
eventmanager = class("eventmanager")

function eventmanager:initialize()
    self._events_map = { }
end

function eventmanager:addEventListener(event_type, listener)
    local tmp_event = self._events_map[event_type]
    if not tmp_event then
        self._events_map[event_type] = event(event_type)
        tmp_event = self._events_map[event_type]
    end
    if tmp_event then
        tmp_event:Add(listener)
    end
end

function eventmanager:removeEventListener(event_type, listener)
    local tmp_event = self._events_map[event_type]
    if tmp_event then
        tmp_event:Remove(listener)
    end
end

function eventmanager:remove_all_event_listener(event_type)
    local tmp_event = self._events_map[event_type]
    if tmp_event then
        tmp_event:Clear()
    end
end

function eventmanager:triggerEvent(event_type, ...)
    printEvent("eventmanager:triggerEvent"..event_type)
    local tmp_event = self._events_map[event_type]
    if tmp_event then
        local args = { ...}
        tmp_event(args)
    end
end

return eventmanager