using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


//делегаты для работы с фабрикой объектов
public delegate void ObjectLoadedCallbackDelegate(GameObject newObject,string instr);
//делегат передаваемый Caching Load
public delegate void ObjectLoadedCallbackDelegateWithCustomCallBack(AssetBundle bundle,string instr,ObjectLoadedCallbackDelegate callback);

/// <summary>
/// Фабрика объектов с загрузкой через Asset Bundles
/// Pool объектов, для их переиспользования
/// В связи с тем, что загрузка одного и того же Bundle валится с ошибками 
/// (нельзя грузить один и тот же одновременно), 
/// необходимо сначала сделать предзагрузку пула, а потом уже производить с ним работу
/// </summary>

public class AbstractElementFactory: Abstract{
	//путь к AssetBundle.unity3d
	public string pathToResources="";
	//через запятую перечисленные имена объектов
	public string preloadNames="";
	
	//распарсеный список предзагрузки
	private string []objectPreload=null;
	
	//список используемых вещей
	protected List<AbstractTag> objectsList = new List<AbstractTag>();
	//список вещей в свободном пуле
	protected List<AbstractTag> objectsListToDel = new List<AbstractTag>();
	
	//фабрика должна быть монобехейвером, иначе она не сможет иметь полный доступ к управляемым элементам
	//метод для получения фабрики (можно сделать фабрику и в едиторе)
	public static AbstractElementFactory GetFabric(string inFabricName,string inpathToResources,string inpreloadNames)
	{
		AbstractElementFactory newinstance;
		newinstance=(new GameObject()).AddComponent<AbstractElementFactory>();
		newinstance.pathToResources=inpathToResources;
		newinstance.preloadNames=inpreloadNames;
		
		newinstance.name=inFabricName;
		return newinstance;	
	}
	
	void Start(){
		//предзагрузить имена вещей в пуле
		if(preloadNames!="")
		{
			parseObjectsNames();
		}
	}
	
	//Распарсить спикок объетков из строки
	public void parseObjectsNames()
	{
		//получили массив объектов
		char []separator={',','\n',' '};
		string []names=preloadNames.Split(separator);
		objectPreload=names;
	}
	
	//не удаляет объекты из пула, а только приводит их в исходное состояние
	public virtual void ReStart(){
		int i;
		Vector3 newPos=new Vector3(-9999,-9999,-9999);
		for(i=0;i<objectsListToDel.Count;i++){
			objectsListToDel[i].MakeInactive();
			objectsListToDel[i].singleTransform.position=newPos;
		}
		for(i=0;i<objectsList.Count;i++){
			objectsList[i].MakeInactive();
			objectsList[i].singleTransform.position=newPos;
			objectsListToDel.Add(objectsList[i]);
		}
		objectsList.Clear();
	}
	
	//удалить указанный
	public void DeleteCurrent(AbstractTag inObject){
		objectsList.Remove(inObject);			
		objectsListToDel.Add(inObject);
	}
	
	//поместить объект в первоначальные установки
	public virtual void PutToFirstState(AbstractTag newObject){
		//far from camera, unity recomends this
		newObject.singleTransform.position=new Vector3(-9999,-9999,-9999);
		newObject.singleTransform.rotation=Quaternion.identity;
	}
	
	//get random object from pull use only with preload
	public virtual void GetNewObject(ObjectLoadedCallbackDelegate callback){
		if(objectPreload==null&&preloadNames!="")
		{
			parseObjectsNames();
		}
				
		string objectname;
		int RandomIndex=UnityEngine.Random.Range (0,objectPreload.Length);
		objectname=objectPreload[RandomIndex];
		
		GetNewObjectWithName(objectname,callback);
	}
	
	//Уничтожить все объекты в пуле
	public virtual void DestroyPoolObjects()
	{
		AbstractTag newObject=null;
		for (int i=0; i<objectsListToDel.Count;i++){
			newObject	= objectsListToDel[i];
			Destroy(newObject.gameObject);
		}
		objectsListToDel.Clear();
		
		for (int i=0; i<objectsList.Count;i++){
			newObject	= objectsList[i];
			Destroy(newObject.gameObject);
		}
		objectsList.Clear();
	}
	
	//add all objects into pull
	public virtual void PreloadPoolObjects(ObjectLoadedCallbackDelegate customcallback)
	{
		if(objectPreload==null&&preloadNames!="")
		{
			parseObjectsNames();
		}
		

		LoadObjectWWW(null,new ObjectLoadedCallbackDelegateWithCustomCallBack(ObjectLoadedCallbackPreload),customcallback);

	}
	
