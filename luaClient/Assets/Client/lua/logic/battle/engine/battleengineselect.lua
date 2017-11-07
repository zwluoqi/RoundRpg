

battleengineselect = class("battleengineselect")




function battleengineselect:initialize()
  
end


function battleengineselect:getHeroByTarget(e_actor,select_target_type)
  local select_logic_camp_types = {}
  if select_target_type == SelectTargetType.ALL then
    select_logic_camp_types[LogicCampType.YEGUAI] = 0
    select_logic_camp_types[LogicCampType.DEFENCE]= 0
    select_logic_camp_types[LogicCampType.ATTACK]= 0
    return battlemanager.instance().battle_engine:getHeroByCamps(select_logic_camp_types)
  elseif select_target_type == SelectTargetType.ENEMY then
    select_logic_camp_types = e_actor.enemy_logic_camp_types
    return battlemanager.instance().battle_engine:getHeroByCamps(select_logic_camp_types)
  elseif select_target_type == SelectTargetType.FRIEND then  
    select_logic_camp_types = e_actor.input_actor.friend_logic_camp_types
    return battlemanager.instance().battle_engine:getHeroByCamps(select_logic_camp_types)
  elseif select_target_type == SelectTargetType.SELF then  
    return {[e_actor.guid] = e_actor}
  elseif select_target_type == SelectTargetType.FRIEND_NOT_SELF then  
    select_logic_camp_types = e_actor.input_actor.friend_logic_camp_types
    local heros = battlemanager.instance().battle_engine:getHeroByCamps(select_logic_camp_types)
    heros[e_actor.guid] = nil
    return heros
  else
    return {}
  end
  
  
end

function battleengineselect:getTargets(source_e_actor,select_target_type,select_target_strategy)
  if select_target_strategy == SelectStrategyType.base then
    local target = self:getBaseTarget(source_e_actor,select_target_type)
    if target == nil then
      return {}
    else
      return {[target.guid] = target}
    end
  elseif select_target_strategy == SelectStrategyType.face_col then
    return self:getFaceCol(source_e_actor,select_target_type)
  elseif select_target_strategy == SelectStrategyType.face_front_row then
    return self:getFaceFrontRow(source_e_actor,select_target_type)
  elseif select_target_strategy == SelectStrategyType.face_back_row then
    return self:getFaceBackRow(source_e_actor,select_target_type)
  elseif select_target_strategy == SelectStrategyType.random then
    return self:getRandom(source_e_actor,select_target_type)
  elseif select_target_strategy == SelectStrategyType.all then
    return self:getAll(source_e_actor,select_target_type)
  elseif select_target_strategy == SelectStrategyType.min_hp then
    local target = self:getMinHP(source_e_actor,select_target_type)
    if target == nil then
      return {}
    else
      return {[target.guid] = target}
    end
  else
    local target = self:getBaseTarget(source_e_actor,select_target_type)
    if target == nil then
      return {}
    else
      return {[target.guid] = target}
    end
  end
  
end

--血量最少
function battleengineselect:getMinHP(source_e_actor,select_target_type)
  local selected_targets = self:getHeroByTarget(source_e_actor,select_target_type)
  
  
  local min_hp = 1000000000
  local target = nil
  for k,v in  pairs(selected_targets) do
    if v.e_actor_atrribute_manager.hp < min_hp then
      target = v
      min_hp = v.e_actor_atrribute_manager.hp 
    end
  end
  
  return target;
end


--所有
function battleengineselect:getAll(source_e_actor,select_target_type)
  local selected_targets = self:getHeroByTarget(source_e_actor,select_target_type)
  return selected_targets
end


--随机N个
function battleengineselect:getRandom(source_e_actor,select_target_type)
  local selected_targets = self:getHeroByTarget(source_e_actor,select_target_type)
  local max_num = table.nums(selected_targets)
  
  local select_num =  battlemanager.instance().battle_engine.random_util.value*max_num;
  select_num = int(select_num)
  select_num = Mathf.Clamp(select_num,1,max_num)
  
  
  --TODO
  local selected_num =0;
  local targets = {}
  for k,v in selected_targets do
    if selected_num < select_num then
        targets[k] = v
    end
  end
  return targets
  
end


