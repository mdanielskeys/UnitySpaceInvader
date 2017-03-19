using UnityEngine;

public class RemoveWhenDone : MonoBehaviour
{

    private Animator animator;

	// Use this for initialization
	void Start ()
	{
	    animator = GetComponent<Animator>();
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
