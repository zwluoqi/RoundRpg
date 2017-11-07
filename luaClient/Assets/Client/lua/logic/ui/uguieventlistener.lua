

uguieventlistener = {}



function uguieventlistener.addUGUIOnClickListener(listener,fun)
  if listener.onClick == nil then
    listener.onClick = fun
  else
    listener.onClick = listener.onClick + fun
  end
end

function uguieventlistener.removeUGUIOnClickListener(listener,fun)
  if listener.onClick == nil then
    
  else
    listener.onClick = listener.onClick - fun
  end
end


--TODO 其他事件


return uguieventlistener