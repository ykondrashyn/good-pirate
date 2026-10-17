public class Flag : global::UnityEngine.MonoBehaviour
{
	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Material flagMaterial;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Renderer meshRenderer;

	public void SetFlag(global::UnityEngine.Material material)
	{
		global::UnityEngine.Material[] sharedMaterials = meshRenderer.sharedMaterials;
		sharedMaterials[1] = material;
		meshRenderer.sharedMaterials = sharedMaterials;
	}

	private void Awake()
	{
		if (meshRenderer == null)
		{
			meshRenderer = GetComponentInChildren<global::UnityEngine.Renderer>();
		}
	}
}
