using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CollectingCoin : MonoBehaviour
{
    [SerializeField] private Button _spawnButton;
    [SerializeField] private Transform _spawnPoint;
    [SerializeField] private Transform _endPoint;
    [SerializeField] private TextMeshProUGUI _coinText;

    [Header("Coin Pool")]
    [SerializeField] private GameObject _coinPrefab;
    [SerializeField] private Transform _coinParent;
    [SerializeField] private int _initialPoolSize = 20;

    [Header("Spawn Settings")]
    [SerializeField] private int _spawnAmount = 10;
    [SerializeField] private Vector2 _xRange;
    [SerializeField] private Vector2 _yRange;
    [SerializeField] private float _collectDuration = 1f;
    [SerializeField] private float _delayBeforeCollect = 0.1f;
    [SerializeField] private float _delayBetweenCoins = 0.1f;

    [Header("React Settings")]
    [SerializeField] private float _punchDuration = 0.2f;
    [SerializeField] private float _punchStrength = 0.5f;

    private List<GameObject> _coinList = new List<GameObject>();
    private CancellationTokenSource _ct;
    private Tween _reactTween;
    private int _collectedCoins = 0;
    private Queue<GameObject> _coinPool = new Queue<GameObject>();

    void Start()
    {
        InitPool();
        _spawnButton?.onClick.AddListener(() => SpawnCoinsAsync().Forget());
    }

    private void OnDestroy()
    {
        _ct?.Cancel();
        _ct?.Dispose();

        while (_coinPool.Count > 0)
        {
            GameObject coin = _coinPool.Dequeue();
            Destroy(coin);
        }
    }

    private void InitPool()
    {
        for (int i = 0; i < _initialPoolSize; i++)
        {
            GameObject coin = Instantiate(_coinPrefab, _coinParent);
            coin.SetActive(false);
            _coinPool.Enqueue(coin);
        }
    }

    private GameObject GetCoinFromPool()
    {
        GameObject coin = _coinPool.Count > 0 ? _coinPool.Dequeue() : Instantiate(_coinPrefab, _coinParent);
        coin.SetActive(true);
        return coin;
    }

    private void ReturnCoinToPool(GameObject coin)
    {
        coin.SetActive(false);
        _coinPool.Enqueue(coin);
    }


    public async UniTask SpawnCoinsAsync()
    {
        _ct?.Cancel(); // Cancel any ongoing animations
        _ct = new CancellationTokenSource();

        foreach (var coin in _coinList)
        {
            Destroy(coin);
        }
        _coinList.Clear();
        SetCoinAmount(0);

        List<UniTask> spawnTasks = new List<UniTask>();
        for (int i = 0; i < _spawnAmount; i++)
        {
            GameObject coin = GetCoinFromPool();
            Vector3 randomPos = new Vector3(Random.Range(_xRange.x, _xRange.y), Random.Range(_yRange.x, _yRange.y));
            coin.transform.position = _spawnPoint.position + randomPos;
            spawnTasks.Add(coin.transform.DOPunchPosition(new Vector3(0, 30, 0), Random.Range(0, 1f)).SetEase(Ease.InOutElastic).ToUniTask(cancellationToken: _ct.Token));
            _coinList.Add(coin);
        }

        await UniTask.WhenAll(spawnTasks);
        await UniTask.Delay(System.TimeSpan.FromSeconds(_delayBeforeCollect), cancellationToken: _ct.Token);

        List<UniTask> moveTasks = new List<UniTask>();
        for (int i = _coinList.Count - 1; i >= 0; i--) // Move from last to first to prevent out of range
        {
            moveTasks.Add(MoveCoinAsync(_coinList[i]));

            if (i < _coinList.Count - 1)
            {
                await UniTask.Delay(System.TimeSpan.FromSeconds(_delayBetweenCoins), cancellationToken: _ct.Token);
            }
        }
    }

    private async UniTask MoveCoinAsync(GameObject coin)
    {
        await coin.transform.DOMove(_endPoint.position, _collectDuration).SetEase(Ease.InBack).ToUniTask(cancellationToken: _ct.Token);
        ReturnCoinToPool(coin);
        _coinList.Remove(coin);
        await ReactToCoinCollected();
        SetCoinAmount(_collectedCoins + 1);
    }

    private async UniTask ReactToCoinCollected()
    {
        if (_reactTween == null) // We prevent overlapping tween
        {
            _endPoint.DOKill(true);
            _reactTween = _endPoint.DOPunchScale(Vector3.one * _punchStrength, _punchDuration).SetEase(Ease.InOutElastic);
            await _reactTween.ToUniTask();
            _reactTween = null;
        }
    }

    private void SetCoinAmount(int amount)
    {
        _collectedCoins = amount;
        _coinText.text = _collectedCoins.ToString();
    }
}
