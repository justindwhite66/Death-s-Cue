using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteFade : MonoBehaviour
{
    public float FadeTime = .4f;
    private SpriteRenderer spriteRenderer;
    private Coroutine fadeCoroutine;
    private bool isFading = false;

    void Awake()
    {
        spriteRenderer =GetComponent<SpriteRenderer>();
    }

    public IEnumerator SlowFadeRoutine(){
        isFading = true;
        float elapsedTime = 0;
        float startValue = spriteRenderer.color.a;

        while (elapsedTime < FadeTime){
            if (!isFading) {
                yield break;
            }
        elapsedTime += Time.deltaTime;
        float newAlpha = Mathf.Lerp(startValue, 0f, elapsedTime / FadeTime);
        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, newAlpha);
        yield return null;
        }
        if (isFading){
            Destroy(gameObject);
        }
    }
    public void StopFade(){

        isFading = false;
        if(fadeCoroutine != null){
            StopCoroutine(fadeCoroutine);
            fadeCoroutine = null;
        }

        Color color = spriteRenderer.color;
            color.a = .02f;
            spriteRenderer.color = color;

    }

    public void StartFade(){
        fadeCoroutine = StartCoroutine(SlowFadeRoutine());
    }
}
