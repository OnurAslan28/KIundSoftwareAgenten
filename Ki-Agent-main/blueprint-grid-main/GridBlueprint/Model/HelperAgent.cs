using System;
using Mars.Interfaces.Agents;
using Mars.Interfaces.Environments;

namespace GridBlueprint.Model;

public class HelperAgent : IAgent<DrawLayer>, IPositionable
{
    public void Init(DrawLayer layer)
    {
        _layer = layer;
    }

    public void Tick()
    {
        // The simulation sends layer data to the visualization web socket only when a change in layer data occurs.
        // Therefore, we make an initial layer access to send layer data to web socket for visualization.
        _layer[0, 0] = _layer[0, 0];
    }

    public Guid ID { get; set; }
    public Position Position { get; set; }
    private DrawLayer _layer;
}