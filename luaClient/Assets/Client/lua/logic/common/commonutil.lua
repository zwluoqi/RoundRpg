function table.printLog(hash_table)
    for k,v in pairs(hash_table) do
      printLog("k:"..k.." v:"..tostring(v))
    end
    
end

function table.simpleCopy(hash_table)
    local new_table = {}
    for k,v in pairs(hash_table) do
      new_table[k] = v
    end
    return new_table
end

function table.simpleAdd(source_table,add_table)
    
    for k,v in pairs(add_table) do
      
      if source_table[k] ~= nil then
        error("exist key")
      end
      
        source_table[k] = v
    end

end


local uniqed_id = 9527

function getGUID()
    uniqed_id = uniqed_id+1
    return uniqed_id
end

function maxValue()
    return 1000000000
end

local max_vector3 = Vector3.New(99999,99999,99999)
function maxVector3()
  return max_vector3
end

function printLog(msg)
    --print(msg..debug.traceback())
end


function printBattleView(msg)
    print("<color=#A2CD5A>"..msg.."</color>\t\n"..debug.traceback())
end

function printBattleEngine(msg)
    print("<color=#9B30FF>"..msg.."</color>\t\n"..debug.traceback())
end

function printEvent(msg)
    print("<color=#CD6889>"..msg.."</color>\t\n"..debug.traceback())
end

function printTimeRoundEvent(msg)
    print("<color=#CD4F39>"..msg.."</color>\t\n"..debug.traceback())
end

function printBattleCamera(msg)
    print("<color=#00CD66>"..msg.."</color>\t\n"..debug.traceback())
end

function printWarning(msg)
    print("<color=yellow>"..msg.."</color>\t\n"..debug.traceback())
end

function printUI(msg)
    print("<color=green>"..msg.."</color>\t\n"..debug.traceback())
end

function printError(msg)
  
    Debugger.LogError(msg..debug.traceback())
    --print("<color=red>"..msg.."</color>\t\n"..debug.traceback())
end

function printSkill(msg)
    print("<color=orange>"..msg.."</color>\t\n"..debug.traceback())
end


function printActor(msg)
    print("<color=cyan>"..msg.."</color>\t\n"..debug.traceback())
end



