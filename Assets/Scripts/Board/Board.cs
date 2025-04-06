using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter)), RequireComponent(typeof(MeshRenderer))]
public class Board : MonoBehaviour
{
    [SerializeField] private int boardSize = 8;
    [SerializeField] private float cellSize = 1f;
    [SerializeField] private Material evenCellMaterial; // Материал для четных клеток
    [SerializeField] private Material oddCellMaterial;  // Материал для нечетных клеток

    void Start()
    {
        GenerateBoard();
    }

    void GenerateBoard()
    {
        Mesh boardMesh = new Mesh();

        Vector3[] vertices = new Vector3[(boardSize + 1) * (boardSize + 1)];
        Vector2[] uv = new Vector2[vertices.Length]; // Добавляем UV-координаты
        int[] evenTriangles = new int[boardSize * boardSize * 6 / 2]; // Треугольники для четных клеток
        int[] oddTriangles = new int[boardSize * boardSize * 6 / 2];  // Треугольники для нечетных клеток

        float offset = (boardSize * cellSize) / 2f;

        // Генерация вершин и UV
        for (int z = 0, i = 0; z <= boardSize; z++)
        {
            for (int x = 0; x <= boardSize; x++, i++)
            {
                vertices[i] = new Vector3(
                    x * cellSize - offset,
                    z * cellSize - offset,
                    0);

                // UV-координаты для текстурирования
                uv[i] = new Vector2((float)x / boardSize, (float)z / boardSize);
            }
        }

        int evenIndex = 0;
        int oddIndex = 0;

        // Разделяем треугольники на четные и нечетные клетки
        for (int z = 0; z < boardSize; z++)
        {
            for (int x = 0; x < boardSize; x++)
            {
                int vertIndex = z * (boardSize + 1) + x;
                int[] currentTriangles = ((x + z) % 2 == 0) ? evenTriangles : oddTriangles;
                int currentIndex = ((x + z) % 2 == 0) ? evenIndex : oddIndex;

                currentTriangles[currentIndex++] = vertIndex;
                currentTriangles[currentIndex++] = vertIndex + boardSize + 1;
                currentTriangles[currentIndex++] = vertIndex + 1;
                currentTriangles[currentIndex++] = vertIndex + 1;
                currentTriangles[currentIndex++] = vertIndex + boardSize + 1;
                currentTriangles[currentIndex++] = vertIndex + boardSize + 2;

                if ((x + z) % 2 == 0)
                    evenIndex = currentIndex;
                else
                    oddIndex = currentIndex;
            }
        }

        // Назначаем данные мешу
        boardMesh.vertices = vertices;
        boardMesh.uv = uv; // Добавляем UV-координаты

        // Создаем подмеши
        boardMesh.subMeshCount = 2;
        boardMesh.SetTriangles(evenTriangles, 0);
        boardMesh.SetTriangles(oddTriangles, 1);

        boardMesh.RecalculateNormals();
        boardMesh.RecalculateBounds();

        GetComponent<MeshFilter>().mesh = boardMesh;

        // Назначаем материалы
        GetComponent<MeshRenderer>().materials = new Material[] { evenCellMaterial, oddCellMaterial };
    }
}