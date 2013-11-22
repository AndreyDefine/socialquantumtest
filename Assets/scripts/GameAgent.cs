using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// Главный класс игры 
/// </summary>

public class GameAgent : Abstract {
	//выполняются ли действия в update
	private bool flagRun=false;
	
	private AbstractElementFactory BubleFactory;
	private TextureFactory TextureFactory1;
	//time to show new bubble
	private float timePassed;
	
	private static GameAgent instance=null;
			
	//получить singleton
	public static GameAgent GetSharedGameAgent()
	{
		return instance;	
	}
	
	// Use this for initialization
	void Start () {
		//not best way to make singleton
		if(!instance)
		{	
			instance=this;
		}
		else
		{
			throw new Exception("Can't use two GameAgent!!!");
		}
		//очистим кеш, для теста!!!
		Caching.CleanCache();
		//bubbles factory
		BubleFactory=AbstractElementFactory.GetFabric("BubbleFactory","http://squeakyoak.com/files/tmp/socialquantum.unity3d","Bubble,Bubble2");
		BubleFactory.PreloadPoolObjects(new ObjectLoadedCallbackDelegate(ObjectLoadedCallbackPreload));
		//textureFactory
		TextureFactory1=TextureFactory.GetFabric("TextureFactory1");
		//create GuiLayer
		GuiLayer.GetSharedGuiLayer();
		
		timePassed=Time.time;
	}
	
	//changesetof textures
	public void ChangeSet()
	{
		TextureFactory1.ChangeSet();
	}
		
		
	//reset game
	public void Reset()
	{
		//удалить все текстуры
		TextureFactory1.DestroyAll();
		//не удаляем объекты а переводим их в неиспользуемые
		BubleFactory.ReStart();
	}
	
	//сгенерировать один кружочек
	private void GenerateOneBuble()
	{
		BubleFactory.GetNewObject(new ObjectLoadedCallbackDelegate(ObjectLoadedCallback));
	}
	
	//callback from fabric preload objects
	private void ObjectLoadedCallbackPreload(GameObject newObject,string instr)
	{
		//если всё хорошо!
		if(newObject)
		{
			GuiLayer.GetSharedGuiLayer().flagLoading=false;
			flagRun=true;
		}
		else
		{
			GuiLayer.GetSharedGuiLayer().message="Error in Loading!!!";
		}
	}
	
	//callback from fabric
	private void ObjectLoadedCallback(GameObject newObject,string instr)
	{
		if(newObject)
		{
			float minscale=0.5f,maxscale=1.5f;
			FallingBottom newFallingBottom=newObject.GetComponent<FallingBottom>();
			float newscale=UnityEngine.Random.Range(minscale,maxscale);
			float newfalling=GlobalOptions.difficultyLevel*0.05f/newscale;
			float newrotation=UnityEngine.Random.Range(1f,4f)*(UnityEngine.Random.Range(0,2)>0?1:-1);
			
			float newscore=(10f*GlobalOptions.difficultyLevel)/newscale;
			//задаём параметры
			newFallingBottom.singleTransform.localScale=new Vector3(newscale,newscale,newscale);
			newFallingBottom.movingBottomVector=new Vector3(0,-newfalling,0);
			newFallingBottom.movingRotation=new Vector3(0,0,newrotation);
			newFallingBottom.score=(int)newscore;
			
			int indexOfTextureScale=(int)((newscale-minscale)/((maxscale-minscale)/4f));
			
			//материал создаётся при вызове .material
			TextureFactory1.GetNewObject(TextureFactory1.GetSizeForIndex(indexOfTextureScale),newFallingBottom.singleRenderer.material);
			
			Vector2 newbounds=newFallingBottom.singleRenderer.bounds.extents;
			float newy=Cameras.MainCamera.orthographicSize+newbounds.y;
			float newx=(Cameras.MainCamera.orthographicSize*Cameras.MainCamera.aspect-newbounds.x)*UnityEngine.Random.Range(-1f,1f);						
			newFallingBottom.singleTransform.position=new Vector3(newx,newy,1f);
			//задаём позицию, в которой кружочек пропадает
			newFallingBottom.yPositionToRestart=-newy;
			newFallingBottom.flagIsActive=true;
			newFallingBottom.TextureFactory1=TextureFactory1;
		}
	}
	
	// Update is called once per frame
	void Update () {
		if(!flagRun)
			return;
		//нужно ли добавлять шарик
		if(Time.time-timePassed>=3f)
		{
			//сбросили счётчик
			timePassed=Time.time;
			//изменяем уровень сложности по нарастающей
			GlobalOptions.difficultyLevel+=0.02f;
			float maxDifficulty=3f;
			GlobalOptions.difficultyLevel=GlobalOptions.difficultyLevel>maxDifficulty?GlobalOptions.difficultyLevel=maxDifficulty:GlobalOptions.difficultyLevel;

			GenerateOneBuble();
		}
	}
}
