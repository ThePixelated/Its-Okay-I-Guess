using UnityEngine;
using System;
using UnityEngine.SceneManagement;

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
        if (anim != null)
            anim.SetTrigger(triggerSet);
        else
            Debug.LogWarning("anim null le");
        // Kita mulai coroutine buat nunggu durasi animasi
        //StartCoroutine(WaitAndNotify(objType));
    }

    
}
