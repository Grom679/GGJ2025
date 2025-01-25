using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [SerializeField]
    private bool _isAuto;
    [SerializeField]
    private float _time;
    [SerializeField]
    private int _scene;

    private void Start()
    {
        if (_isAuto) 
        {
            StartCoroutine(WaitToLoad());
        }
    }

    public void LoadScene()
    {
        SceneManager.LoadScene(_scene);
    }

    private IEnumerator WaitToLoad()
    {
        yield return new WaitForSeconds(_time);

        LoadScene();
    }
}
