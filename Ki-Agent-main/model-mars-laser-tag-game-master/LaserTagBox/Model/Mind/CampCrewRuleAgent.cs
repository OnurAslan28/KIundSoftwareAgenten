using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using LaserTagBox.Model.Shared;
using Mars.Interfaces.Environments;
using ServiceStack;

namespace LaserTagBox.Model.Mind;

public class CampCrewRuleAgent  : AbstractPlayerMind
{
    private PlayerMindLayer _mindLayer;
    private Position _goal;
    private static Guid _teamLeader;
    private int tikCounter;
    private static bool _barier;
    private static List<Position> _barrierPosison;
    private readonly Random _r = new();
    
    private List<Position> _hills;
    private int leaderMovment;
    private int leaderStatic;
    private static int hasNotSeenEnamisCounter;
    
    public override void Init(PlayerMindLayer mindLayer)
    {
        _mindLayer = mindLayer;
        _barier = false;
        leaderMovment = 0;
        tikCounter = 0;
        hasNotSeenEnamisCounter = 0;
        
        if (_teamLeader == Guid.Empty)//seting a Leader
        {
            _teamLeader = this.ID;
        }
    }
    
    public override void Tick()
    {
        
        
        if (_teamLeader == this.ID)//Leadear movment
        {
            if ((leaderMovment >= 3)||(leaderStatic >= 3))//wee need new Bariers
            {
                _barier = false;
                leaderMovment = 0;
                leaderStatic = 0;
            }
            if(!_barier)//sarch for Bariers
            {
                _barrierPosison = Body.ExploreBarriers1();
                _barier = true;
            }
            mooveLeadear();//Moove Leadear

        }
        else//Crw moovment
        {
            if (tikCounter < _mindLayer.GetCurrentTick())//after a number of thiks select new gole
            {
                calculateNewGole();//finde gole form the barier list
            }
            
            Body.GoTo(_goal);
        }
        
        var enemies = Body.ExploreEnemies1();//sarch for Enemise
        
        if (!enemies.IsEmpty())//enemise found schoot
        {
            Shotting(enemies);
        }
        else//enmeise not found count up
        {
            //Console.WriteLine("No enemy " + hasNotSeenEnamisCounter);
            hasNotSeenEnamisCounter++;
        }

        if (Body.WasTaggedLastTick&&Body.Stance != Stance.Lying)//if hit lay doun
        {
            Body.ChangeStance2(Stance.Lying);
        }
        else// if not hit and laing stand back up
        {
            if (Body.Stance != Stance.Standing)
            {
                Body.ChangeStance2(Stance.Standing);
            }
        }

        if (Body.ActionPoints == 3)// if we habe points still there realode
        {
            if (Body.RemainingShots <= 5)
            {
                Body.Reload3();
            }
        }
    }

    //this methode represenz the way the leadear moves
    private void mooveLeadear()
    {


        if (_mindLayer.GetCurrentTick()>tikCounter)
        {
            calculateNewGole();
            tikCounter += 20;
        }

        if (!Body.GoTo(_goal))
        {
            leaderStatic++;
            //Console.WriteLine("Leader did not move!");
            calculateNewGole(); 
        }
        else
        {
            //Console.WriteLine("Did not move");
            leaderMovment++;
        }
    }

    //this method calculats a gole out out the Barryar List
    private void calculateNewGole()
    {
        //Console.WriteLine(Body.Position.Equals(_goal));
        if (hasNotSeenEnamisCounter > 20 || _barrierPosison.IsEmpty() || _barrierPosison == null)
        {
            _goal = Position.CreatePosition(_mindLayer.Width/2,(_mindLayer.Height/2));
            hasNotSeenEnamisCounter = 0;
        }
        else
        {
            _goal = Position.CreatePosition(_barrierPosison[_r.Next(0, _barrierPosison.Count - 1)].PositionArray);
        }
        
        _goal.Y +=  _r.Next(-1,2);
        _goal.X +=  _r.Next(-1,2);
   
    }
    
    // used when we want to schoot
    private bool Shotting(List<EnemySnapshot> enemies)
    {
        ReloadIfEmpty();
        var hit = Body.Tag5(enemies.First().Position);
        return hit;
    }
    
    // used whene needed to realode
    private void ReloadIfEmpty()
    {
        if (Body.RemainingShots <= 0)
        {
            Body.Reload3();
        }
    }
}