require "logic/battle/inputdata/battledatautil"
require "logic/battle/inputdata/inputroom"
require "logic/battle/inputdata/inputactor"
require "logic/dict/init"

battledatacreater = class("battledatacreater")

function battledatacreater.createDataFromCopyID(copy_id)
    local input_room = inputroom()
    input_room.random_seed = os.time()
    input_room.copy_id = copy_id
    
    local copy_data = dict_copy.getDataByID(copy_id)
    input_room.scene_id = copy_data.copy_sence_id
    local scene_data = dict_scene.getDataByID(copy_data.copy_sence_id)
    
    local scene_pos_datas = dict_scene_position.getDataByIndexOne(scene_data.sence_res_id)
    
    local copy_enmeys = dict_copy_enemy.getDataByIndexOne(copy_id)
    
    
    if copy_enmeys == nil then
      printError("copy_enmeys nil:"..tostring(copy_id))
    end
    
    local index = 1
    for k,v in pairs(copy_enmeys) do
      
      if input_room.input_def_actors[v.order] == nil then
        input_room.input_def_actors[v.order] = {}
      end
      
      local input_actor = battledatautil.createActorByCopyEnemy(v,false)
      input_actor.logic_camp_type = LogicCampType.DEFENCE
      
      --[[local scene_pos_id = scene_data.scene_defence_pos_id_s[v.pos_id]
      local pos_data = scene_pos_datas[scene_pos_id].pos_data
      input_actor.position_x = pos_data[1]
      input_actor.position_y = pos_data[2]
      input_actor.position_z = pos_data[3]
      input_actor.euler_y = pos_data[4]
      ]]
      
      
      table.insert(input_room.input_def_actors[v.order],input_actor)
    
    end
    
    
    local attack_actors = dict_copy_enemy.getDataByIndexOne(copy_id-1)

    for k,v in pairs(attack_actors) do
      
      if input_room.input_atk_actors[v.order] == nil then
        input_room.input_atk_actors[v.order] = {}
      end
      
      local input_actor = battledatautil.createActorByCopyEnemy(v,true)
      input_actor.logic_camp_type = LogicCampType.ATTACK
      
      local scene_pos_id = scene_data.scene_attack_pos_id_s[v.pos_id]
      local pos_data = scene_pos_datas[scene_pos_id].pos_data
      --[[input_actor.position_x = pos_data[1]
      input_actor.position_y = pos_data[2]
      input_actor.position_z = pos_data[3]
      input_actor.euler_y = pos_data[4]
      ]]
      
      table.insert(input_room.input_atk_actors[v.order],input_actor)
    
    end


    return input_room
end



return battledatacreater