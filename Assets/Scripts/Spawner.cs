using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

public class Spawner : MonoBehaviour
{
    [SerializeField] private Cube _cubePrefab;

    [SerializeField] private int _poolCapacity = 10;

    [SerializeField] float _spawnRate = 1.0f;

    private ObjectPool<Cube> _cubePool;

    private bool _isSpawning = true;

    private float _minimumX = -13.0f;
    private float _maximumX = 10.0f;
    private float _positionY = 10.0f;
    private float _positionZ = 0.55f;

    private void Awake()
    {
        _cubePool = new ObjectPool<Cube>(
        createFunc: () => InstantiateCube(),
        actionOnGet: (cube) => ActivateCube(cube),
        actionOnRelease: (cube) => cube.gameObject.SetActive(false),
        actionOnDestroy: (cube) => DestroyCube(cube),
        defaultCapacity: _poolCapacity);
    }

    private void Start()
    {
        StartCoroutine(Spawn());
    }

    private IEnumerator Spawn()
    {
        WaitForSecondsRealtime delayTime = new WaitForSecondsRealtime(_spawnRate);

        while (_isSpawning)
        {
            GetCube();

            yield return delayTime;
        }
    }

    private Cube InstantiateCube()
    {
        Cube cube = Instantiate(_cubePrefab);

        cube.OccuredCollision += Release;

        return cube;
    }

    private void ActivateCube(Cube cube)
    {
        cube.Init(_minimumX, _maximumX, _positionY, _positionZ);
    }

    private void GetCube()
    {
        _cubePool.Get();
    }

    private void DestroyCube(Cube cube)
    {
        cube.OccuredCollision -= Release;
        Destroy(cube);
    }

    private void Release(Cube cube)
    {
        _cubePool.Release(cube);
    }
}
