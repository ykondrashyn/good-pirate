public class ModifiedShadow : global::UnityEngine.UI.Shadow
{
	public override void ModifyMesh(global::UnityEngine.UI.VertexHelper vh)
	{
		if (IsActive())
		{
			global::System.Collections.Generic.List<global::UnityEngine.UIVertex> list = ListPool<global::UnityEngine.UIVertex>.Get();
			vh.GetUIVertexStream(list);
			ModifyVertices(list);
			vh.Clear();
			vh.AddUIVertexTriangleStream(list);
			ListPool<global::UnityEngine.UIVertex>.Release(list);
		}
	}

	public virtual void ModifyVertices(global::System.Collections.Generic.List<global::UnityEngine.UIVertex> verts)
	{
	}
}
