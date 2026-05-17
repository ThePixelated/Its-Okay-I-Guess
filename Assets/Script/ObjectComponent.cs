using UnityEngine;
using System;

public class ObjectComponent : MonoBehaviour
{
    [SerializeField] private bool isTriggerEnable;
    [SerializeField] private Sprite defaultSprite;
    [SerializeField] private Animator anim;

    private SpriteRenderer _spriteRenderer;

    //public Action OnAnimationFinished;

    private void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetDefaultSprite()
    {
        if (defaultSprite != null)
        {
            _spriteRenderer.sprite = defaultSprite;
        }
        else
            Debug.LogWarning("defaultSprite null");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isTriggerEnable)
        {
            if (collision.tag == "Player")
            {
                //Debug.Log("Enter");
                if (anim != null)
                    PlayTriggerAnim("Start");
                else
                    Debug.LogWarning("anim null le");
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (isTriggerEnable)
        {
            if (collision.tag == "Player")
            {
                //Debug.Log("Exit");
                if (anim != null)
                    PlayTriggerAnim("Exit");

                else
                    Debug.LogWarning("anim null le");
            }
        }
    }

    public void PlayTriggerAnim(string triggerSet, ObjectiveType objType = ObjectiveType.Null)
    {
        anim.SetTrigger(triggerSet);
        // Kita mulai coroutine buat nunggu durasi animasi
        StartCoroutine(WaitAndNotify(objType));
    }

    private System.Collections.IEnumerator WaitAndNotify(ObjectiveType objType)
    {
        // Nunggu satu frame biar Animator-nya update ke state baru
        yield return null;

        // Ambil durasi animasi yang sedang jalan sekarang
        float duration = anim.GetCurrentAnimatorStateInfo(0).length;

        yield return new WaitForSeconds(duration + 2);

        if (objType == ObjectiveType.EndLocation)
        {
            UIManager.Instance.fadeImage.StartFadeIn();
        }
        
        // Kasih tau siapa pun yang dengerin kalau animasinya kelar
        //OnAnimationFinished?.Invoke();
    }
}
