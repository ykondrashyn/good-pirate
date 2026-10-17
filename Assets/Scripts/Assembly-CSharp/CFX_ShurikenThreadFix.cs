public class CFX_ShurikenThreadFix : global::UnityEngine.MonoBehaviour
{
	private global::UnityEngine.ParticleSystem[] systems;

	private void OnEnable()
	{
		systems = GetComponentsInChildren<global::UnityEngine.ParticleSystem>();
		global::UnityEngine.ParticleSystem[] array = systems;
		foreach (global::UnityEngine.ParticleSystem obj in array)
		{
			obj.Stop(withChildren: true);
			obj.Clear(withChildren: true);
		}
		StartCoroutine("WaitFrame");
	}

	private global::System.Collections.IEnumerator WaitFrame()
	{
		yield return null;
		global::UnityEngine.ParticleSystem[] array = systems;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Play(withChildren: true);
		}
	}
}
