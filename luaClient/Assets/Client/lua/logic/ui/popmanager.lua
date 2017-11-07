popmanager = class("popmanager")


function popmanager:initialize(ui_manager,game_obejct)
  self.current_pop_sort_order = 5001
    self.ui_manager = ui_manager
    self.unity_page_root_game_object = game_obejct

end


function popmanager:createPopMessage(msg)
    local pop_game_object = uitools.SpawnUIObject("Pop/base_pop",self.unity_page_root_game_object.transform)
    
    local ui_text = pop_game_object.transform:FindChild("text"):GetComponent(typeof(UnityEngine.UI.Text))
    ui_text.text = msg
    self.ui_manager:autoSetCanvasSortOrder(pop_game_object,self.current_pop_sort_order)    
    GameObjectPoolManager.Instance:Unspawn(pop_game_object,5)
    
    --DG.Tweening.Tweener.DOLocalMove(,Vector3.New(0,200,0),3)
    pop_game_object.transform:DOLocalMove(Vector3.New(0,200,0),3,false)

    self.current_pop_sort_order = self.current_pop_sort_order+1
    if self.current_pop_sort_order >6000 then
      self.current_pop_sort_order  = 5001  
    end
    
end





