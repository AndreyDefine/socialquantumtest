using UnityEngine;
using System.Collections;

/// <summary>
/// Диспатчер тачсобытий, Singleton
/// </summary>

public class TouchInfo
{
	public int curTouchNumber;
	public int []fingerId;
	public bool swallowsTouches;
	public int touchPriority;
	public TouchInfo(){
		touchPriority=0;
		curTouchNumber=0;
		//max 10 touches
		fingerId=new int[15];
		swallowsTouches=false;
	}
}

//singleton
public class TouchDispatcher : MonoBehaviour
{  
	
	private static TouchDispatcher instance = null;
	private ArrayList targetedHandlers=new ArrayList();
	private ArrayList targetedHandlersTouchInfo=new ArrayList();
	
	private ArrayList targetedHandlersToDel=new ArrayList();
  
	bool mouseReleased;
	
	public static TouchDispatcher GetSharedTouchDispatcher()
	{
		if(instance==null)
		{
			instance=(new GameObject()).AddComponent<TouchDispatcher>();
			instance.name="TouchDispatcher";
		}
		return instance;	
	}
	
    void Start () {
		mouseReleased=true;
	}  
  
	public void addTargetedDelegate(TouchTargetedDelegate intarget,int inTouchPriority,bool inswallowsTouches)
	{	
		int i=0;
		//searching for place to insert delegate
		for (i=0;i<targetedHandlers.Count;i++)
		{
			if((targetedHandlersTouchInfo[i] as TouchInfo).touchPriority>inTouchPriority)
				break;
		}
		
		targetedHandlers.Insert(i,intarget);
		
		TouchInfo newTouchInfo=new TouchInfo();
		newTouchInfo.swallowsTouches=inswallowsTouches;
		newTouchInfo.touchPriority=inTouchPriority;
		
		targetedHandlersTouchInfo.Insert(i,newTouchInfo);
	}
	
	public void removeDelegate(TouchTargetedDelegate intarget)
	{
		//add one to remove list
		targetedHandlersToDel.Add(intarget);
	}
	
	public void removeAllDelegates()
	{
		targetedHandlers.Clear();
		targetedHandlersTouchInfo.Clear();
	}
	
	private void ClearDelList()
	{
		int index;
		TouchTargetedDelegate curTarget;
		for(int i=0;i<targetedHandlersToDel.Count;i++){
			curTarget=targetedHandlersToDel[i] as TouchTargetedDelegate;
			index=targetedHandlers.IndexOf(curTarget);
			
			targetedHandlersTouchInfo.RemoveAt(index);
			targetedHandlers.Remove(curTarget);
		}
		targetedHandlersToDel.Clear();
	}
	
	private void Update() { 
		if(targetedHandlers.Count>0){
			MakeDetectionMouseTouch();
		}
		ClearDelList();
	}
	
	protected virtual void MakeDetectionMouseTouch()
	{
		#if UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN
        	MakeDetectionMouse();
		#else
       		MakeDetectionTouch();
		#endif
	}
	
	protected virtual void MakeDetectionMouse()
	{
		//left mouse button
		if(Input.GetMouseButtonDown (0)){
			//мышь не была отжата
			if(!mouseReleased){
				TouchCanceled(Input.mousePosition,1);
			}
			else
			{
            	if(TouchBegan(Input.mousePosition,1)){
					mouseReleased=false;
				}
			}
        }
		//зажатый компонент
		if(Input.GetMouseButton (0))
		{
			TouchMoved(Input.mousePosition,1);
		}
		//released button
		if(Input.GetMouseButtonUp (0))
		{
			mouseReleased=true;
			TouchEnded(Input.mousePosition,1);
		}
	}
	
	protected virtual void MakeDetectionTouch()
	{
        int count = Input.touchCount;
		Touch touch; 
        for (int i = 0; i < count; i++) {
            touch = Input.GetTouch (i); 
			switch (touch.phase) {
            		case TouchPhase.Began:TouchBegan(touch.position,touch.fingerId);break;
					case TouchPhase.Moved:TouchMoved(touch.position,touch.fingerId);break;
					case TouchPhase.Ended:TouchEnded(touch.position,touch.fingerId);break;
					case TouchPhase.Canceled:TouchCanceled(touch.position,touch.fingerId);break;
				}
			}
	}
	
	//TouchDelegateMethods
	public virtual bool TouchBegan(Vector2 position,int infingerId) {
		for (int i=0;i<targetedHandlers.Count;i++)
		{
			if((targetedHandlers[i] as TouchTargetedDelegate).TouchBegan(position,infingerId)){
				(targetedHandlersTouchInfo[i] as TouchInfo).fingerId[(targetedHandlersTouchInfo[i] as TouchInfo).curTouchNumber]=infingerId;
				(targetedHandlersTouchInfo[i] as TouchInfo).curTouchNumber++;
				if((targetedHandlersTouchInfo[i] as TouchInfo).swallowsTouches){
					break;
				}
			}
		}
		return true;
	}
	
	public virtual void TouchMoved(Vector2 position,int infingerId) {
		for (int i=0;i<targetedHandlers.Count;i++)
		{
			for(int j=0;j<(targetedHandlersTouchInfo[i] as TouchInfo).curTouchNumber&&i<targetedHandlers.Count;j++)
			{
				if((targetedHandlersTouchInfo[i] as TouchInfo).fingerId[j]==infingerId)
					(targetedHandlers[i] as TouchTargetedDelegate).TouchMoved(position,infingerId);
			}
		}
	}
	
	public virtual void TouchEnded(Vector2 position,int infingerId) {
		for (int i=0;i<targetedHandlers.Count;i++)
		{
			for(int j=0;j<(targetedHandlersTouchInfo[i] as TouchInfo).curTouchNumber;j++)
			{
				if((targetedHandlersTouchInfo[i] as TouchInfo).fingerId[j]==infingerId){
					(targetedHandlers[i] as TouchTargetedDelegate).TouchEnded(position,infingerId);
					//сместим события тач
					(targetedHandlersTouchInfo[i] as TouchInfo).curTouchNumber--;
					for(int k=j;k<(targetedHandlersTouchInfo[i] as TouchInfo).curTouchNumber;k++)
					{
						(targetedHandlersTouchInfo[i] as TouchInfo).fingerId[k]=(targetedHandlersTouchInfo[i] as TouchInfo).fingerId[k+1];
					}
				}
			}
		}
	}
	
	public virtual void TouchCanceled(Vector2 position,int infingerId) {
		for (int i=0;i<targetedHandlers.Count;i++)
		{
			for(int j=0;j<(targetedHandlersTouchInfo[i] as TouchInfo).curTouchNumber;j++)
			{
				if((targetedHandlersTouchInfo[i] as TouchInfo).fingerId[j]==infingerId){
					(targetedHandlers[i] as TouchTargetedDelegate).TouchCanceled(position,infingerId);
					//сместим события тач
					(targetedHandlersTouchInfo[i] as TouchInfo).curTouchNumber--;
					for(int k=j;k<(targetedHandlersTouchInfo[i] as TouchInfo).curTouchNumber;k++)
					{
						(targetedHandlersTouchInfo[i] as TouchInfo).fingerId[k]=(targetedHandlersTouchInfo[i] as TouchInfo).fingerId[k+1];
					}
				}
			}
		}
	}
	//end TouchDelegateMethods
}  