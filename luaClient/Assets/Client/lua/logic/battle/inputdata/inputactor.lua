inputactor = class("inputactor")


function inputactor:initialize()
        self.guid = -1;
        self.hero_id = -1;
        self.level = 0;
        self.random_seed = 9527;
        self.order = -1
        self.position_x = 0;
        self.position_y = 0;
        self.position_z = 0;
        self.euler_y = 0;
        
        self.logic_pos_row = 1--(1-2)
        self.logic_pos_col = 1--(1-3)

        self.logic_camp_type = LogicCampType.ATTACK;
        self.obj_type = ObjType.PLAYER;

        self.hp = 20;
        self.max_hp = nil;
        self.anger = 0;
        self.max_anger = 100;
        self.attack = 0
        self.phy_def = 0--物防
        self.mag_def = 0--模防
        self.attack_speed = 0

        -- <summary>
        -- 英雄所有可使用技能,对英雄而言就是英雄已解锁的所有技能,小怪是单独配的额外表(list)
        -- </summary>
        self.hero_skills = {}

        -- <summary>
        -- 副本小怪ID
        -- </summary>
        self.copy_enemy_id = -1
        
        -- <summary>
        -- 召唤物ID
        -- </summary>
        self.summon_enemy_id = -1;
end



return inputactor