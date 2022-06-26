public interface IDestructable
{
    float Durability { get; }
    float MaxDurability { get; }
    void Damage(float damage);
    void Destroy();
    bool IsCharacter { get; }
}
