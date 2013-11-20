using UnityEngine;
using System.Collections;
using  System.Globalization;

/// <summary>
/// Тег объекта, лежащего в фабрике, автоматически вешается
/// на каждый объект, генерируемый фабрикой, если уже не висит
/// </summary>

public class AbstractTag : Abstract{	
	protected AbstractElementFactory abstractElementFactory;
	
	//добавить фабрику, к которой привязан объект
	public void addFactory(AbstractElementFactory inabstractElementFactory){
		abstractElementFactory=inabstractElementFactory;
	}
	
	//пометить объект в пуле объектов как неиспользуемый
	public virtual void DeleteFromUsed(){
		abstractElementFactory.DeleteCurrent(this);
	}
	
	//сделать объект неактивным
	public virtual void MakeInactive(){
		//do nothing
	}
}