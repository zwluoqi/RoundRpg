    --[[local msg = totem_h1_pb.Person()
    
    
    msg.id = 1024
    msg.name = 'foo'
    msg.email = 'bar'


    local tmp_phtone = msg.phones:add()
    tmp_phtone.num = "12306"
    tmp_phtone.type = totem_h1_pb.Phone.HOME

    tmp_phtone = msg.phones:add()
    tmp_phtone.num = "12308"
    tmp_phtone.type = totem_h1_pb.Phone.MOBILE

    self.pb_data = msg:SerializeToString()
    print(self.pb_data)

    local buffer = ByteBuffer.New()
    buffer:WriteBuffer(self.pb_data)

                
                --UNetworkManager:SendMessage(buffer)
  
  
    self.page_manager.ui_manager:getPopManager():createPopMessage(self.pb_data)
]]