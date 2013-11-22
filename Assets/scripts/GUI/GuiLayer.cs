using System;
using UnityEngine;
using System.Collections;

/// <summary>
/// Класс, который отвечает за вывод GUI
/// Конечно всё должно быть графическое и через свою камеру, возможно через tk2d,
/// но для тестового задания вполне сойдёт корявый стандартный GUI
/// </summary>

//singleton
public class GuiLayer : Abstract {
	
	//флаг загрузки
	public bool flagLoading=true;
	
	//сообщение в гуи
	private string _message="Loading...";
	public  string message {
        get {
            return _message;
        }
		set {
			_message = value;
		}
    }
	
	//для отслеживания изменения размеров экрана
	private static float lastHeight = 0;
	//singleton instance
	private static GuiLayer instance=null;
			
	public static GuiLayer GetSharedGuiLayer()
	{
		if(instance==null)
		{
			instance=(new GameObject()).AddComponent<GuiLayer>();
			instance.name="GuiLayer";
		}
		return instance;	
	}
	
	
	// Use this for initialization
	void Start () {
		//do nothing
	}
	
	void OnGUI()
	{
		if(flagLoading)
		{
			GUI.Label(new Rect(Screen.width/2-100, Screen.height/2, 200, 20), message);
		}
		else
		{
			GUILayout.BeginArea(new Rect(10,10,Screen.width-10,30));
			GUILayout.BeginHorizontal();
			
			GUILayout.Label("score", GUILayout.Width(50));
			GUILayout.Label(GlobalOptions.score.ToString(), GUILayout.Width(50));
			
			GUILayout.Label("difficulty", GUILayout.Width(50));
			GUILayout.Label(GlobalOptions.difficultyLevel.ToString(), GUILayout.Width(200));
					
			GUILayout.EndHorizontal();
			GUILayout.EndArea();
			
			//buttons
			if (GUI.Button(new Rect(Screen.width-120,Screen.height-60,100,40),"Reset"))
			{
				Reset();
			}
			
			if (GUI.Button(new Rect(Screen.width-120,Screen.height-60-50,100,40),"ChangeSet"))
			{
				ChangeSet();
			}
		}
	}
	
	//нажали на кнопку поменять сет
	private void ChangeSet()
	{
		GameAgent.GetSharedGameAgent().ChangeSet();
	}
	
	//нажали на кнопку сбросить игру
	private void Reset()
	{
		GlobalOptions.score=0;
		GlobalOptions.difficultyLevel=1f;
		GameAgent.GetSharedGameAgent().Reset();
	}
	
	// Update is called once per frame
	void Update () {
		//проверить может изменились размеры экрана
		if (lastHeight != Screen.height)
        {
            Cameras.ResetCameraStats();
        }
	}
}
