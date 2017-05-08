using UnityEngine;

public class PlayerWeaponBehavior : MonoBehaviour
{
    public Transform Explosion;
    public int WeaponBuffer;
    private AudioSource _audioSouce;
    public AudioEvent PlayerFireEvent;
    public float BulletForce;
    private static int WeaponCount = 0;

    // Use this for initialization
    void Start ()
    {
        if (WeaponCount >= WeaponBuffer)
        {
            Destroy(gameObject);
            return;
        }

        WeaponCount += 1;
        var rigidBody = GetComponent<Rigidbody2D>();
        _audioSouce = GetComponent<AudioSource>();
        
        PlayerFireEvent.Play(_audioSouce);

        rigidBody.AddForce(new Vector2(0, BulletForce), ForceMode2D.Impulse);
    }

    // Update is called once per frame
    void Update () {
        if (transform.position.y > 6f)
        {
            WeaponCount -= 1;
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Enemy")
        {
            //Debug.Log("Bullet collision");
            var v3 = other.gameObject.transform.position;
            var ebehavior = other.gameObject.GetComponent<EnemyActions>();
            if (ebehavior != null)
            {
                ebehavior.DestroyedByPlayer();
            }

            Instantiate(Explosion, v3, Quaternion.identity);
            Destroy(gameObject);
            WeaponCount -= 1;
        }
    }
}
