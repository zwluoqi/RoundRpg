
exportengineevent = class("exportengineevent")




        function exportengineevent: exportCouldEndOnceOrder(onceDatas)
                  battlemanager.instance().event_manager:triggerEvent(EventManagerDefine.battle_logic2view_could_endonce_order)
          
        end
        
        function exportengineevent: exportCouldEndOnceRound(onceDatas)
                  battlemanager.instance().event_manager:triggerEvent(EventManagerDefine.battle_logic2view_could_endonce_round)
          
        end
        
        function exportengineevent: exportWaitRequestUtil()
                  battlemanager.instance().event_manager:triggerEvent(EventManagerDefine.battle_logic2view_could_use_utl)
          
        end
  

        -- <summary>
        -- 输出战斗数据
        -- </summary>
        -- <param name="onceDatas"></param>
        function exportengineevent: exportBattleOnceData(once_data)
                  battlemanager.instance().event_manager:triggerEvent(EventManagerDefine.battle_logic2view_actor_skill_export,{["once_data"] = once_data})
          
        end

        -- <summary>
        -- 通知施法开始事件
        -- </summary>
        -- <param name="skill"></param>
        function exportengineevent: exportBeginSkill( skill)
          
          
          battlemanager.instance().event_manager:triggerEvent(EventManagerDefine.battle_logic2view_actor_start_skill,{ ["skill"] = skill})
          
          
        end

        -- <summary>
        -- 通知施法结束事件
        -- </summary>
        -- <param name="skill"></param>
        function exportengineevent: exportEndSkill( skill)
                    battlemanager.instance().event_manager:triggerEvent(EventManagerDefine.battle_logic2view_actor_end_skill,{ ["skill"] = skill})
          
          end

        -- <summary>
        -- 通知施法中断事件
        -- </summary>
        -- <param name="skill"></param>
        -- <param name="accord"></param>
        function exportengineevent: exportInterruptSkill( skill,  accord)
                              battlemanager.instance().event_manager:triggerEvent(EventManagerDefine.battle_logic2view_actor_interrupt_skill,{ ["skill"] = skill})
       
          end







        -- <summary>
        -- 输出战斗结束信息
        -- </summary>
        function exportengineevent: exportEndBattlingMsg(win_lose,desc)
          
                battlemanager.instance().event_manager:triggerEvent(EventManagerDefine.battle_logic2view_battle_over,{["win_lose"] = win_lose,["desc"] = desc})
                
end

    
    
    
    function exportengineevent:exportSkillMoveToTarget(e_actor_skill,e_target_actor)
      printSkill("exportSkillMoveToTarget:"..e_target_actor.guid)
                battlemanager.instance().event_manager:triggerEvent(EventManagerDefine.battle_logic2view_skill_move_to_target,{ ["e_actor_skill"] = e_actor_skill,["e_target_actor"] = e_target_actor})
    end
    
    function exportengineevent:exportSkillReturnToSource(e_actor_skill)
                battlemanager.instance().event_manager:triggerEvent(EventManagerDefine.battle_logic2view_skill_return_to_source,{ ["e_actor_skill"] = e_actor_skill})
    end 
    
    
    function exportengineevent:exportSkillReleaseProject(e_actor_skill_release,e_target_actor)
        battlemanager.instance().event_manager:triggerEvent(EventManagerDefine.battle_logic2view_skill_release_project,{ ["e_actor_skill_release"] = e_actor_skill_release,["e_target_actor"] = e_target_actor})
    end
    
    function exportengineevent:exportExchangeWeather(pre_weather_id,weather_id)
        battlemanager.instance().event_manager:triggerEvent(EventManagerDefine.battle_logic2view_exchange_weather,{["pre_weather_id"] = pre_weather_id, ["weather_id"] = weather_id})
    end
    
    

return exportengineevent