using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;

public class PoolManager : Singleton<PoolManager>
{
    [System.Serializable]
    public class ObjectInfo
    {
        // 오브젝트 이름
        public string objectName;
        // 오브젝트 풀에서 관리할 오브젝트
        public GameObject perfab;
        // 몇개를 미리 생성 해놓을건지
        public int count;
    }
    // 오브젝트풀 매니저 준비 완료표시
    public bool IsReady { get; private set; }

    [SerializeField]
    private ObjectInfo[] _objectInfos = null;

    // 생성할 오브젝트의 key값지정을 위한 변수
    private string _objectName;

    // 오브젝트풀들을 관리할 딕셔너리
    private Dictionary<string, IObjectPool<GameObject>> _ojbectPoolDic = new Dictionary<string, IObjectPool<GameObject>>();

    // 오브젝트풀에서 오브젝트를 새로 생성할때 사용할 딕셔너리
    private Dictionary<string, GameObject> _goDic = new Dictionary<string, GameObject>();

    private Dictionary<string, Transform> _parentObjects = new Dictionary<string, Transform>();

    protected override void Awake()
    {
        base.Awake();
        Init();
    }

    private void Init()
    {
        IsReady = false;

        for (int idx = 0; idx < _objectInfos.Length; idx++)
        {
            IObjectPool<GameObject> pool = new ObjectPool<GameObject>(CreatePooledItem, OnTakeFromPool, OnReturnedToPool,
            OnDestroyPoolObject, true, _objectInfos[idx].count, _objectInfos[idx].count);

            if (_goDic.ContainsKey(_objectInfos[idx].objectName))
            {
                return;
            }

            _goDic.Add(_objectInfos[idx].objectName, _objectInfos[idx].perfab);
            _ojbectPoolDic.Add(_objectInfos[idx].objectName, pool);

            // 상위 오브젝트 생성
            GameObject parentObject = new GameObject(_objectInfos[idx].objectName + "_Parent");
            _parentObjects.Add(_objectInfos[idx].objectName, parentObject.transform);

            // 미리 오브젝트 생성 해놓기
            for (int i = 0; i < _objectInfos[idx].count; i++)
            {
                _objectName = _objectInfos[idx].objectName;
                PoolAble poolAbleGo = CreatePooledItem().GetComponent<PoolAble>();
                poolAbleGo.Pool.Release(poolAbleGo.gameObject);
            }
        }
        IsReady = true;
    }


    // 생성
    private GameObject CreatePooledItem()
    {
        GameObject poolGo = Instantiate(_goDic[_objectName]);
        poolGo.GetComponent<PoolAble>().Pool = _ojbectPoolDic[_objectName];

        // 부모 오브젝트 설정
        if (_parentObjects.ContainsKey(_objectName))
        {
            poolGo.transform.SetParent(_parentObjects[_objectName]);
        }

        return poolGo;
    }

    // 사용
    private void OnTakeFromPool(GameObject poolGo)
    {
        poolGo.SetActive(true);
    }

    // 반환
    private void OnReturnedToPool(GameObject poolGo)
    {
        poolGo.SetActive(false);
    }

    // 삭제
    private void OnDestroyPoolObject(GameObject poolGo)
    {
        Destroy(poolGo);
    }

    public GameObject GetGameObject(string goName)
    {
        _objectName = goName;
        try
        {
            if (!_goDic.ContainsKey(goName))
            {
                throw new System.NullReferenceException($"{goName} Object Does not Exist in PoolManager");
            }
        }
        catch (System.NullReferenceException ex)
        {
        }
        return _ojbectPoolDic[goName].Get();
    }
}