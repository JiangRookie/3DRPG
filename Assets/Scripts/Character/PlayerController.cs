using System.Collections;
using Jiang.Games;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    private SaveSystem _SaveSystem;
    private NavMeshAgent m_Agent;
    private Animator m_Animator;
    private CharacterStats m_CharacterStats;

    private GameObject m_AttackTarget;
    private float m_LastAttackTime;
    private bool m_bIsDead;
    private float m_StopDistance;

    private static readonly int Speed = Animator.StringToHash("Speed");
    private static readonly int Death = Animator.StringToHash("Death");
    private static readonly int Critical = Animator.StringToHash("Critical");
    private static readonly int Attack = Animator.StringToHash("Attack");

    private void Awake()
    {
        m_Agent = GetComponent<NavMeshAgent>();
        m_Animator = GetComponent<Animator>();
        m_CharacterStats = GetComponent<CharacterStats>();

        m_StopDistance = m_Agent.stoppingDistance;
    }

    private void OnEnable()
    {
        MouseManager.Instance.OnMouseClicked += MoveToTargetEvent;
        MouseManager.Instance.OnEnemyClicked += MoveToAttackTargetEvent;
        GameManager.Instance.RegisterPlayer(m_CharacterStats);
    }

    private void Start()
    {
        _SaveSystem = Global.Interface.GetSystem<SaveSystem>();
        _SaveSystem.LoadPlayerData();
    }

    private void OnDisable()
    {
        if (!MouseManager.IsInitialized) return;
        MouseManager.Instance.OnMouseClicked -= MoveToTargetEvent;
        MouseManager.Instance.OnEnemyClicked -= MoveToAttackTargetEvent;
    }

    private void Update()
    {
        m_bIsDead = m_CharacterStats.CurrentHealth == 0;

        if (m_bIsDead)
            GameManager.Instance.NotifyObservers();

        SwitchAnimation();

        m_LastAttackTime -= Time.deltaTime;
    }

    private void SwitchAnimation()
    {
        m_Animator.SetFloat(Speed, m_Agent.velocity.sqrMagnitude);
        m_Animator.SetBool(Death, m_bIsDead);
    }

    private void MoveToTargetEvent(Vector3 theTargetPoint)
    {
        StopAllCoroutines();
        if (m_bIsDead) return;

        m_Agent.stoppingDistance = m_StopDistance;
        m_Agent.isStopped = false;
        m_Agent.destination = theTargetPoint;
    }

    private void MoveToAttackTargetEvent(GameObject theAttackTarget)
    {
        if (m_bIsDead) return;

        if (theAttackTarget == null) return;

        m_AttackTarget = theAttackTarget;
        m_CharacterStats.isCritical = Random.value < m_CharacterStats.attackData.criticalChance;
        StartCoroutine(MoveToAttackTarget());
    }

    private IEnumerator MoveToAttackTarget()
    {
        m_Agent.isStopped = false;
        m_Agent.stoppingDistance = m_CharacterStats.attackData.attackRange;

        transform.LookAt(m_AttackTarget.transform);

        if (m_AttackTarget == null)
            yield break;

        while (Vector3.Distance(m_AttackTarget.transform.position, transform.position) >
               m_CharacterStats.attackData.attackRange)
        {
            m_Agent.destination = m_AttackTarget.transform.position;
            yield return null;
        }

        m_Agent.isStopped = true;

        if (m_LastAttackTime < 0)
        {
            m_Animator.SetBool(Critical, m_CharacterStats.isCritical);
            m_Animator.SetTrigger(Attack);
            m_LastAttackTime = m_CharacterStats.attackData.coolDown;
        }
    }

    //Animation Event
    public void Hit()
    {
        var targetStats = m_AttackTarget.GetComponent<CharacterStats>();
        targetStats.TakeDamage(m_CharacterStats, targetStats);
    }
}