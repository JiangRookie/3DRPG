using UnityEngine;
using UnityEngine.AI;

public enum EnemyStates
{
    GUARD,
    PATROL,
    CHASE,
    DEAD
}

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(CharacterStats))]
[RequireComponent(typeof(HealthBarUI))]
public class EnemyController : MonoBehaviour, IEndGameObserver
{
    private EnemyStates m_EnemyStates;
    private NavMeshAgent m_Agent;
    private Animator m_Animator;
    private Collider m_Collider;

    protected CharacterStats characterStats;
    protected GameObject attackTarget;

    [Header("Basic Settings")] public float sightRange = 6f;
    public bool isGuard;

    private float m_LastAttackTime;

    private float m_InitSpeed;
    private Vector3 m_InitPosition;
    private Quaternion m_InitRotation;

    [Header("巡逻状态")] public float patrolRange = 6f;
    public float patrolTime = 3f;
    private float m_RemainingPatrolTime;
    private Vector3 m_NewRandomPatrolPoint;

    private bool m_bIsWalk;
    private bool m_bIsChase;
    private bool m_bIsFollow;
    private bool m_bIsDead;
    private bool m_bPlayerDead;
    private static readonly int Walk = Animator.StringToHash("Walk");
    private static readonly int Chase = Animator.StringToHash("Chase");
    private static readonly int Follow = Animator.StringToHash("Follow");
    private static readonly int Death = Animator.StringToHash("Death");
    private static readonly int Attack = Animator.StringToHash("Attack");
    private static readonly int Skill = Animator.StringToHash("Skill");
    private static readonly int Critical = Animator.StringToHash("Critical");
    private static readonly int Win = Animator.StringToHash("Win");

    private Transform m_Transform;

    private void Awake()
    {
        m_Agent = GetComponent<NavMeshAgent>();
        m_Animator = GetComponent<Animator>();
        characterStats = GetComponent<CharacterStats>();
        m_Collider = GetComponent<Collider>();

        m_InitSpeed = m_Agent.speed;

        m_Transform = transform;
        m_InitPosition = m_Transform.position;
        m_InitRotation = m_Transform.rotation;
        m_RemainingPatrolTime = patrolTime;
    }

    private void Start()
    {
        if (isGuard)
        {
            m_EnemyStates = EnemyStates.GUARD;
        }
        else
        {
            m_EnemyStates = EnemyStates.PATROL;
            GetNewRandomPatrolPoint();
        }

        //FIXME:场景切换后修改掉
        GameManager.Instance.AddObserver(this);
    }

    //切换场景时启用
    // void OnEnable()
    // {
    //     GameManager.Instance.AddObserver(this);
    // }

    private void OnDisable()
    {
        if (!GameManager.IsInitialized) return;
        GameManager.Instance.RemoveObserver(this);

        if (GetComponent<LootSpawner>() && m_bIsDead)
            GetComponent<LootSpawner>().SpawnLoot();

        if (QuestManager.IsInitialized && m_bIsDead)
            QuestManager.Instance.UpdateQuestProgress(this.name, 1);
    }

    private void Update()
    {
        if (characterStats.CurrentHealth == 0)
            m_bIsDead = true;

        if (m_bPlayerDead) return;
        SwitchStates();
        SwitchAnimation();
        m_LastAttackTime -= Time.deltaTime;
    }

    private void SwitchAnimation()
    {
        m_Animator.SetBool(Walk, m_bIsWalk);
        m_Animator.SetBool(Chase, m_bIsChase);
        m_Animator.SetBool(Follow, m_bIsFollow);
        m_Animator.SetBool(Critical, characterStats.isCritical);
        m_Animator.SetBool(Death, m_bIsDead);
    }


