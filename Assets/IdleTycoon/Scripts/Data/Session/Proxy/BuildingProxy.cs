namespace IdleTycoon.Scripts.Data.Session.Proxy
{
    public readonly struct BuildingProxy
    {
        public readonly TileProxy tile;
        public readonly int id;

        public BuildingProxy(TileProxy tile, int id)
        {
            this.tile = tile;
            this.id = id;
        }
    }
}