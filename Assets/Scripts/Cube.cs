using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(Rigidbody))]
public class Cube : MonoBehaviour
{
    [SerializeField] private float _minDestroyDelay = 2;
    [SerializeField] private float _maxDestroyDelay = 5;

    private Rigidbody _rigidbody;
    private Renderer _renderer;

    private Color _baseColor = Color.white;

    private bool _isCollided;
    private bool _isColorChanged;

    private float _destroyDelay;

    public event Action<Cube> OccuredCollision;

    private void Awake()
    {
        _renderer = GetComponent<MeshRenderer>();
        _rigidbody = GetComponent<Rigidbody>();

        SetCollided();
        _isColorChanged = false;

        _renderer.material.color = _baseColor;

        _destroyDelay = Random.Range(_minDestroyDelay, _maxDestroyDelay + 1);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (_isCollided)
            return;

        if (collision.gameObject.TryGetComponent<Platform>(out _))
            StartCoroutine(CounterEvent());
    }

    public void SetCollided()
    {
        _isCollided = true;
    }

    public void SetUncollided()
    {
        _isCollided = false;
    }

    public void Init(float minimumPositionX, float maximumPositionX, float positionY, float positionZ)
    {
        transform.position = new Vector3(Random.Range(minimumPositionX, maximumPositionX), positionY, positionZ);
        _rigidbody.linearVelocity = Vector3.zero;
        transform.rotation = Quaternion.identity;
        SetUncollided();
        gameObject.SetActive(true);
    }

    private IEnumerator CounterEvent()
    {
        WaitForSecondsRealtime waitTime = new WaitForSecondsRealtime(_destroyDelay);

        ChangeColor();

        yield return waitTime;

        OccuredCollision?.Invoke(this);

        _isColorChanged = false;

        _renderer.material.color = _baseColor;
    }

    private void ChangeColor()
    {
        if (_isColorChanged == false)
        {
            _renderer.material.color = Random.ColorHSV();
            _isColorChanged = true;
        }
    }
}
