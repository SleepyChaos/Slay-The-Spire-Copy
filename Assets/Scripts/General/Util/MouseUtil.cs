using UnityEngine;

public static class MouseUtil
{
    private static Camera _camera;
    
    public static Camera camera
    {
        get
        {
            // 如果 Camera 为空或被销毁，重新获取
            if (_camera == null)
            {
                _camera = Camera.main;
                if (_camera == null)
                {
                    _camera = Object.FindObjectOfType<Camera>();
                }
            }
            return _camera;
        }
    }
    
    static MouseUtil(){
        Debug.Log("MouseUtil 工具已加载");
    }
    
    public static Vector3 GetMousePositionInWorldSpace(float zValue = 0f)
    {
        Camera cam = camera;
        if (cam == null)
        {
            Debug.LogError("❌ MouseUtil: 无法找到 Camera！");
            return Vector3.zero;
        }
        
        Plane dragPlane = new Plane(cam.transform.forward, new Vector3(0, 0, zValue));
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        if (dragPlane.Raycast(ray, out float distance))
        {
            return ray.GetPoint(distance);
        }
        return Vector3.zero;
    }
    
    /// <summary>
    /// 重置 Camera 引用（场景切换后调用）
    /// </summary>
    public static void ResetCamera()
    {
        _camera = null;
    }
}
