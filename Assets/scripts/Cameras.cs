using UnityEngine;
using System.Collections;

/// <summary>
/// Класс для удобной работы с камерой, названной MainCamera
/// </summary>

public static class Cameras {
	
	//возвращает текущую главную камеру
	private static Camera _MainCamera=null;
    public static Camera MainCamera {
        get {
				if(!_MainCamera)
				{
		            foreach (Camera camera in Camera.allCameras) {
		            if (camera.name.Equals ("MainCamera")) {
		                _MainCamera=camera;
						break;
		            }
				}
	        }
        	return _MainCamera;
        }
	}    
	
	//возвращает количество пикселей в одном юните, для текущей главной камеры, 
	//для мобильной версии(размер экрана не меняется, нет смысла делать пересчёт, при изменении экрана)
	static float _pixelPerUnit=-1;

	public static float pixelPerUnit {
        get {
			if(_pixelPerUnit==-1)
			{
        		Vector3 p1 = MainCamera.WorldToScreenPoint (new Vector3 (0, 0, 0));
        		Vector3 p2 = MainCamera.WorldToScreenPoint (new Vector3 (0, 1f, 0));
        		_pixelPerUnit=Mathf.Abs(p2.y - p1.y);
			}
			return _pixelPerUnit;
        }
	}  
	
	public static void ResetCameraStats()
	{
		_pixelPerUnit=-1;
	}
}