

eweather= class("eweather")


function eweather:initialize(battle_engine)
  self.battle_engine = battle_engine
    self.current_temperature = 0
    self.current_humidity =0
    self.current_weather_data = nil
end

function eweather:initData(temperature,humidity)
  self.current_temperature = temperature
  self.current_humidity = humidity
  local changed,new_weather_data = self:checkWeatherChanged()
  self.current_weather_data = new_weather_data
end



function eweather:getSkillEffectFactor(skill_effect_type)
  if self.current_weather_data.skill_effect_types[skill_effect_type] ~= nil then
    return self.current_weather_data.skill_effect_factors[skill_effect_type]
  end
  return 0
end

function eweather:changeTemperature(add_temperature)
  if(add_temperature == 0) then
    return
  end
  
  self.current_temperature = self.current_temperature + add_temperature
  
  local changed,new_weather_data = self:checkWeatherChanged()
  if changed then
    self:exchangeWeather(new_weather_data)
  end
  
  return false
end

function eweather:changeHumidity(add_humidity)
  if(add_humidity == 0) then
    return
  end
  
  self.current_humidity = self.current_humidity + add_humidity

  local changed,new_weather_data = self:checkWeatherChanged()
  if changed then
    self:exchangeWeather(new_weather_data)
  end

  return false
end




function eweather:checkWeatherChanged()
  local new_weather_data = eweather.getWeatherDataByTH(self.current_temperature,self.current_humidity)

  if new_weather_data == nil then
    return false,nil
  end
  
  if new_weather_data == self.current_weather_data then
      return false,new_weather_data
  end
  return true,new_weather_data
end


function eweather:exchangeWeather(new_weather_data)

  local pre_weather_id = self.current_weather_data.weather_id
  self.current_weather_data = new_weather_data
  printBattleEngine("exchangeWeather from:"..pre_weather_id.." to:"..new_weather_data.weather_id)


  if self.current_weather_data.weather_type == "spetical" then
    self.current_temperature = self.current_weather_data.max_temperature
    self.current_humidity = self.current_weather_data.max_humidity
  --TODO
    self:exportOnceData()
  else
    --normal exchange
  end
  
  self.battle_engine.export_engine_event_util:exportExchangeWeather(pre_weather_id,new_weather_data.weather_id)
end

function eweather:getEndRoundResumeNormalWeather()
  if self.current_weather_data.weather_type == "spetical" then
    local resume_weather_data = eweather.getWeatherDataByTH(self.current_weather_data.min_temperature,    self.current_weather_data.min_humidity)
    if resume_weather_data ~= nil then
      return true, resume_weather_data
    else
      printError("error getResumeNormalWeather")
      return false, self.current_weather_data
    end
  end
  return false,self.current_weather_data
end


function eweather:endOnceRound()
  if self.current_weather_data.weather_type == "spetical" then
    self.current_temperature = self.current_weather_data.min_temperature
    self.current_humidity = self.current_weather_data.min_humidity
    local resume_weather_data = eweather.getWeatherDataByTH(self.current_temperature,    self.current_humidity)
    if resume_weather_data ~= nil then
      printBattleEngine("weather end round resume from:"..self.current_weather_data.weather_id.." to:"..resume_weather_data.weather_id)
      self.current_weather_data = resume_weather_data
    else
      printError("error weather endOnceRound")
      return false
    end
  end
  
  return true
end




function eweather:exportOnceData()
  
end



function eweather.getWeatherDataByTH(temperature,humidity)
  for k,v in pairs( dict_weather["data"]) do
    if v.min_temperature < temperature and temperature <= v.max_temperature and v.min_humidity < humidity and humidity <= v.max_humidity then
      return v
    end
  end
  return nil
end

return eweather






