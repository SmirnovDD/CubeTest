using UnityEngine;

public class Destructable : MonoBehaviour, IDestructable
{
    [SerializeField] private float _durability = 100;
    
    public void Damage(float damage)
    {
        _durability -= damage;
        if (_durability <= 0)
            Destroy();
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }
}
