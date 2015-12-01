using UnityEngine;
using System.Collections;

//[RequireComponent(typeof(T))]
//ensure there is a T component attached to this obj
public interface StateBase<T>
{
    bool finish { get; }
    void Enter(T entity);
    void Execute(T entity);
	void Exit(T entity);
}
