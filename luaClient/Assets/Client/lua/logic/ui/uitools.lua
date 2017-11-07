
uitools = class("uitools")

function uitools.SpawnUIObject(prefab_path,parent_trans)
    local go =  GameObjectPoolManager.Instance:GetGameObjectDirect(prefab_path)

    go.name = prefab_path;
    go.layer = LayerMask.NameToLayer("UI");

    go.transform:SetParent(parent_trans);
    unitytools.resetPos(go)
    
    local rtTr = go:GetComponent(typeof(UnityEngine.RectTransform))

    rtTr.anchoredPosition = Vector2.zero;
    rtTr.sizeDelta = Vector2.zero;
    
    rtTr.anchorMax = Vector2.one
    rtTr.anchorMin = Vector2.zero

    return go
end

function uitools.loadUIObject(prefab_path,parent_trans)
    local asset =  ResourceManager.Instance:LoadResourceBlock(prefab_path)

    local go = UnityEngine.GameObject.Instantiate(asset)
    
    go.name = prefab_path;
    go.layer = LayerMask.NameToLayer("UI");

    go.transform:SetParent(parent_trans);
    unitytools.resetPos(go)
    
    local rtTr = go:GetComponent(typeof(UnityEngine.RectTransform))

    rtTr.anchoredPosition = Vector2.zero;
    rtTr.sizeDelta = Vector2.zero;
    
    rtTr.anchorMax = Vector2.one
    rtTr.anchorMin = Vector2.zero

    return go
end

return uitools