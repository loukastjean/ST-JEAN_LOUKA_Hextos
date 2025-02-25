using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Ce qui n'est pas autorise
/// --------------------------------------
/// 1. Modifier les noms des fonctions
/// 2. Modifier les parametres acceptes par les fonctions
///
/// Ce qui est autorise
/// --------------------------------------
/// 1. Modifier le contenu des fonctions
/// 2. Creer des surcharges aux fonctiones
/// 
/// </summary>
public class Unite : MonoBehaviour
{
    // [Header("Attributes")]
    // Attributs de l'unite
    public float healthPoints { get; private set; }
    public float maxHealthPoints { get; private set; }
    
    public float movementSpeed { get; private set; }
    
    public Vector2 strength { get; private set; }
    
    public float attackCooldown { get; private set; }
    public float attackDistance { get; private set; }
    public float attackRadius { get; private set; }

    // Timerstamp du moment de l aderniere attaque
    private float lastAttackTs;
    
    // Timestamp du moment de la creation
    private float creationTs;
    
    // Cible que l'unite veut attaquer
    private Unite target; // CA SUCE
    
    public NavMeshAgent agent { get; private set; }
    
    // Equipe
    public Team team { get; private set; }
    
    // Start is called before the first frame update
    void Start()
    {
        AssignAttributes();
        
        agent = GetComponent<NavMeshAgent>();
        
        agent.speed = movementSpeed;
        
        creationTs = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Assigner equipe
    public void SetTeam(Team _team)
    {
        team = _team;
    }

    protected void AssignAttributes()
    {
        maxHealthPoints = 100f;
        healthPoints = maxHealthPoints;
        movementSpeed = 1.5f;
        attackCooldown = 1.5f;
        attackDistance = 05f;
        strength = new Vector2(15f, 20f);
        attackRadius = 1.5f;
    }
    
    // Assigner la cible
    public void SetCible(Unite _target)
    {
        target = _target;
    }
    
    // Donne une destination a l'unite
    public void SetDestination(Vector2 destination)
    {
        agent.SetDestination(destination);
    }

    // Verifier si on peut attaquer
    public bool AttackReady()
    {
        // Calculer difference de temps entre maintenant et lastAttackTs
        return Time.time >= lastAttackTs + attackCooldown;
    }

    public void Attack(Vector2 _position)
    {
        // Verifier que l'attaque est prete
        if (!AttackReady())
        {
            Debug.LogWarning("Tentative d'attaquer echouee car attaque par prete");
            return;
        }
        
        // Verifier distance avec position
        if (Vector2.Distance(transform.position, _position) > attackDistance)
        {
            Debug.LogWarning("Tentative d'attaquer echouee car trop loin");
            return;
        }
        
        // Effectuer l'attaque
        InflictDamage(_position, Random.Range(strength.x, strength.y));
        
        // Modifier le lastAttackTs
        lastAttackTs = Time.time;
        
        // TODO: Animation d'attaque
        
    }

    protected virtual void InflictDamage(Vector2 position, float damage)
    {
        // Recuperer tous les colliders a proximite de la position
        Collider2D[] colliders = Physics2D.OverlapCircleAll(position, attackRadius);

        // Trier les colliders
        foreach (Collider2D collider in colliders)
        {
            Debug.Log("Is a unit");
            // Verifier s'il s'agit d'un Unite
            if (collider.TryGetComponent(out Unite _unite))
            {
                // Verifier qu'elle ne soit pas dans l'equipe
                if (team != _unite.team)
                {
                    // Infliger des degats
                    _unite.GetDamaged(damage);
                }
            }
        }
    }

    private void GetDamaged(float degats)
    {
        // Verifier les iframes
        if (Time.time < creationTs + 2f)
            return;
        
        // Perdre des points de vie
        healthPoints -= degats;
        
        // Verifier si je suis mort
        if (healthPoints <= 0)
        {
            // Auto destruction dans 10.. 9..
            team.UniteDeath(this);
            
            // Detruire
            Destroy(gameObject);
        }
    }
}
