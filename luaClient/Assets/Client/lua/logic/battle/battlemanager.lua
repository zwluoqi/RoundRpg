require "logic/dict/init"
require "logic/battle/inputdata/battledatacreater"
require "logic/battle/view/battleview"
require "logic/battle/engine/battleengine"
require "logic/battle/camera/battlecamera"
require "logic/battle/battletools"
require "logic/battle/battleconfig"


require "logic/common/basemodel"

local class = require("common/middleclass")
battlemanager = class("battlemanager",basemodel)

local _instatnce = nil
function battlemanager.instance()
  return _instatnce
end

function battlemanager:Start()
  
    printLog("battlemanager:Start")
    
    
end

function battlemanager:Update()
--    printLog("battlemanager:Update")
    self.delta_time = Time.deltaTime
    self:tick();

end

function battlemanager:tick()
  
    basemodel.tick(self)
    
    self.time_event_manager:tick(self.delta_time)
    
    if self.working then
      self.battle_engine:tick()
      self.battle_view:tick()
      self.battle_camera:tick()
    end
    
end




function battlemanager:initialize()
      

    
    printLog("battlemanager:initialize()")
    _instatnce = self
    self.name = "battlemanager"

    self.manager_game_object = UGameObject.New("battlemanager")
    UGameObject.DontDestroyOnLoad(self.manager_game_object)
    local luaBehaviour = self.manager_game_object:AddComponent(typeof(LuaBehaviour))
    luaBehaviour:Init(self)
    
    self.battle_engine = battleengine(self)
    self.battle_view = battleview(self)
    self.battle_camera  = battlecamera(self)
    
    self.event_manager = eventmanager()
    self.time_event_manager = timeeventmanager("battle_manager_time_event")
    self.time_event_manager:ready()
    

end

function battlemanager:ready()
    self.current_timer = 0;
    self.real_timer = 0;
    self.excute_count = 0;
    self.delta_time = 0;
		self.battle_engine:ready();
    self.battle_view:ready();
		self.battle_camera:ready ();

    self.working = true;  
    
    self.battle_view:startWeather(self.battle_engine.e_weather.current_weather_data.weather_id)


    ui_manager.instance():getPageManager():openPage("battle_ui","")
    


    
    --开始战斗
    self.event_manager:triggerEvent(EventManagerDefine.battle_view2logic_startworking)
    

    
    --显示攻守方
    self.battle_view:showCurrentAttack()
    self.battle_view:readyCurrentAttack()
    self.battle_view:showOrderEnemy(1)
    self.battle_view:readyOrderEnemy(1)
    
    
    --开启一场战斗
    self.event_manager:triggerEvent(EventManagerDefine.battle_view2logic_startonce_order)
    --开启一个回合
    self.event_manager:triggerEvent(EventManagerDefine.battle_view2logic_startonce_round)
    
end

function battlemanager:clear()
    ui_manager.instance():getPageManager():closeAllPage()

    self.current_timer = 0;
    self.real_timer = 0;
    self.excute_count = 0;
    self.delta_time = 0;
		self.battle_engine:clear();
    self.battle_view:clear();
		self.battle_camera:clear ();

    self.working = false; 
    

end


function battlemanager:createFubenData(copy_id)
  local input_room = battledatacreater.createDataFromCopyID(copy_id)
  self.battle_engine:setRoomData(input_room)
  return input_room
end




return battlemanager