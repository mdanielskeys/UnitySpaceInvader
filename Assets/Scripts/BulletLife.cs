using UnityEngine;

public class BulletLife : MonoBehaviour
{
    public string EnemyTag;
    public Transform explosion;
    private Rigidbody2D _rb2D;
    private GameGridManager _manager;
    public MoveShip _moveShip;
    public AudioClip fireSound;
    private AudioSource _audioSouce;
    public AudioEvent m_EnemyFireEvent;
    public AudioEvent m_PlayerFireEvent;

    // Use this for initialization
    void Start () {
        _rb2D = GetComponent<Rigidbody2D>();
        _manager = GetComponentInParent<GameGridManager>();
        _audioSouce = GetComponent<AudioSource>();
        if (_audioSouce != null && m_EnemyFireEvent != null)
        {
            m_EnemyFireEvent.Play(_audioSouce);
        }
        else if (_audioSouce != null)
        {
            m_PlayerFireEvent.Play(_audioSouce);
        }

        if (EnemyTag == "Player")
        {
            _rb2D.AddForce(new Vector2(0, Random.Range(-3,-5)), ForceMode2D.Impulse);
        }
        else
        {
            _rb2D.AddForce(new Vector2(0, 4), ForceMode2D.Impulse);
        }

    }

    // Update is called once per frame
    void Update () {
        if (_rb2D.transform.position.y > 6f || _rb2D.transform.position.y < -6f)
        {
            if (_manager != null)
            {
                _manager.ReleaseBulletCount();
            }

            if (gameObject.tag == "PlayerFire")
            {
                if (_moveShip != null)
                {
                    _moveShip.BulletCallback();
                }
            }
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == EnemyTag)
        {
            //Debug.Log("Bullet collision");
            var v3 = other.gameObject.transform.position;
            var ebehavior = other.gameObject.GetComponent<EnemyActions>();
            if (ebehavior != null)
            {
                ebehavior.DestroyedByPlayer();
            }

            Instantiate(explosion, v3, Quaternion.identity);
            Destroy(gameObject);
            if (_manager != null)
            {                
                _manager.ReleaseBulletCount();
            }
            if (_moveShip != null)
            {
                _moveShip.BulletCallback();
            }
        }

        if (other.tag == "Player")
        {
            //Debug.Log("Setup out of player view");
            _manager.FlyOutOfView();
        }
    }
}
