using UnityEngine;
using System.Collections;

/// <summary>
/// Класс, который отвечает за движение кружочков вниз, 
/// а также за все отсальные аспекты
/// объекта кружок, отклики на тач
/// </summary>

public class FallingBottom : AbstractTag,TouchTargetedDelegate {
	
	public Vector3 movingBottomVector=new Vector3(0,0,0);
	public float yPositionToRestart=0;
	public bool _flagIsActive=false;
	public int score=0;
	public TextureFactory TextureFactory1;
	
	public bool flagIsActive {
        get {
            return _flagIsActive;
        }
		set {
			_flagIsActive = value;
			//активный кружок
			if(_flagIsActive)
			{
				//do nothing
			}
			else
			{
				//объект может быть переиспользован
				DeleteFromUsed();
				if(TextureFactory1)
				{
					TextureFactory1.DeleteCurrent(singleRenderer.material);
				}
			}
		}
    }
	
	//делаем объект неактивным
	public override void MakeInactive(){
		_flagIsActive=false;
	}

	// Use this for initialization
	void Start () {
		//не захватываем тач события
		TouchDispatcher.GetSharedTouchDispatcher().addTargetedDelegate(this,0,false);
	}
	
	//поймали кружок
	void BubbleCatched()
	{
		GlobalOptions.score+=score;
		flagIsActive=false;
		MakeCatchedAnimation();
	}
	
	//сделать по поимке шарика эффект
	void MakeCatchedAnimation()
	{
		singleTransform.position=new Vector3(-9999,-9999,-9999);
	}
	
	
	//равномерное поступательное движение вниз
	void MoveBottom()
	{
		singleTransform.Translate(movingBottomVector);
		if(singleTransform.position.y<=yPositionToRestart)
		{
			flagIsActive=false;
		}
	}
	
	// Update is called once per frame
	void Update () {
		if(!flagIsActive) return;
		MoveBottom();
	}
	
	//TouchDelegateMethods
	public virtual bool TouchBegan(Vector2 position,int fingerId) {
		bool isTouchHandled=MakeDetection(position);
		if(isTouchHandled)
		{
			//был пойман тач необходимо это показать
			BubbleCatched();
		}
		return isTouchHandled;
	}
	
	public virtual void TouchMoved(Vector2 position,int fingerId) {
		//do nothing
	}
	
	public virtual void TouchEnded(Vector2 position,int fingerId) {
		//do nothing
	}
	
	public virtual void TouchCanceled(Vector2 position,int fingerId) {
		//usually the same as end
		TouchEnded(position,fingerId);
	}
	
	//end TouchDelegateMethods
	
	
	//сделать проверку попали ли мы в объект по  нажатию
	protected virtual bool MakeDetection(Vector2 position)
	{
		bool isTouchHandled=false;
		float pixelPerUnit=Cameras.pixelPerUnit;
		//проверим попадает ли точка в круг, вообще можно было бы использовать здесь лучи, но для 2д использовать 3д математику?
		Vector3 center=singleRenderer.bounds.center*pixelPerUnit+new Vector3(Screen.width/2,Screen.height/2,0);
		float Radius=singleRenderer.bounds.extents.x*pixelPerUnit;
		
		float f= Mathf.Pow(position.x-center.x,2) + Mathf.Pow(position.y-center.y,2);
		//попадает
    	if (f<= Radius*Radius){
			isTouchHandled=true;
		}
		return isTouchHandled;
	}
}
