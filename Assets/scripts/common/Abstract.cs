using UnityEngine;
using System.Collections;

/// <summary>
/// Base class.
/// Содержит кешируемые свойства
/// </summary>
public class Abstract : MonoBehaviour {

	//трансформ
	private Transform _singleTransform = null;
	
    public Transform singleTransform {
        get {
            return _singleTransform;
        }
        protected set {
            _singleTransform = value;
        }
    }
	
	//renderer
	private Renderer _singleRenderer = null;
    public Renderer singleRenderer {
        get {
            return _singleRenderer;
        }
        protected set {
            _singleRenderer = value;
        }
    }
	
	//здесь задаются кешируемые параметры, иначе могут быть глюки
	public virtual void Awake(){
		singleTransform = transform;
		singleRenderer = renderer;
	}
}


