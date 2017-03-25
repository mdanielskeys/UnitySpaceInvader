using UnityEngine;

public class RemoveWhenDone : MonoBehaviour
{

    private Animator animator;
    public AudioEvent explosionAudioEvent;

	// Use this for initialization
	void Start ()
	{
	    animator = GetComponent<Animator>();
	    var audio = gameObject.GetComponent<AudioSource>();
        explosionAudioEvent.Play(audio);
	}
	
	// Update is called once per frame
	void Update ()
	{
	    var asi = animator.GetCurrentAnimatorStateInfo(0);
        if (asi.normalizedTime >= 1)
	    {
	        Destroy(gameObject);
	    }
	}
}
