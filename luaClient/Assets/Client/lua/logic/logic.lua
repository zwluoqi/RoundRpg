--周威

--local ui_manager = require "uiscripts/ui_manager"
require "logic/battle/battlemanager"
require "logic/stage/stagemanager"
require "logic/eventsystem/eventmanager"
require "logic/ui/uimanager"

require "logic/common/init"
require "logic/dict/init"

require "logic/tool/unitytools"


dict_language =  require "logic/globalization/zh/string_table"

logic = class("logic")



function logic.startUp()
    printLog("logic")
    ui_manager = uimanager()
    battle_manager = battlemanager()
    lstage_manager = stagemanager()
    
    ui_manager:getPageManager():openPage("main_ui")
end



return logic