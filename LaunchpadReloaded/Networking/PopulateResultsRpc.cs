using Hazel;
using LaunchpadReloaded.Features;
using LaunchpadReloaded.Features.Managers;
using Reactor.Networking.Attributes;
using Reactor.Networking.Rpc;
using System.Linq;
using LaunchpadReloaded.Features.Voting;

namespace LaunchpadReloaded.Networking;

[RegisterCustomRpc((uint)LaunchpadRpc.PopulateResults)]
public class PopulateResultsRpc(LaunchpadReloadedPlugin plugin, uint id)
    : PlayerCustomRpc<LaunchpadReloadedPlugin, PopulateResultsRpc.Data>(plugin, id)
{
    public override RpcLocalHandling LocalHandling => RpcLocalHandling.After;

    public readonly struct Data(CustomVote[] votes)
    {
        public readonly CustomVote[] Votes = votes;
    }

    public override void Write(MessageWriter writer, Data data)
    {
        writer.WritePacked((uint)data.Votes.Length);
        foreach (var t in data.Votes)
        {
            writer.Write(t.Voter);
            writer.Write(t.Suspect);
        }
    }

    public override Data Read(MessageReader reader)
    {
        var votes = new CustomVote[reader.ReadPackedUInt32()];
        
        for (var i = 0; i < votes.Length; i++)
        {
            votes[i] = new CustomVote(reader.ReadByte(), reader.ReadByte());
        }

        return new Data(votes);
    }

    public override void Handle(PlayerControl player, Data data)
    {
        VotingTypesManager.HandlePopulateResults(data.Votes.ToList());
    }
}