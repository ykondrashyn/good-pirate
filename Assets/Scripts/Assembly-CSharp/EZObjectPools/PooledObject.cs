namespace EZObjectPools
{
	[global::UnityEngine.AddComponentMenu("EZ Object Pools/Pooled Object")]
	public class PooledObject : global::UnityEngine.MonoBehaviour
	{
		[global::UnityEngine.HideInInspector]
		public global::EZObjectPools.EZObjectPool ParentPool;

		public virtual void Disable()
		{
			base.gameObject.SetActive(value: false);
		}

		private void OnDisable()
		{
			base.transform.position = global::UnityEngine.Vector3.zero;
			if ((bool)ParentPool)
			{
				ParentPool.AddToAvailableObjects(base.gameObject);
			}
			else
			{
				global::UnityEngine.Debug.LogWarning("PooledObject " + base.gameObject.name + " does not have a parent pool. If this occurred during a scene transition, ignore this. Otherwise reoprt to developer.");
			}
		}
	}
}