    private void SwitchStates()
    {
        if (m_bIsDead)
            m_EnemyStates = EnemyStates.DEAD;

        //如果发现player 切换到CHASE
        else if (IsFoundPlayer())
        {
            m_Agent.stoppingDistance = characterStats.attackData.attackRange;
            m_EnemyStates = EnemyStates.CHASE;
        }
        else
            m_Agent.stoppingDistance = m_Agent.radius;

        switch (m_EnemyStates)
        {
            case EnemyStates.GUARD:
                m_bIsChase = false;

                if (transform.position != m_InitPosition)
                {
                    m_bIsWalk = true;
                    m_Agent.isStopped = false;
                    m_Agent.destination = m_InitPosition;

                    if (Vector3.SqrMagnitude(m_InitPosition - transform.position) <= m_Agent.stoppingDistance)
                    {
                        m_bIsWalk = false;
                        transform.rotation = Quaternion.Lerp(transform.rotation, m_InitRotation, 0.01f);
                    }
                }

                break;
            case EnemyStates.PATROL:

                m_bIsChase = false;
                m_Agent.speed = m_InitSpeed * 0.5f;

                //判断是否到了随机巡逻点
                if (Vector3.Distance(m_NewRandomPatrolPoint, transform.position) <= m_Agent.stoppingDistance)
                {
                    m_bIsWalk = false;
                    if (m_RemainingPatrolTime > 0)
                        m_RemainingPatrolTime -= Time.deltaTime;
                    else
                        GetNewRandomPatrolPoint();
                }
                else
                {
                    m_bIsWalk = true;
                    m_Agent.destination = m_NewRandomPatrolPoint;
                }

                break;
            case EnemyStates.CHASE:

                m_bIsWalk = false;
                m_bIsChase = true;

                m_Agent.speed = m_InitSpeed;

                if (!IsFoundPlayer())
                {
                    m_bIsFollow = false;
                    if (m_RemainingPatrolTime > 0)
                    {
                        m_Agent.destination = transform.position;
                        m_RemainingPatrolTime -= Time.deltaTime;
                    }

                    else if (isGuard)
                        m_EnemyStates = EnemyStates.GUARD;
                    else
                        m_EnemyStates = EnemyStates.PATROL;
                }
                else
                {
                    m_bIsFollow = true;
                    m_Agent.isStopped = false;
                    //FIXME
                    m_Agent.destination = attackTarget.transform.position;
                }

                //在攻击范围内则攻击
                if (AttackTargetInAttackRange() || AttackTargetInSkillRange())
                {
                    m_bIsFollow = false;
                    m_Agent.isStopped = true;

                    if (m_LastAttackTime < 0)
                    {
                        m_LastAttackTime = characterStats.attackData.coolDown;

                        //暴击判断
                        characterStats.isCritical = Random.value < characterStats.attackData.criticalChance;
                        //执行攻击
                        AttackTarget();
                    }
                }

                break;
            case EnemyStates.DEAD:
                m_Collider.enabled = false;
                // agent.enabled = false;
                m_Agent.radius = 0;
                Destroy(gameObject, 2f);
                break;
        }
    }

    private void AttackTarget()
    {
        transform.LookAt(attackTarget.transform);
        if (AttackTargetInAttackRange())
        {
            m_Animator.SetTrigger(Attack);
        }

        if (AttackTargetInSkillRange())
        {
            m_Animator.SetTrigger(Skill);
        }
    }

    private bool IsFoundPlayer()
    {
        var maxColliders = 20;
        var hitColliders = new Collider[maxColliders];
        var numColliders = Physics.OverlapSphereNonAlloc(transform.position, sightRange, hitColliders);
        for (var i = 0; i < numColliders; i++)
        {
            if (hitColliders[i].CompareTag("Player"))
            {
                attackTarget = hitColliders[i].gameObject;
                return true;
            }
        }

        return false;
        /*var colliders = Physics.OverlapSphere(transform.position, sightRange);

        foreach (var target in colliders)
        {
            if (target.CompareTag("Player"))
            {
                attackTarget = target.gameObject;
                return true;
            }
        }

        attackTarget = null;
        return false;*/
    }

    private bool AttackTargetInAttackRange()
    {
        if (attackTarget != null)
            return Vector3.Distance(attackTarget.transform.position, transform.position) <=
                   characterStats.attackData.attackRange;
        return false;
    }

    private bool AttackTargetInSkillRange()
    {
        if (attackTarget != null)
            return Vector3.Distance(attackTarget.transform.position, transform.position) <=
                   characterStats.attackData.skillRange;
        return false;
    }

    private void GetNewRandomPatrolPoint()
    {
        m_RemainingPatrolTime = patrolTime;

        var randomX = Random.Range(-patrolRange, patrolRange);
        var randomZ = Random.Range(-patrolRange, patrolRange);

        var newRandomPatrolPoint =
            new Vector3(m_InitPosition.x + randomX, transform.position.y, m_InitPosition.z + randomZ);

        m_NewRandomPatrolPoint = NavMesh.SamplePosition(newRandomPatrolPoint, out var hit, patrolRange, 1)
            ? hit.position
            : transform.position;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }

    //Animation Event
    public void Hit()
    {
        if (attackTarget != null && transform.IsFacingTarget(attackTarget.transform))
        {
            var playerStats = attackTarget.GetComponent<CharacterStats>();
            playerStats.TakeDamage(characterStats, playerStats);
        }
    }

    public void EndNotify()
    {
        //获胜动画
        //停止所有移动
        //停止Agent
        m_Animator.SetBool(Win, true);
        m_bPlayerDead = true;
        m_bIsChase = false;
        m_bIsWalk = false;
        attackTarget = null;
    }
}