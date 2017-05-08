using UnityEngine;

public class EnemyWeaponBehavior : MonoBehaviour
{
    public Transform Explosion;
    public AudioClip FireSound;
    public AudioEvent EnemyFireEvent;
    public int MaxWeaponCount;
    [MinMaxRange(-3, -5)] public RangedFloat WeaponForce;
    private AudioSource _audioSouce;

    private static int _weaponCount = 0;

    // Use this for initialization
    void Start () {
        if (_weaponCount >= MaxWeaponCount)
        {
            Destroy(gameObject);
            return;
        }
        _weaponCount += 1;
        var rigidBody = GetComponent<Rigidbody2D>();
        _audioSouce = GetComponent<AudioSource>();
        EnemyFireEvent.Play(_audioSouce);

        rigidBody.AddForce(new Vector2(0, Random.Range(WeaponForce.minValue, WeaponForce.maxValue)), ForceMode2D.Impulse);
    }

    // Update is called once per frame
    void Update () {
        if (!(transform.position.y < -6f)) return;
        _weaponCount -= 1;
        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Component other)
    {
        if (other.tag != "Player") return;
        Destroy(gameObject);
        _weaponCount -= 1;
    }
}
