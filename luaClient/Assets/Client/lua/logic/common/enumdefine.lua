
EventManagerDefine =
{
    --通知Engine开始工作
    battle_view2logic_startworking = "battle_view2logic_startworking",
    --通知Engine开始一个回合
    battle_view2logic_startonce_round = "battle_view2logic_startonce_round",
    --通知Engine结束一个回合
    battle_view2logic_endonce_round = "battle_view2logic_endonce_round",
    --通知Engine开始一场战斗
    battle_view2logic_startonce_order = "battle_view2logic_startonce_order",
    --通知Engine结束一场战斗
    battle_view2logic_endonce_order = "battle_view2logic_endonce_order",
    --通知Engine某一个角色放大招
    battle_view2logic_request_utl_skill_click = "battle_view2logic_request_utl_skill_click",
    --通知Engine切换自动
    battle_view2logic_request_auto = "battle_view2logic_request_auto",



    --提示View层，可以释放大招
    battle_logic2view_could_use_utl = "battle_logic2view_could_use_utl",
    --提示View层，可以结束回合
    battle_logic2view_could_endonce_round = "battle_logic2view_could_endonce_round",
    --提示View层，可以结束一场战斗
    battle_logic2view_could_endonce_order = "battle_logic2view_could_endonce_order",

    battle_logic2view_actor_start_skill = "battle_logic2view_actor_start_skill",
    battle_logic2view_actor_interrupt_skill = "battle_logic2view_actor_interrupt_skill",
    battle_logic2view_actor_end_skill = "battle_logic2view_actor_end_skill",
    
    --数据输出
    battle_logic2view_actor_skill_export = "battle_logic2view_actor_skill_export",
    
    --整个战斗结束
    battle_logic2view_battle_over = "battle_logic2view_battle_over",

    battle_logic2view_skill_release_project = "battle_logic2view_skill_release_project",
    --抛射物释放
    battle_logic2view_skill_move_to_target = "battle_logic2view_skill_move_to_target",
    --施法前移动
    battle_logic2view_skill_return_to_source = "battle_logic2view_skill_return_to_source",
    --施法后返回
    
    battle_logic2view_exchange_weather = "battle_logic2view_exchange_weather",
    --切换天气
}

StageType = {
    NONE = "none",
    TOWN = "stagetown",
    FUBEN = "stagefuben",
  
}

PageType = {
  FULL_SCREEN = "FULL_SCREEN",
  COVER = "COVER",
  
  }

LocalResourceType={
  Prefab = "Prefab",
  Scene = "Scene",
}

BattleViewState =
{
    NONT = "NONE",
    EMPTY = "empty",
    LOADING = "loading",
    RUNNING = "running",
    FINISH = "finish",
}


EventType = {
          None = "none",
        Once = "once",
        Count_Loop = "count_loop",
        Infinity_loop = "infiniity_loop",
}

EventLifeCircle = {
          CREATE = "create",
        DOING = "doing",
        PAUSE = "pause",
        DEATH = "death",
  }

--<summary>
-- 阵营信息
--</summary>
LogicCampType = 
{
    NONE = "NONE",
    ATTACK = "ATTACK",--攻方
    DEFENCE ="DEFENCE",--守方
    YEGUAI = "YEGUAI",--野怪
    PEACE = "PEACE",--中立
}

ObjType = {
          NONE = "NONE",
        PLAYER= "PLAYER",
        MONSTER = "MONSTER",
        NPC = "NPC",
        BOX = "BOX",
  }

AIClass = {
  NONE = "NONE",
  COPY_ENMEY = "COPY_ENMEY",
  MAIN = "MAIN",
  TEAM_MEMBER = "TEAM_MEMBER",
}

    SkillLogicType = 
    {
        None = "NONE",
        IMMEDIATE = "IMMEDIATE",

        PASSITIVE = "PASSITIVE",--
    }
    
    SkillKind = {
              NONE = "0",
        normal = "normal",
        skill = "skill",
        utl = "utl",
      }

      
SkillState = {
  None = "none",
  ExchangeWeather = "exchange_weather",
  GotoPos = "gotopos",
  Before = "before",
  Skilling = "skilling",
  After = "after",
  RertunPos = "returnpos",
  End = "end",
  
}

    --选人目标
  SelectTargetType = 
    {
        NONE = 0,	--
        ALL = "all",	--
        ENEMY = "enemy",	--
        FRIEND = "friend",	--
        SELF ="self",	--
        FRIEND_NOT_SELF = "friend_not_self",	--
    }
    
  --策略
  SelectStrategyType = 
    {
        NONE = 0,	--
        base = "base",
        face_col = "face_col",--对位列
        face_front_row = "face_front_row",--前横排
        face_back_row = "face_back_row",--后横排
        random = "random",--随机
        all = "all",--所有
        min_hp = "min_hp",--血量最少
    }
    
    --选择范围类型
        ReleaseRangeType = 
    {
        Circle = 0,--
        Square = 1,--
    }

AnimName = {
  IDLE = "idle",
  RUN = "run_2",
  WALK = "walk",
  DIE = "die",
  HIT = "hit",
  
}

  ExportValueClass =     {        NONE = 0,        DEMAGE ="demage",--damage
        MP ="mp",--mp        ANGER = "anger",--anger
        DEAD = "dead",
        HIT = "hit",    }
  
  HpInfSpecialType =   {      None = 0,       Crit ="crit", --暴击          Miss = "miss",     --闪避
  }

HangPointType = 
    {
        UNKOWN = 0,         -- 没有挂点(跟随人)
        --
        HEAD = 1,           -- 头
        LEFTHAND = 2,       -- 左手
        RIGHTHAND = 3,      -- 右手
        BODY = 4,           -- 躯干
        BOW = 5,            -- 臀部
        LEFTFEET = 6,       -- 左脚
        RIGHTFEET = 7,      -- 右脚
        HEADNUB = 8,        -- 面部
        ROOT = 9,           -- 根
        CENTER = 10,        -- 质心
        ARMS = 11,          -- 挂到武器的源点上
        RIDE = 12,--骑乘点
        leftWeaponTra =13,--左手武器
        rightWeaponTra = 14,--右手武器
        Cloth = 15,--衣服

        HEAD_EXCURSION = 20,       -- 头上根据身高偏移
        BODY_EXCURSION = 21,       -- 胸前偏移
        FEET_EXCURSION = 22,       -- 脚下偏移

        NOBONE = 30,               -- 没有挂点(不跟随人)

    }