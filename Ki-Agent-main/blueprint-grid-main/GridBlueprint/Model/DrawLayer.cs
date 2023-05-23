using System.Collections.Generic;
using System.Linq;
using Mars.Components.Environments;
using Mars.Components.Layers;
using Mars.Core.Data;
using Mars.Interfaces.Data;
using Mars.Interfaces.Layers;

namespace GridBlueprint.Model;

public class DrawLayer: RasterLayer
{
    public override bool InitLayer(LayerInitData layerInitData, RegisterAgent registerAgentHandle,
        UnregisterAgent unregisterAgentHandle)
    {
        var initLayer = base.InitLayer(layerInitData, registerAgentHandle, unregisterAgentHandle);

        AntEnvironment = new SpatialHashEnvironment<AntAgent>(Width, Height);
        var agentManager = layerInitData.Container.Resolve<IAgentManager>();

        AntAgents = agentManager.Spawn<AntAgent, DrawLayer>().ToList();
        HelperAgents = agentManager.Spawn<HelperAgent, DrawLayer>().ToList();
        return initLayer;
    }
    
    public SpatialHashEnvironment<AntAgent> AntEnvironment { get; set; }
    public List<AntAgent> AntAgents { get; private set; }
    public List<HelperAgent> HelperAgents { get; private set; }
}