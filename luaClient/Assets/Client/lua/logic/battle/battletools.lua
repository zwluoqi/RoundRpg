
battletools = {}

function battletools.reverse_camp(source_type)
  
    local reverse_camp_types = {}
    if (source_type == LogicCampType.ATTACK) then
      reverse_camp_types[LogicCampType.DEFENCE] = 0
      reverse_camp_types[LogicCampType.YEGUAI] = 0
    elseif (source_type == LogicCampType.DEFENCE)
    then
      reverse_camp_types[LogicCampType.ATTACK] = 0
      reverse_camp_types[LogicCampType.YEGUAI] = 0
    elseif (source_type == LogicCampType.YEGUAI)
    then
      reverse_camp_types[LogicCampType.ATTACK] = 0
      reverse_camp_types[LogicCampType.DEFENCE] = 0
    else
      
    end
          
    return reverse_camp_types

end

return battletools