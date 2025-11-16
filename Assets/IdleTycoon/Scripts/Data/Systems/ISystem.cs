namespace IdleTycoon.Scripts.Data.Systems
{
    public interface ISystem
    {
        void Init();

        void OnTick();
    }
}