	//получить объект по имени
	public virtual void GetNewObjectWithName(string instr,ObjectLoadedCallbackDelegate callback){
		GameObject newObject=null;
		int i;
		//ищем в пуле не используемых
		if(objectsListToDel.Count>0){
			for(i=0;i<objectsListToDel.Count;i++)
			{
				//нашли
				AbstractTag newObjectTag;
				if(objectsListToDel[i].name==instr){
					newObjectTag=objectsListToDel[i];
					objectsListToDel.Remove(newObjectTag);
					PutToFirstState(newObjectTag);
					newObject=newObjectTag.gameObject;
					break;
				}
			}
		}
		//нашли
		if(newObject)
		{
			BackToCaller(newObject,instr,callback);
			return;
		}
		//ничего не нашли в неиспользованных
		
		//ищем в пуле используемых
		if(objectsList.Count>0){
			for(i=0;i<objectsList.Count;i++)
			{
				//нашли
				AbstractTag newObjectTag;
				if(objectsList[i].name==instr){
					newObjectTag=objectsList[i];
					
					newObject=Instantiate(newObjectTag.gameObject) as  GameObject;
					addTagToObject(newObject);
					PutToFirstState(newObject.GetComponent<AbstractTag>());
					newObject.name=instr;
					break;
				}
			}
		}
		
		//нашли
		if(newObject)
		{
			BackToCaller(newObject,instr,callback);
			return;
		}
		//ничего не нашли в используемых
		
		//пытаемся загрузить из интернета
		if(!newObject)
		{
			LoadObjectWWW(instr,new ObjectLoadedCallbackDelegateWithCustomCallBack(ObjectLoadedCallbackLoad),callback);
		}
	}
	
	//загрузка объекта из бандла
	private void LoadObjectWWW(string instr,ObjectLoadedCallbackDelegateWithCustomCallBack callback, ObjectLoadedCallbackDelegate customcallback){
		GameObject newWWWRequest= new GameObject();
		newWWWRequest.name="newWWWRequest";
		newWWWRequest.AddComponent("CachingLoad");
		CachingLoad CachingLoadComponent=newWWWRequest.GetComponent<CachingLoad>();
		
		//set request parameters
		CachingLoadComponent.AssetName=instr;
		CachingLoadComponent.BundleURL=pathToResources;
		CachingLoadComponent.ObjectLoadedCallback=callback;
		CachingLoadComponent.customcallback=customcallback;
		
		CachingLoadComponent.StartLoading();		
	}
	
	//колбек от загрузки нового объекта c возвратом к вызывающему
	private void ObjectLoadedCallbackLoad(AssetBundle bundle,string instr,ObjectLoadedCallbackDelegate customcallback)
	{
		GameObject newObject=Instantiate(bundle.Load(instr)) as GameObject;
		bundle.Unload(false);
		if(newObject)
		{
			addTagToObject(newObject);	
			PutToFirstState(newObject.GetComponent<AbstractTag>());
			newObject.name=instr;
		}
		
		BackToCaller(newObject,instr,customcallback);
	}
	
	//колбек от предзагрузки нового объекта без возврата к вызывающему
	private void ObjectLoadedCallbackPreload(AssetBundle bundle,string instr,ObjectLoadedCallbackDelegate customcallback)
	{
		GameObject newObject=null;
		if(bundle)
		{
			Debug.Log ("good");
			for (int i=0; objectPreload!=null&&i<objectPreload.Length;i++){
				newObject=Instantiate(bundle.Load(objectPreload[i])) as GameObject;
				if(newObject)
				{
					addTagToObject(newObject);	
					PutToFirstState(newObject.GetComponent<AbstractTag>());
					newObject.name=objectPreload[i];
					objectsListToDel.Add(newObject.GetComponent<AbstractTag>());
				}
			}
				
			bundle.Unload(false);
		}
		//not best way to organize callback and response if all good or bad
		customcallback(newObject,null);
	}
	
	//возвратить объект, тому, кто его запросил
	private void BackToCaller(GameObject newObject,string instr,ObjectLoadedCallbackDelegate customcallback)
	{
		if(newObject)
		{
			objectsList.Add(newObject.GetComponent<AbstractTag>());
		}
		if(newObject==null)
		{
			//much better than exeption, not critical error
			Debug.Log ("NUUUUUUUUUUUUUL factory="+name+"name= "+instr);
		}
		customcallback(newObject,instr);
	}
	
	//добавить тег к объекту
	public virtual void addTagToObject(GameObject newObject){
		AbstractTag curTag;
		
		//объект создан как новый, и в нём нет AbstractTag
		if(!newObject.GetComponent<AbstractTag>())
		{
			newObject.AddComponent("AbstractTag");
		}
		else
		{
			//Debug.Log ("NOT addTagToObject");
		}

		curTag=newObject.GetComponent("AbstractTag") as AbstractTag;
		curTag.addFactory(this);
	}
}
