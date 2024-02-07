[System.Serializable]
public class BaseStats
{
    public string name;
    public int attackDamage;
    public int hp;
    public float speed;

    public BaseStats(string name, int attackDamage, int hp, float speed)
    {
        this.name = name;
        this.attackDamage = attackDamage;
        this.hp = hp;
        this.speed = speed;
    }

    // Methods for modifying these stats, if needed
}
