using System.Collections;
using QFramework;
using UnityEngine;

namespace Jiang.Games
{
    public partial class Player : ViewController, ICanGetSystem
    {
        private SaveSystem _SaveSystem;

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
            m_StopDistance = NavMeshAgent.stoppingDistance;
        }

        private void OnEnable()
        {
            MouseManager.Instance.OnMouseClicked += MoveToTargetEvent;
            MouseManager.Instance.OnEnemyClicked += MoveToAttackTargetEvent;
            GameManager.Instance.RegisterPlayer(CharacterStats);
        }

        private void Start()
        {
            _SaveSystem = this.GetSystem<SaveSystem>();
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
            m_bIsDead = CharacterStats.CurrentHealth == 0;

            if (m_bIsDead)
                GameManager.Instance.NotifyObservers();

            SwitchAnimation();

            m_LastAttackTime -= Time.deltaTime;
        }

        private void SwitchAnimation()
        {
            Animator.SetFloat(Speed, NavMeshAgent.velocity.sqrMagnitude);
            Animator.SetBool(Death, m_bIsDead);
        }

        private void MoveToTargetEvent(Vector3 theTargetPoint)
        {
            StopAllCoroutines();
            if (m_bIsDead) return;

            NavMeshAgent.stoppingDistance = m_StopDistance;
            NavMeshAgent.isStopped = false;
            NavMeshAgent.destination = theTargetPoint;
        }

        private void MoveToAttackTargetEvent(GameObject theAttackTarget)
        {
            if (m_bIsDead) return;

            if (theAttackTarget == null) return;

            m_AttackTarget = theAttackTarget;
            CharacterStats.isCritical = Random.value < CharacterStats.attackData.criticalChance;
            StartCoroutine(MoveToAttackTarget());
        }

        private IEnumerator MoveToAttackTarget()
        {
            NavMeshAgent.isStopped = false;
            NavMeshAgent.stoppingDistance = CharacterStats.attackData.attackRange;

            transform.LookAt(m_AttackTarget.transform);

            if (m_AttackTarget == null)
                yield break;

            while (Vector3.Distance(m_AttackTarget.transform.position, transform.position) >
                CharacterStats.attackData.attackRange)
            {
                NavMeshAgent.destination = m_AttackTarget.transform.position;
                yield return null;
            }

            NavMeshAgent.isStopped = true;

            if (m_LastAttackTime < 0)
            {
                Animator.SetBool(Critical, CharacterStats.isCritical);
                Animator.SetTrigger(Attack);
                m_LastAttackTime = CharacterStats.attackData.coolDown;
            }
        }

        //Animation Event
        public void Hit()
        {
            var targetStats = m_AttackTarget.GetComponent<CharacterStats>();
            targetStats.TakeDamage(CharacterStats, targetStats);
        }

        public IArchitecture GetArchitecture() => Global.Interface;
    }
}