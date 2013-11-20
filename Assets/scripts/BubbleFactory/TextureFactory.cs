using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Фабрика текстур, хранит материалы, так как текстура сама по себе без матераила не живёт,
/// особенно для реализации сетов, очень удобно хранить именно материалы
/// </summary>

public class TextureFactory: Abstract{	
	protected List<Material> objectsList = new List<Material>();
	private int []arrayOfSizes={32,64,128,256};

	public void DeleteCurrent(Material inObject){
		objectsList.Remove(inObject);
		DestroyImmediate(inObject.mainTexture);
		inObject.mainTexture=null;
	}
	
	public void DestroyAll()
	{
		foreach(Material mat in objectsList)
		{
			DestroyImmediate(mat.mainTexture);
			mat.mainTexture=null;
		}
		objectsList.Clear();
	}
	
	//поменять текстуры на материалах
	public void ChangeSet()
	{
		foreach(Material mat in objectsList)
		{
			int size=mat.mainTexture.height;
			//get index
			DestroyImmediate(mat.mainTexture);
			mat.mainTexture=null;
			GetNewObject(size,mat);
		}
	}
	
	//получить размер по индексу
	public int GetSizeForIndex(int indexOfTextureScale)
	{
		return arrayOfSizes[indexOfTextureScale];
	}
	
	//получить новую текстуру на материал
	public virtual void GetNewObject(int size,Material newMaterial){
		Color color1,color2;
		color1=new Color(Random.Range (0,1f),Random.Range (0,1f),Random.Range (0,1f),1);
		color2=new Color(Random.Range (0,1f),Random.Range (0,1f),Random.Range (0,1f),1);
		
		//creating Texture
		Texture2D newTexture=new Texture2D(size,size);
		// Fill the texture with Sierpinski's fractal pattern!
		for (int y=0; y < newTexture.height; ++y) {
			for (int x = 0; x < newTexture.width; ++x) {
				Color color = (x&y)!=0 ? color1: color2;
				newTexture.SetPixel(x, y, color);
			}
		}
		
		// Apply all SetPixel calls
		newTexture.Apply();
		
		//creating Material
		newMaterial.shader=Shader.Find("Mobile/VertexLit");
		newMaterial.mainTexture=newTexture;
		newMaterial.mainTextureScale =new Vector2 (3f,3f);	
		
		if(objectsList.Contains(newMaterial))
		{
			//Debug.Log ("contains");
		}
		else{
			objectsList.Add (newMaterial);
		}
	}
}
