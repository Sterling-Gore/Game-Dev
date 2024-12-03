using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;
using SoundSource = PathNode;

public class AlienController : Loadable
{
    public SoundSource curTarget;
    public SoundSource nextTarget;
    public GameObject player;

    [Header("Decision Making")]
    public float attackRadius;
    public bool heardSomething = false;
    public float mentalDelay = 5.0f;
    public int soundSourcesMemory;

    [Header("Movement")]
    public float turnRadius;
    public float turnSpeed;
    public float walkSpeed;
    public float runSpeed;
    public float tiredSpeed;
    [Header("Calculated Movement")]
    public float actualSpeed;
    public float timeStayingStill;
    public float curSpeed;
    public float nextSpeed;
    float timeInSpeed;

    [Header("Stamina")]
    public float restingPeriod;
    public float walkingStamina;
    public float runningStamina;

    Vector3 prevPos = new();

    Rigidbody playerRb;
    public PathGraph pathGraph;

    public PathFindingController pathFinder;
    RoamController roamer;
    public Transform head;
    public List<SoundSource> blackListedSoundSources = new();
    public GameObject soundSource;
    bool justHeardSomething;
    int curPowerLevel = -1;
    public List<Transform> curSections = new();
    Animator animator;

    AudioSource walkingAudio, idleAudio, attackAudio;

    [Header("Audio")]
    public List<AudioClip> walkingClips = new();
    public List<AudioClip> idleClips = new(), attackClips = new();


    HealthSystem playerHealthSystem;
    [Header("Attack")]
    public float damageAmount = 0.1f;
    public float attackCooldown = 1f;
    private float lastAttackTime = 0;

    public static List<AlienController> aliens = new();

    static int ignoreAlienLayer, groundLayer;

    public bool isAwareOfPlayer = false;

    void Start()
    {
        animator = GetComponent<Animator>();

        playerRb = player.GetComponent<Rigidbody>();

        curSpeed = nextSpeed = walkSpeed;

        pathFinder = new(this);
        roamer = GetComponent<RoamController>();

        UpdatePowerLevel(PowerLevel.instance.currentPowerLevel);
        PowerLevel.instance.SubscribeToUpdates(powerLevel => UpdatePowerLevel(powerLevel));

        roamer.Init(this);

        Transform sounds = transform.Find("Sounds");
        idleAudio = sounds.Find("IdleSounds").gameObject.GetComponent<AudioSource>();
        walkingAudio = sounds.Find("WalkSounds").gameObject.GetComponent<AudioSource>();
        attackAudio = sounds.Find("AttackSounds").gameObject.GetComponent<AudioSource>();

        aliens.Add(this);
        ignoreAlienLayer = ~(
            1 << LayerMask.NameToLayer("AlienLayer")
        );
        groundLayer = 1 << LayerMask.NameToLayer("whatIsGround");

        playerHealthSystem = player.GetComponent<HealthSystem>();
        lastAttackTime = -attackCooldown;
    }

    void Update()
    {
        if (!isAwareOfPlayer)
            return;

        soundSource.transform.position = curTarget.pos;

        KeepUpright();
        animator.SetBool("isWalking", true);
        animator.SetBool("isRunning", false);
        if (!heardSomething)
        {
            if (nextTarget != SoundSource.None)
            {
                curTarget = nextTarget;
                nextTarget = SoundSource.None;
                AnnounceHeardSomething();
            }
            nextSpeed = walkSpeed;
            roamer.RoamAround();
        }
        else
        {
            if (Time.time - lastAttackTime >= attackCooldown)
                HuntPlayer();
            else
                RunFromPlayer();
        }
        // Debug.DrawRay(transform.position + Vector3.up, player.transform.position - transform.position + Vector3.up);
    }

    override protected void OnDestroy()
    {
        aliens.Remove(this);
        pathFinder.Dispose();
        pathGraph.Dispose();
        base.OnDestroy();
    }

