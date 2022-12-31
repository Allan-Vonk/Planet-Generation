using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkGenerator : MonoBehaviour
{
    [SerializeField] GameObject ChunkPrefab;
    #region Testgeneration
    [SerializeField] Vector3 size;
    int width;
    int height;
    #endregion
    public List<GameObject> chunks;
    private void Start()
    {
        chunks = new List<GameObject>();
        width = ChunkPrefab.GetComponent<NewChunk>().width;
        height = ChunkPrefab.GetComponent<NewChunk>().height;
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                for (int z = 0; z < size.z; z++)
                {
                    chunks.Add(Instantiate(ChunkPrefab, new Vector3(x * width, y * height, z * width) + transform.position, Quaternion.identity));
                }
            }
        }
    }
}
