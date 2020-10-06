using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;


public class ScreenFader : MonoBehaviour
{
    public static ScreenFader sharedInst = default;
    [SerializeField] CanvasGroup cg = default;
    [SerializeField] float fadeInDuration =0.5f;
    [SerializeField] float fadeOutDuration =0.5f;

    [ContextMenu("fadein")]

    void Awake()
    {
        if(sharedInst==null)
            sharedInst = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }
    public void fadeIn()
    {
        cg.DOFade(1,fadeInDuration);
    }

    [ContextMenu("fadeout")]
    public void fadeOut()
    {
        cg.DOFade(0,fadeOutDuration);
    }

    public void LoadLevel(string _nombre)
    {
        StartCoroutine(loadCoroutine(_nombre));
    }

    public void ExitApp(){
        Application.Quit();
    }

    public IEnumerator loadCoroutine(string _level)
    {
        fadeIn();
        yield return new WaitForSeconds(0.6f);
        SceneManager.LoadScene(_level);
        yield return new WaitForSeconds(0.1f);
        fadeOut();
    }
}
