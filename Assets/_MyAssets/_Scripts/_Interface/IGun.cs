public interface IGun
{
    public void Init();
    public bool Shoot();
    public bool CanShoot();
    public void Reload();
    public void AddAmmo(int amount);
}