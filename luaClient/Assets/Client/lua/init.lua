require("common/init")
require("logic/logic")


UGameObject = UnityEngine.GameObject
UObject = UnityEngine.Object
UDebug = UnityEngine.Debug

--lua代码命名规范
--自定义类名，全部小写、不间断如 battlemaanger battleview
--函数名，驼峰命名 如 switchStage ready，resorceLoad，unloadResource等
--变量名，linux命名 单词下划线间隔 如 battle_manager battle_view view_fsm等
--枚举定义与c#类，Unity命名风格