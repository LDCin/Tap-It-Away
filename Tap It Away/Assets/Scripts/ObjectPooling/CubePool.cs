using System.Collections.Generic;
using uPools;
using UnityEngine;

public class CubePool : MonoBehaviour
{
    [SerializeField] private Cube cubePrefab;
    [SerializeField, Min(0)] private int initialSize = 32;
    [SerializeField] private Transform inactiveRoot;

    private readonly HashSet<Cube> pooledCubes = new();
    private ObjectPool<Cube> pool;

    private void Awake()
    {
        if (inactiveRoot == null)
        {
            inactiveRoot = transform;
        }

        InitializePool();
    }

    public Cube GetCube(CubeData cubeData, Transform parent)
    {
        Cube cube = GetInactiveCube();
        if (cube == null)
        {
            return null;
        }

        Transform cubeTransform = cube.transform;
        cubeTransform.SetParent(parent, false);
        cubeTransform.localRotation = Quaternion.identity;
        cubeTransform.localScale = Vector3.one;
        cube.gameObject.SetActive(true);

        CubeMover cubeMover = cube.GetComponent<CubeMover>();
        if (cubeMover != null)
        {
            cubeMover.SetPool(this);
            cubeMover.ResetForSpawn();
        }

        cube.InitByCubeData(cubeData);
        return cube;
    }

    public void ReturnCube(Cube cube)
    {
        if (cube == null)
        {
            return;
        }

        if (pooledCubes.Contains(cube))
        {
            return;
        }

        pooledCubes.Add(cube);
        pool.Return(cube);
    }

    private void InitializePool()
    {
        if (cubePrefab == null)
        {
            Debug.LogError("CubePool: Cube prefab is missing.");
            return;
        }

        pool = new ObjectPool<Cube>(
            CreateCube,
            null,
            ResetAndHideCube,
            cube => Destroy(cube.gameObject)
        );

        pool.Prewarm(initialSize);
    }

    private Cube CreateCube()
    {
        Cube cube = Instantiate(cubePrefab, inactiveRoot);
        cube.gameObject.SetActive(false);
        return cube;
    }

    private Cube GetInactiveCube()
    {
        if (pool == null)
        {
            return null;
        }

        Cube cube = pool.Rent();
        pooledCubes.Remove(cube);
        return cube;
    }

    private void ResetAndHideCube(Cube cube)
    {
        CubeMover cubeMover = cube.GetComponent<CubeMover>();
        if (cubeMover != null)
        {
            cubeMover.ResetForPool();
        }

        cube.ResetForPool();
        cube.transform.SetParent(inactiveRoot, false);
        cube.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        pool?.Dispose();
    }
}
