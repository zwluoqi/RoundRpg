
battledatautil = class("battledatautil")

function battledatautil.createActorByCopyEnemyID(copy_enemy_id,is_attack)
  local copy_enemy = dict_copy_enemy.getDataByID(copy_enemy_id,is_attack)
  return createActorByCopyEnemy(copy_enemy,is_attack)
end


function battledatautil.createActorByCopyEnemy(copy_enemy,is_attack)
  local inputactor = inputactor()
  inputactor.guid = getGUID()
  inputactor.copy_enemy_id = copy_enemy.id
  inputactor.hero_id = copy_enemy.hero_id
  inputactor.order = copy_enemy.order
  inputactor.pos_id = copy_enemy.pos_id
  inputactor.logic_pos_row = math.floor((inputactor.pos_id - 1)/3 + 1)
  inputactor.logic_pos_col = (inputactor.pos_id- 1) % 3 + 1


  battledatautil.setBaseAttribute(inputactor)
  
  local hero_skills = dict_hero_skill.getDataByIndexOne(copy_enemy.hero_id)
  if hero_skills == nil then
    inputactor.hero_skills  = {}
  else
    inputactor.hero_skills = table.simpleCopy(hero_skills)
  end

  
  return inputactor
end


function battledatautil.createActorByHeroID(hero_id)
  
  local inputactor = inputactor()
  local hero = dict_hero.getDataByID(hero_id)
  
  inputactor.guid = getGUID()
  inputactor.hero_id = hero.id
  inputactor.pos_id = 1
  inputactor.logic_pos_row = math.floor((inputactor.pos_id - 1)/3 + 1)
  inputactor.logic_pos_col = (inputactor.pos_id- 1) % 3 + 1
  
  battledatautil.setBaseAttribute(inputactor)
  
  local hero_skills = dict_hero_skill.getDataByIndexOne(hero.id)
  if hero_skills == nil then
    inputactor.hero_skills  = {}
  else
    inputactor.hero_skills = table.simpleCopy(hero_skills)
  end
  
  return inputactor

end



function battledatautil.setBaseAttribute(input_actor)
  local hero = dict_hero.getDataByID(input_actor.hero_id)
  input_actor.hp = hero.default_hp
  input_actor.attack = hero.default_attack
  input_actor.phy_def = hero.default_phy_defence
  input_actor.mag_def = hero.default_mag_defence
  input_actor.attack_speed = hero.attack_speed
end


return battledatautil