

unitytools = {}

    
function unitytools.resetPos(game_object)

    game_object.transform.localPosition =  Vector3.zero
    game_object.transform.localRotation =Quaternion.identity
    game_object.transform.localScale = Vector3.one
end


function unitytools.loadUIObject(prefab_path,parent_trans)
    local asset =  ResourceManager.Instance:LoadResourceBlock(prefab_path)

    local go = UnityEngine.GameObject.Instantiate(asset)
    
    go.name = prefab_path;
    go.layer = LayerMask.NameToLayer("Default");

    go.transform:SetParent(parent_trans);
    unitytools.resetPos(go)
    

    return go
end


return unitytools