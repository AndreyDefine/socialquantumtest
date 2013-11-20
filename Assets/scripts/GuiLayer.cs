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
	
	private static float lastHeight = 0;
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
		GUILayout.BeginArea(new Rect(10,10,Screen.width-10,30));
		GUILayout.BeginHorizontal();
		
		GUILayout.Label("score", GUILayout.Width(100));
		GUILayout.Label(GlobalOptions.score.ToString(), GUILayout.Width(200));
				
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
	
	private void ChangeSet()
	{
		GameAgent.GetSharedGameAgent().ChangeSet();
	}
	
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
