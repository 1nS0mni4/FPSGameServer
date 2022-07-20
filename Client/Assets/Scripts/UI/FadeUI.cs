using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeUI : MonoBehaviour
{
    public Image _fadeImage = null;
    /**********************************************************
     * 
     *              Fade In : True / Fade Out : False         *
     * 
     **********************************************************/
    private WaitForSeconds _fadeTime = new WaitForSeconds(0.05f);
    [SerializeField]
    private Color color = Color.black;

    private void Awake() {
        _fadeImage = GetComponent<Image>();
    }

    /// <summary>
    /// Control Fade value
    /// </summary>
    /// <param name="fadeType">Fade In : True / Fade Out : False</param>
    public void FadeControlTo(bool fadeType) {

        StartCoroutine(CoFade(fadeType ? 1.0f : 0.0f));
    }

    private IEnumerator CoFade(float value) {
        _fadeImage.enabled = true;

        color.a = Mathf.Abs(value - 1);

        while(true) {
            color.a = Mathf.Lerp(color.a, value, 0.05f);
            _fadeImage.color = color;

            if(Mathf.Abs(color.a - value) <= 0.005f) {
                color.a = value;
                _fadeImage.color = color;
                break;
            }

            yield return _fadeTime;
        }
        _fadeImage.enabled = false;

        yield break;
    }

}
