
namespace HeroesFlight.System.Inventory
{
    public interface InventorySystemInterface : SystemInterface
    {
        InventoryHandler InventoryHandler { get; }
        void InitConnections();
    }
}