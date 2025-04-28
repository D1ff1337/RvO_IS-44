using UnityEngine;

[ExecuteInEditMode] 
public class OctagonDrawer : MonoBehaviour
{
    public float outerRadius = 50f; 
    public float height = 80f;     
    public Material wallMaterial;  

    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private MeshCollider meshCollider;

    void Awake()
    {
        meshFilter = GetComponent<MeshFilter>() ?? gameObject.AddComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>() ?? gameObject.AddComponent<MeshRenderer>();
        meshCollider = GetComponent<MeshCollider>() ?? gameObject.AddComponent<MeshCollider>();

        if (wallMaterial != null)
        {
            meshRenderer.material = wallMaterial; 
        }

        GenerateMesh();
    }

    void Update()
    {
        GenerateMesh();
    }

    void GenerateMesh()
    {
        Mesh mesh = new Mesh();

        Vector3[] vertices = new Vector3[16]; 
        Vector2[] uv = new Vector2[16]; 
        int[] triangles = new int[8 * 6]; 

        for (int i = 0; i < 8; i++)
        {
            float angle = i * Mathf.PI / 4; 

            
            vertices[i] = new Vector3(Mathf.Cos(angle) * outerRadius, 0, Mathf.Sin(angle) * outerRadius);
            
            vertices[i + 8] = new Vector3(Mathf.Cos(angle) * outerRadius, height, Mathf.Sin(angle) * outerRadius);

            
            uv[i] = new Vector2(i / 8f, 0);
            uv[i + 8] = new Vector2(i / 8f, 1);
        }

        int index = 0;
        for (int i = 0; i < 8; i++)
        {
            int next = (i + 1) % 8;

            
            triangles[index++] = i;
            triangles[index++] = next;
            triangles[index++] = i + 8;

            triangles[index++] = next;
            triangles[index++] = next + 8;
            triangles[index++] = i + 8;
        }

        mesh.vertices = vertices;
        mesh.uv = uv; 
        mesh.triangles = triangles;
        mesh.RecalculateNormals(); 
        mesh.RecalculateBounds();

        meshFilter.mesh = mesh;
        meshCollider.sharedMesh = mesh; 
    }
}