    void KeepUpright()
    {
        transform.rotation.Set(0, 0, 0, 0);
    }

    void AnnounceHeardSomething()
    {
        PlayRandomIdleAudio();
        justHeardSomething = true;
        heardSomething = true;
        Debug.Log("I hear you");
    }

    void HuntPlayer()
    {
        if (
            nextTarget != SoundSource.None &&
            Vector3.Distance(nextTarget.pos, transform.position) < Vector3.Distance(curTarget.pos, transform.position)
        )
            curTarget = nextTarget;
        PlayRandomWalkAudio();
        var pos = transform.position;
        var playerPos = player.transform.position;
        playerPos.y = pos.y = (playerPos.y + pos.y + 1) / 2;

        var directionToPlayer = playerPos - pos;
        var distanceToPlayer = directionToPlayer.magnitude;

        Physics.Raycast(pos, directionToPlayer.normalized, out RaycastHit j, distanceToPlayer, ignoreAlienLayer);
        // Debug.Log(j.rigidbody);
        Debug.DrawRay(pos, directionToPlayer, Color.green);
        if (j.rigidbody == playerRb)
        {
            // if (curSpeed == runSpeed)
            animator.SetBool("isRunning", true);
            nextSpeed = runSpeed;
            if (distanceToPlayer < attackRadius)
                AttackPlayer();
            else
                GoStraightToPlayer();
        }
        else if (!justHeardSomething && pathFinder.HasArrived())
        {
            blackListedSoundSources.Add(curTarget);
            if (blackListedSoundSources.Count > soundSourcesMemory)
                blackListedSoundSources.RemoveAt(0);

            roamer.FindCurrentRoom();
            heardSomething = false;
            if (nextTarget == SoundSource.None)
                Debug.Log("no longer heard anything");
        }
        else
        {
            nextSpeed = walkSpeed;
            pathFinder.CalculatePathPeriodically(curTarget.pos);
            pathFinder.FollowPath();
        }
        justHeardSomething = false;
    }

    void RunFromPlayer()
    {

    }

    void UpdatePowerLevel(int powerLevel)
    {
        if (powerLevel != curPowerLevel)
        {
            isAwareOfPlayer = powerLevel > 0;
            curSections = new(3);
            switch (powerLevel)
            {
                case 3:
                    goto case 2;
                case 2:
                    curSections.Add(GameObject.Find("SectionC").transform);
                    goto case 1;
                case 1:
                    curSections.Add(GameObject.Find("SectionB").transform);
                    goto case 0;
                case 0:
                    curSections.Add(GameObject.Find("SectionA").transform);
                    break;
            }

            ReloadPathGraph();
            curPowerLevel = powerLevel;
            roamer.UpdateRooms(curSections);
        }
    }

    public void ReloadPathGraph()
    {
        pathGraph = new PathGraph(NodesInSections(curSections));
    }
    List<Transform> NodesInSections(List<Transform> pathNodeSections)
    {
        List<Transform> nodes = new();
        foreach (Transform section in pathNodeSections)
            foreach (Transform room in section)
                foreach (Transform pathNode in room)
                    nodes.Add(pathNode);
        return nodes;
    }

    void AttackPlayer()
    {
        Debug.Log("damage dealt player");
        PlayRandomAttackAudio();
        playerHealthSystem.TakeDamage(damageAmount);
        lastAttackTime = Time.time;
        PlayRandomAttackAudio();
    }

    void GoStraightToPlayer()
    {
        MoveTowards(player.transform.position);
        //don't go back to path just recalculate path
        pathFinder.WillRecalculate();
    }

    /// <returns>true if reached target </returns>
    public bool MoveTowards(Vector3 target)
    {
        CheckStayingStill();
        PlayRandomWalkAudio();
        target.y = transform.position.y;
        var targetRotation = Quaternion.LookRotation(target - transform.position);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * turnSpeed);

        var dPos = CurSpeed() * Time.deltaTime;

