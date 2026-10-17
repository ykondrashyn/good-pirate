namespace UnityStandardAssets.Water
{
	public class MeshContainer
	{
		public global::UnityEngine.Mesh mesh;

		public global::UnityEngine.Vector3[] vertices;

		public global::UnityEngine.Vector3[] normals;

		public MeshContainer(global::UnityEngine.Mesh m)
		{
			mesh = m;
			vertices = m.vertices;
			normals = m.normals;
		}

		public void Update()
		{
			mesh.vertices = vertices;
			mesh.normals = normals;
		}
	}
}
