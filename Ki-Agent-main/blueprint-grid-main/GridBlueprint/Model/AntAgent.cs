using System;
using System.Collections.Generic;
using Mars.Common.Core.Random;
using Mars.Interfaces.Agents;
using Mars.Interfaces.Annotations;
using Mars.Interfaces.Environments;
using Mars.Interfaces.Layers;

namespace GridBlueprint.Model;

public class AntAgent: IAgent<DrawLayer>, IPositionable
{
    public void Init(DrawLayer layer)
    {
        _layer = layer;
        // Position = Position.CreatePosition(RandomHelper.Random.Next(_layer.Width),
        //     RandomHelper.Random.Next(_layer.Height));
        Position = new Position(StartX, StartY);
        _layer.AntEnvironment.Insert(this);
    }

    

    public void Tick()
    {
        Move();
    }
    
    private void RemoveFromSimulation()
    {
        Console.WriteLine($"SimpleAgent {ID} is removing itself from the simulation.");
        _layer.AntEnvironment.Remove(this);
        UnregisterAgentHandle.Invoke(_layer, this);
    }

    public double CurentVal()
    {
        return _layer[Position];
    }

    public void Move()
    {
        if (CurentVal() == 0)
        {
            _layer[Position] = 1;
            _layer.AntEnvironment.MoveTowards(this, Position.Bearing + 90, 1);
            
        }
        else
        {
            _layer[Position] = 0;
            _layer.AntEnvironment.MoveTowards(this, Position.Bearing - 90, 1);
        }
    }

    public Guid ID { get; set; }
    public Position Position { get; set; }
    [PropertyDescription(Name = "StartX")]
    public int StartX { get; set; }
    
    [PropertyDescription(Name = "StartY")]
    public int StartY { get; set; }
    public UnregisterAgent UnregisterAgentHandle { get; set; }
    private DrawLayer _layer;
}
