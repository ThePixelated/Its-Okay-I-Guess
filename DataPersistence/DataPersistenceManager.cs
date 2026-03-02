using System;
using System.Collections;
using UnityEngine;

public class DataPersistenceManager : MonoBehaviour
{
    public static DataPersistenceManager Instance;

    [SerializeField] private float saveCountdown;
    //[SerializeField] private const float saveInterval = 10;
    
    private Coroutine _localCor;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        Instance = this;
    }

    private void Start()
    {
        LoadData();

        //saveCountdown = saveInterval;
        _localCor = StartCoroutine(AutoSaveData(saveCountdown));
    }

    public void LoadData()
    {

    }

    public void SaveData()
    {
        StopCoroutine(_localCor);
        _localCor = null;

        SaveDataLogic();
    }

    public IEnumerator AutoSaveData(float time)
    {
        yield return new WaitForSeconds(time);
        SaveDataLogic();

        StopCoroutine(_localCor);
        _localCor = null;
        _localCor = StartCoroutine(AutoSaveData(time));
    }

    private void SaveDataLogic()
    {
        Debug.Log(DateTime.Now + " Saving Data...");
    }
}
