namespace Actors
{
    public enum DamageType
    {
        ByPlayer,
        ByAI,
        ByEnvironment,
        ByOther
    }
    
    public interface IDamageable
    {
        void SendDamage(float damage, uint id, DamageType damageType = DamageType.ByOther);
        
        void TakeDamage(float damage, DamageType damageType = DamageType.ByOther);
        
        void TakeDamage(float damage, uint id, DamageType damageType = DamageType.ByOther);
        
        void Die();
        
        void Heal(int amount);
    }
}