--后排
--1.后排
--2.前排
function battleengineselect:getFaceBackRow(source_e_actor,select_target_type)
    local selected_targets = self:getHeroByTarget(source_e_actor,select_target_type)
    local targets = {}
    local check =false
    
    for k,v in pairs(selected_targets) do
      if v.input_actor.logic_pos_col == 2 then
        targets[k] = v
        check = true
      end
    end
    
    if check then
        return targets
    end
    
    
    for k,v in pairs(selected_targets) do
      if v.input_actor.logic_pos_col == 1 then
        targets[k] = v
        check = true
      end
    end
    return targets
 
end


--前排
--1.前排
--2.后排
function battleengineselect:getFaceFrontRow(source_e_actor,select_target_type)
    local selected_targets = self:getHeroByTarget(source_e_actor,select_target_type)
    local targets = {}
    local check =false
    
    for k,v in pairs(selected_targets) do
      if v.input_actor.logic_pos_col == 1 then
        targets[k] = v
        check = true
      end
    end
    
    if check then
        return targets
    end
    
    
    for k,v in pairs(selected_targets) do
      if v.input_actor.logic_pos_col == 2 then
        targets[k] = v
        check = true
      end
    end
    return targets
 
end




--对位列
--1.对位列
--2.列排序
function battleengineselect:getFaceCol(source_e_actor,select_target_type)
    local selected_targets = self:getHeroByTarget(source_e_actor,select_target_type)
    local targets = {}
    local check =false
    local min_col = 3
    for k,v in pairs(selected_targets) do
      if v.input_actor.logic_pos_col == source_e_actor.input_actor.logic_pos_col then
        targets[k] = v
        check = true
      end
      if v.input_actor.logic_pos_col < min_col then
        min_col = v.input_actor.logic_pos_col
      end
      
    end
    
    if check then
        return targets
    end
    
    for k,v in pairs(selected_targets) do
      if v.input_actor.logic_pos_col == min_col then
        targets[k] = v
      end
    end
    return targets
end



--基本对位选择
--1.对位前排
--2.对位后排
--3.距离最近
function battleengineselect:getBaseTarget(source_e_actor,select_target_type)
    local selected_targets = self:getHeroByTarget(source_e_actor,select_target_type)

    local sort_targets_list = {}
    for k,v in pairs(selected_targets) do
        battleengineselect.sortTargetByBase(source_e_actor.input_actor.logic_pos_col,v,sort_targets_list)
    end
    
    return sort_targets_list[1]
end

function battleengineselect.sortTargetByBase(col,e_actor,sort_e_actor_list)
  
    for index,index_e_actor in pairs(sort_e_actor_list) do
      if e_actor.input_actor.logic_pos_row == 1 then
        if index_e_actor.input_actor.logic_pos_row ~= 1 then
          table.insert(sort_e_actor_list,index,e_actor)
          return
        elseif index_e_actor.input_actor.logic_pos_col == col then
          --not insert
        elseif e_actor.input_actor.logic_pos_col == col then
          table.insert(sort_e_actor_list,index,e_actor)  
          return
        elseif e_actor.input_actor.logic_pos_col < index_e_actor.input_actor.logic_pos_col then
          table.insert(sort_e_actor_list,index,e_actor) 
          return
        else
          --not insert
        end
      else
        if index_e_actor.input_actor.logic_pos_row == 1 then
          --not insert
        elseif index_e_actor.input_actor.logic_pos_col == col then
          --not insert
        elseif e_actor.input_actor.logic_pos_col == col then
          table.insert(sort_e_actor_list,index,e_actor) 
          return
        elseif e_actor.input_actor.logic_pos_col < index_e_actor.input_actor.logic_pos_col then
          table.insert(sort_e_actor_list,index,e_actor) 
          return
        else
          --not insert
        end
      end
    end
    
    table.insert(sort_e_actor_list,e_actor)
    
end


function battleengineselect:getTagetAtPos(row,col,selected_targets)
    for k,v in pairs(selected_targets) do
      if v.input_actor.logic_pos_row == row and v.input_actor.logic_pos_col == col then
        return v
      end
    end

end

function battleengineselect:getTargetByNearestPos(row,col,selected_targets)
  
  
      local target = nil
      local min_space = 1000
      for k,v in pairs(selected_targets) do
          local space = math.abs( v.input_actor.logic_pos_row - row) + math.abs(v.input_actor.logic_pos_col - col)
          if space < min_space then
              target = v
          end
      end
    return target
end







return battleengineselect