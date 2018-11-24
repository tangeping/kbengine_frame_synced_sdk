using System;
using KBEngine;

public interface IPhysicsManagerBase
{
	void Init();

	void UpdateStep();

	IWorld GetWorld();

	IWorldClone GetWorldClone();

	void RemoveBody(IBody iBody);
}
