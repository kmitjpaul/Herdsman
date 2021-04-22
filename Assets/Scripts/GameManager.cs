using System.Collections.Generic;
using System.Linq;
using Enums;
using Events;
using SplineShapeDetection;
using Unity.VisualScripting;
using UnityEngine;
using Random = System.Random;

public class GameManager : MonoBehaviour
{
    private readonly Random _random = new Random();

    private readonly ScoreChangedEvent _scoreChangedEvent = new ScoreChangedEvent();
    private float _agentCollectionRadiusSquared;
    private List<Agent> _agentsOnField;
    private List<Agent> _agentsToDespawn = new List<Agent>();
    private List<Cell> _cells;
    private float _despawnTime;

    private float _spawnTime;

    public float agentCollectionRadius = 1f;
    public AvoidanceManager avoidanceManager;

    public CellsFilter cellsFilter;
    public float despawnDelayTime = 1f;
    public FollowingManager followingManager;
    public Goal goal;
    public uint maxAgentsOnField = 10;

    public uint maxFollowingAgents = 5;
    public PatrollingManager patrollingManager;
    public Agent player;

    public uint score;
    public float spawnDelayTime = 2f;
    public Spawner spawner;
    public SplineClosedShapeDetector splineClosedShapeDetector;
    public UiManager uiManager;

    private void Start()
    {
        _spawnTime = spawnDelayTime;
        _despawnTime = despawnDelayTime;

        _agentCollectionRadiusSquared = agentCollectionRadius * agentCollectionRadius;

        goal.ev.AddListener(GoalCollisionSubscriber);
        _scoreChangedEvent.AddListener(uiManager.OnScoreChange);

        _cells = splineClosedShapeDetector.GetCells();
        _cells = cellsFilter.Filter(_cells);
        followingManager.agents.Add(player.collider);

        Spawn(maxAgentsOnField);
    }

    private void Spawn(uint agentsToSpawn)
    {
        _agentsOnField = spawner.Spawn<Agent>(_cells.OrderBy(x => _random.Next()).Take((int) agentsToSpawn).ToList());
        AssignPatrollingAndAvoidance();
    }

    private void GoalCollisionSubscriber(Collider2D col)
    {
        followingManager.agents.Clear();
        followingManager.agents.Add(player.collider);

        var newAgentsOnField = new List<Agent>();
        foreach (var agent in _agentsOnField)
        {
            if (agent.state != AgentState.Following)
            {
                newAgentsOnField.Add(agent);
                continue;
            }

            if (agent.collider == col)
            {
                agent.state = AgentState.Idling;
                _agentsToDespawn.Add(agent);
                _despawnTime = despawnDelayTime;
                _spawnTime = spawnDelayTime;

                _scoreChangedEvent.Invoke(++score);
            }
            else
            {
                followingManager.agents.Add(agent.collider);
                newAgentsOnField.Add(agent);
            }
        }

        _agentsOnField = newAgentsOnField;
        AssignPatrollingAndAvoidance();
    }

    private void AssignPatrollingAndAvoidance()
    {
        patrollingManager.agents.Clear();
        avoidanceManager.agents.Clear();
        avoidanceManager.agents.Add(player);

        foreach (var agent in _agentsOnField)
        {
            patrollingManager.agents.Add(agent);
            avoidanceManager.agents.Add(agent);
        }
    }

    private void FixedUpdate()
    {
        ProcessSpawning();
        ProcessDespawning();
        ProcessFollowing();
    }

    private void ProcessSpawning()
    {
        _spawnTime = Mathf.Max(0, _spawnTime - Time.deltaTime);

        if (_agentsOnField.Count >= maxAgentsOnField || _spawnTime > 0)
            return;

        var newAgent = spawner.Spawn<Agent>(_cells.OrderBy(x => _random.Next()).Take(1).ToList());

        _agentsOnField.Add(newAgent.First());
        AssignPatrollingAndAvoidance();
        _spawnTime += spawnDelayTime;
    }

    private void ProcessDespawning()
    {
        _despawnTime = Mathf.Max(0, _despawnTime - Time.deltaTime);

        if (_agentsToDespawn.Count == 0 || _despawnTime > 0)
            return;

        _agentsToDespawn.First().GameObject().SetActive(false);
        _agentsToDespawn = _agentsToDespawn.Skip(1).ToList();

        _despawnTime += despawnDelayTime;
    }

    private void ProcessFollowing()
    {
        if (maxFollowingAgents <= followingManager.agents.Count) return;

        foreach (var agent in _agentsOnField)
        {
            if (maxFollowingAgents <= followingManager.agents.Count) return;

            if (!agent.gameObject.activeInHierarchy
                || agent.state == AgentState.Following
                || (agent.rb.position - player.rb.position).sqrMagnitude > _agentCollectionRadiusSquared) continue;

            agent.state = AgentState.Following;

            followingManager.agents.Add(agent.collider);
        }
    }
}