using System.Collections.Generic;
using UnityEngine;

namespace ObjectPool
{
    public class CubePool : Singleton<CubePool>
    {
        [SerializeField] private Cube cubePrefab;
        [SerializeField, Min(0)] private int initialSize = 32;
        [SerializeField] private Transform inactiveRoot;

        private readonly Queue<Cube> pool = new();
        private readonly HashSet<Cube> pooledCubes = new();

        public override void Awake()
        {
            base.Awake();
            if (Instance != this)
            {
                return;
            }

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

            pooledCubes.Remove(cube);

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

            CubeMover cubeMover = cube.GetComponent<CubeMover>();
            if (cubeMover != null)
            {
                cubeMover.ResetForPool();
            }

            cube.ResetForPool();
            cube.transform.SetParent(inactiveRoot, false);
            cube.gameObject.SetActive(false);
            pool.Enqueue(cube);
            pooledCubes.Add(cube);
        }

        private void InitializePool()
        {
            for (int i = 0; i < initialSize; i++)
            {
                Cube cube = CreateCube();
                ReturnCube(cube);
            }
        }

        private Cube CreateCube()
        {
            if (cubePrefab == null)
            {
                Debug.LogError("CubePool: Cube prefab is missing.");
                return null;
            }

            Cube cube = Instantiate(cubePrefab, inactiveRoot);
            cube.gameObject.SetActive(false);
            return cube;
        }

        private Cube GetInactiveCube()
        {
            while (pool.Count > 0)
            {
                Cube cube = pool.Dequeue();
                if (cube != null)
                {
                    return cube;
                }
            }

            return CreateCube();
        }
    }
}