        prevPos = transform.position;
        transform.Translate(Vector3.forward * dPos);
        var curPos = transform.position;
        var closestPoint = GetClosestPointToLine(prevPos, (curPos - prevPos).normalized, prevPos - target);

        transform.position = Clamp(closestPoint, prevPos, curPos);

        CheckNewPosition();

        return Vector3.Distance(transform.position, target) <= dPos;
    }

    Vector3 GetClosestPointToLine(Vector3 origin, Vector3 direction, Vector3 point2origin) =>
        origin - Vector3.Dot(point2origin, direction) * direction;

    Vector3 Clamp(Vector3 point, Vector3 start, Vector3 end)
    {
        var start2end = (end - start).normalized;
        var start2point = (point - start).normalized;
        if (start2point != start2end)
            return start;
        var end2point = (point - start).normalized;
        if (end2point == start2end)
            return end;
        return point;
    }

    void CheckNewPosition()
    {
        var aboveNewPosition = transform.position;
        aboveNewPosition.y += 1;
        if (!Physics.Raycast(aboveNewPosition, Vector3.down, 10, groundLayer))
        {
            Debug.DrawRay(aboveNewPosition, Vector3.down * 10, Color.black, 20);
            Debug.Log($"point not above ground {aboveNewPosition}"); // Change to just Log instead of Error as these can occur depending on object geo. 
            transform.position = prevPos;
        }
    }

    void CheckStayingStill()
    {
        actualSpeed = Vector3.Distance(prevPos, transform.position) / Time.deltaTime;
        if (actualSpeed < .5)
        {
            timeStayingStill += Time.deltaTime;
            if (timeStayingStill > 2)
            {
                Debug.Log($"alien is stuck! was hunting: {heardSomething}, current state: {roamer.state}"); // Change to log, Same issue here
                animator.SetBool("isWalking", false);
                roamer.FindCurrentRoom();
                if (!heardSomething)
                    roamer.curState.OnStuck();
                else
                    heardSomething = false;
                timeStayingStill = 0;
            }
        }
        else
            timeStayingStill = 0;
    }

    float CurSpeed()
    {
        timeInSpeed += Time.deltaTime;
        if (curSpeed == runSpeed && timeInSpeed > runningStamina)
        {
            curSpeed = walkSpeed;
            timeInSpeed = 0;
        }
        else if (curSpeed == walkSpeed && timeInSpeed > walkingStamina)
        {
            curSpeed = tiredSpeed;
            timeInSpeed = 0;
        }
        else if (curSpeed == tiredSpeed && timeInSpeed > restingPeriod)
        {
            curSpeed = nextSpeed;
            timeInSpeed = 0;
        }

        return curSpeed;
    }

    public void PlayRandomWalkAudio() => PlayRandomAudio(walkingAudio, walkingClips);

    public void PlayRandomAttackAudio() => PlayRandomAudio(attackAudio, attackClips);

    public void PlayRandomIdleAudio() => PlayRandomAudio(idleAudio, idleClips);

    void PlayRandomAudio(AudioSource audioSource, List<AudioClip> audioClips)
    {
        if (Time.timeScale > 0 && !audioSource.isPlaying && audioClips.Count != 0)
        {
            audioSource.clip = audioClips[Random.Range(0, audioClips.Count)];
            audioSource.Play();
        }
    }

    public override void Load(JObject state)
    {
        LoadTransform(state);
        heardSomething = false;
    }

    public override void Save(ref JObject state) =>
        SaveTransform(ref state);
}
/*
 * Ideas:
 * To save on CPU usage, only run A* every now and then
 * Path will be calculated and alien will follow it until its close enough to target
 * Alien will use path nodes defined under the "AlienPathNodes" gameobject. 
 * 
 * Also if theres a ray from alien to target with nothing in between go straight
 * 
 * 
 * TODO: make attack, make path finding for noise/specific events
 cases:
 pathing to room -> pathing to player
 pathing in room -> pathing to player
 didn't reach room 
 pathing to player-> pathing to room
 
 */