using UnityEngine;

[ExecuteInEditMode] // Позволяет обновлять меш в режиме редактирования
public class OctagonDrawer : MonoBehaviour
{
    public float outerRadius = 50f; // Радиус внешнего контура (изменяй в инспекторе)
    public float height = 80f;     // Высота стен
    public Material wallMaterial;   // Материал для стен

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
            meshRenderer.material = wallMaterial; // Устанавливаем материал
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

        Vector3[] vertices = new Vector3[16]; // 8 вершин снизу + 8 сверху
        Vector2[] uv = new Vector2[16]; // UV-координаты для текстур
        int[] triangles = new int[8 * 6]; // 8 граней по 6 индексов (2 треугольника на грань)

        for (int i = 0; i < 8; i++)
        {
            float angle = i * Mathf.PI / 4; // 45° между точками

            // Нижний уровень стен
            vertices[i] = new Vector3(Mathf.Cos(angle) * outerRadius, 0, Mathf.Sin(angle) * outerRadius);
            // Верхний уровень стен
            vertices[i + 8] = new Vector3(Mathf.Cos(angle) * outerRadius, height, Mathf.Sin(angle) * outerRadius);

            // UV-координаты (просто для начала, можно улучшить)
            uv[i] = new Vector2(i / 8f, 0);
            uv[i + 8] = new Vector2(i / 8f, 1);
        }

        int index = 0;
        for (int i = 0; i < 8; i++)
        {
            int next = (i + 1) % 8;

            // Боковая стена (из двух треугольников)
            triangles[index++] = i;
            triangles[index++] = next;
            triangles[index++] = i + 8;

            triangles[index++] = next;
            triangles[index++] = next + 8;
            triangles[index++] = i + 8;
        }

        mesh.vertices = vertices;
        mesh.uv = uv; // Добавляем UV-координаты
        mesh.triangles = triangles;
        mesh.RecalculateNormals(); // Генерируем нормали для освещения
        mesh.RecalculateBounds();

        meshFilter.mesh = mesh;
        meshCollider.sharedMesh = mesh; // Обновляем коллайдер
    }
}
