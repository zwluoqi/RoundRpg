
exportengineemptyevent = class("exportengineemptyevent")




        -- <summary>
        -- 输出战斗数据
        -- </summary>
        -- <param name="onceDatas"></param>
        function exportengineemptyevent: exportBattleOnceData(onceDatas)end

        -- <summary>
        -- 通知施法开始事件
        -- </summary>
        -- <param name="skill"></param>
        function exportengineemptyevent: exportBeginSkill( skill,  startSkillTime)end

        -- <summary>
        -- 通知施法结束事件
        -- </summary>
        -- <param name="skill"></param>
        function exportengineemptyevent: exportEndSkill( skill)end

        -- <summary>
        -- 通知施法中断事件
        -- </summary>
        -- <param name="skill"></param>
        -- <param name="accord"></param>
        function exportengineemptyevent: exportInterruptSkill( skill,  accord)end


        -- <summary>
        -- 中断Combo
        -- </summary>
        -- <param name="guid"></param>
        function exportengineemptyevent: exportBreakCombo( player)end

        -- <summary>
        -- 第几波怪出生
        -- </summary>
        -- <param name="order"></param>
        function exportengineemptyevent: exportOrderEnemyReady( order)end

        -- <summary>
        -- 第几波怪dead
        -- </summary>
        -- <param name="order"></param>
        function exportengineemptyevent: exportOrderEnemyClear( order)end



        -- <summary>
        -- 切换主英雄
        -- </summary>
        -- <param name="charactorId"></param>
        function exportengineemptyevent: exportExchangeMainHero()end

        -- <summary>
        -- 切换阵容
        -- </summary>
        -- <param name="player"></param>
        function exportengineemptyevent: exportExchangeTeam( isAttack,  exchangeType,  lastPlayer,  player)
end

        -- <summary>
        -- 主角到达目的地
        -- </summary>
        function exportengineemptyevent: exportMainHeroMoveToTargetPosDone( owner)
end

        -- <summary>
        -- 更新障碍物状态
        -- </summary>
        -- <param name="obstacle"></param>
        function exportengineemptyevent: exportObstacleState( obstacle)
end

        -- <summary>
        -- 主角切换目标事件
        -- </summary>
        -- <param name="hero"></param>
        function exportengineemptyevent: exportMainHeroConditionTarget( hero)
end

        -- <summary>
        -- 输出错误信息
        -- </summary>
        -- <param name="errorDesc"></param>
        function exportengineemptyevent: exportErrorMsg( errorDesc)
end
        -- <summary>
        -- 输出安全区变更信息
        -- </summary>
        -- <param name="hero"></param>
        -- <param name="enter"></param>
        function exportengineemptyevent: exportSafeChanged( hero,  enter)
end
        -- <summary>
        -- 输出移动
        -- </summary>
        -- <param name="dir"></param>
        -- <param name="state"></param>
        function exportengineemptyevent: exportMoveData( hero,  dir, move_state)
end


        -- <summary>
        -- 输出侦测信息
        -- </summary>
        -- <param name="radius_length"></param>
        -- <param name="angle_width"></param>
        -- <param name="detect"></param>
        -- <param name="rangeType"></param>
        function exportengineemptyevent: exportDetectData( hero,  radius_length,  angle_width,  detect,  rangeType)
end

        -- <summary>
        -- 输出重生信息
        -- </summary>
        -- <param name="hero"></param>
        function exportengineemptyevent: exportHeroReborn( hero)
end

        -- <summary>
        -- 输出战斗结束信息
        -- </summary>
        function exportengineemptyevent: exportEndBattlingMsg()
end
		function exportengineemptyevent: exportEngineAim( aim)end

return exportengineemptyevent