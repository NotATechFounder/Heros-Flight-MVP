namespace HeroesFlight.System.ShrineSystem
{
    public interface ShrineSystemInterface : SystemInterface
    {
        Shrine Shrine { get; }
        void UnlockNpc(ShrineNPCType npcType);
    }
}