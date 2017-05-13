using UnityEngine;
using System.Collections;

public interface IPoolableObect {
	bool Initialize(object [] args);
	void UnInitialize();
}
