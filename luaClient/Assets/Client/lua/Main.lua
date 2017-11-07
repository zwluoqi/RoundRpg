-- 加载常用模块
require("init")


-- 主入口函数。从这里开始lua逻辑
function Main()
    print("Main")
    print("Update2")
    
    logic.startUp()

end




-- 场景切换通知
function OnLevelWasLoaded(level)
	Time.timeSinceLevelLoad = 0
end