using System;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 게임진행하며 쭉 살아있는 매니저, 하위 매니저들을 들고있다.
/// </summary>
public class GameManager : Singleton<GameManager>
{

    /// <summary>
    /// Awake 순서 확인하기 DataManager -> GameManager
    /// </summary>
    protected override void Awake()
    {
        _isDontDestroyOnLoad = true;
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
    }

    /// <summary>
    /// DataManager에서 데이터 로드 완료되면 호출
    /// </summary>
    public void Load()
    {

        SetTargetFrame();
    }

    /// <summary>
    /// 모바일 쓰로클링 방지
    /// </summary>
    private void SetTargetFrame()
    {
        Application.targetFrameRate = 30;
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        UIManager.Instance.DestoryAllUIPopup();
        // TODO :
        /*RandomExtension.InitStateFromTicks();*/
    }
}
