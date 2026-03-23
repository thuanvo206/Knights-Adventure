using Fusion;
using UnityEngine;

public enum CharacterClass
{
    Warrior,
    Mage,
    Archer
}

public struct PlayerMeta: INetworkStruct
{
    public NetworkString<_16> Name;
    public CharacterClass Class; // Bây giờ nó sẽ hết báo đỏ

    public int Hp;
    public int MaxHp;
    public int Mp;
    public int MaxMp;
}

public class PlayerDataManager : NetworkBehaviour
{
    [Networked] public NetworkDictionary<PlayerRef, PlayerMeta> Players => default;

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_SetPlayerMeta(PlayerRef playerRef, PlayerMeta playerMeta)
    {
        Players.Set(playerRef, playerMeta);
    }
    
    public bool TryGetMeta(PlayerRef playerRef, out PlayerMeta playerMeta)
    {
        return Players.TryGet(playerRef, out playerMeta);
    }   
}
