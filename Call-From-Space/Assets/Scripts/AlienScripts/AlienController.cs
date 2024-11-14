using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;
using SoundSource = PathNode;

public class AlienController : MonoBehaviour
{
    public SoundSource target;
    public GameObject player;
    GameObject endingScreen;

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

    Rigidbody playerRb;
    public PathGraph pathGraph;

    public PathFindingController pathFinder;
    RoamController roamer;
    public Transform head;
    public List<SoundSource> blackListedSoundSources = new();
    public GameObject soundSource;
    bool justHeardSomething;
    PowerLevel powerLevelManager;
    int curPowerLevel = -1;
    public List<Transform> curSections = new();
    Animator animator;
    AudioSource walkingAudio, idleAudio, attackAudio;
    List<AudioClip> walkingClips = new(), idleClips = new(), attackClips = new();
    public HealthSystem playerHealthSystem; // Assign this in the Inspector
    public float damageAmount = 0.1f; // Amount of damage to deal to the player
    public float attackCooldown = 1f; // Cooldown time between attacks
    private float lastAttackTime;

    void Start()
    {
        animator = GetComponent<Animator>();

        endingScreen = GameObject.Find("EndingScreen");

        playerRb = player.GetComponent<Rigidbody>();

        curSpeed = nextSpeed = walkSpeed;

        SoundSourcesController.GetInstance().SubscribeToSoundSources(this);

        var powerManager = GameObject.Find("PowerManager");
        powerLevelManager = powerManager.GetComponent<PowerLevel>();
        pathFinder = new(this);
        roamer = GetComponent<RoamController>();

        UpdatePathGraph();

        roamer.Init(this);

        Transform sounds = transform.Find("Sounds");
        idleAudio = sounds.Find("IdleSounds").gameObject.GetComponent<AudioSource>();
        attackAudio = sounds.Find("AttackSounds").gameObject.GetComponent<AudioSource>();
        walkingAudio = sounds.Find("WalkSounds").gameObject.GetComponent<AudioSource>();
        StartCoroutine(LoadAudioClips());
        playerHealthSystem = player.GetComponent<HealthSystem>();
        lastAttackTime = -attackCooldown; // Initialize to allow immediate attack
    }

    void Update()
    {
        UpdatePathGraph();
        //Debug
        soundSource.transform.position = target.pos;

        KeepUpright();
        animator.SetBool("isWalking", true);
        animator.SetBool("isRunning", false);
        if (!heardSomething)
        {
            if (target != new SoundSource())
                AnnounceHeardSomething();
            nextSpeed = walkSpeed;
            roamer.RoamAround();
        }
        else
            HuntPlayer();
        Debug.DrawRay(transform.position + Vector3.up, player.transform.position - transform.position + Vector3.up);
    }

    void OnDestroy()
    {
        pathFinder.Dispose();
        pathGraph.Dispose();
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
        PlayRandomWalkAudio();
        var directionToPlayer = player.transform.position - transform.position;
        var distanceToPlayer = directionToPlayer.magnitude;

        Physics.Raycast(transform.position + Vector3.up, directionToPlayer, out RaycastHit j, distanceToPlayer - .1f);
        // Debug.Log(j.rigidbody);
        // Debug.DrawRay(transform.position + Vector3.up, directionToPlayer + Vector3.up);
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
            blackListedSoundSources.Add(target);
            if (blackListedSoundSources.Count > soundSourcesMemory)
                blackListedSoundSources.RemoveAt(0);
            target = new SoundSource();
            heardSomething = false;
        }
        else
        {
            nextSpeed = walkSpeed;
            pathFinder.CalculatePathPeriodically();
            pathFinder.FollowPath();
        }
        justHeardSomething = false;
    }

    void UpdatePathGraph()
    {
        int powerLevel = powerLevelManager.GetCurrentPowerLevel();
        if (powerLevel != curPowerLevel)
        {
            if (powerLevel == 0)
                curSections.Add(GameObject.Find("SectionA").transform);
            else if (powerLevel == 1)
                curSections.Add(GameObject.Find("SectionB").transform);
            else if (powerLevel == 2)
                curSections.Add(GameObject.Find("SectionC").transform);
            pathGraph = new PathGraph(NodesInSections(curSections));
            curPowerLevel = powerLevel;
            roamer.UpdateRooms(curSections[^1]);
        }
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
        if (Time.time - lastAttackTime >= attackCooldown)
        {
            PlayRandomAttackAudio();
            playerHealthSystem.TakeDamage(damageAmount);
            lastAttackTime = Time.time;
        }
        //var gameOver = endingScreen.transform.GetChild(0).gameObject;
        //gameOver.SetActive(true);
    }

    void GoStraightToPlayer()
    {
        MoveTowards(player.transform.position);
        //don't go back to path just recalculate path
        pathFinder.Recalculate();
    }

    /// <returns>true if reached target </returns>
    public bool MoveTowards(Vector3 target)
    {
        PlayRandomWalkAudio();
        target.y = transform.position.y;
        var targetRotation = Quaternion.LookRotation(target - transform.position);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * turnSpeed);

        var dPos = CurSpeed() * Time.deltaTime;

        var prevPos = transform.position;
        transform.Translate(Vector3.forward * dPos);
        var curPos = transform.position;
        var closestPoint = GetClosestPointToLine(prevPos, (curPos - prevPos).normalized, prevPos - target);

        transform.position = Clamp(closestPoint, prevPos, curPos);

        CheckNewPosition(prevPos);
        CheckStayingStill(prevPos);


        prevPos.y += 1;
        var newPos = transform.position;
        newPos.y += 1;
        Debug.DrawLine(prevPos, newPos);

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

    void CheckNewPosition(Vector3 prevPos)
    {
        var aboveNewPosition = transform.position;
        aboveNewPosition.y += 1;
        if (!Physics.Raycast(aboveNewPosition, Vector3.down, 10, ~LayerMask.NameToLayer("whatIsGround")))
        {
            Debug.DrawRay(aboveNewPosition, Vector3.down * 10, Color.black);
            Debug.LogError($"point not above ground {aboveNewPosition} {LayerMask.NameToLayer("whatIsGround")}");
            transform.position = prevPos;
        }
    }

    void CheckStayingStill(Vector3 prevPos)
    {
        actualSpeed = Vector3.Distance(prevPos, transform.position) / Time.deltaTime;
        if (actualSpeed < .5)
        {
            timeStayingStill += Time.deltaTime;
            if (timeStayingStill > 2)
            {
                Debug.LogError("alien is stuck!");
                animator.SetBool("isWalking", false);
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

    public void PlayRandomWalkAudio()
    {
        if (!walkingAudio.isPlaying && walkingClips.Count != 0)
        {
            walkingAudio.clip = walkingClips[Random.Range(0, walkingClips.Count)];
            walkingAudio.Play();
        }
    }

    public void PlayRandomAttackAudio()
    {
        attackAudio.Play();
        //if (!attackAudio.isPlaying && attackClips.Count != 0)
        //{
        //    attackAudio.clip = attackClips[Random.Range(0, attackClips.Count)];
        //    attackAudio.Play();
        //}
    }

    public void PlayRandomIdleAudio()
    {
        idleAudio.Play();
        /*
        if (!idleAudio.isPlaying && idleClips.Count != 0)
        {
            idleAudio.clip = idleClips[Random.Range(0, idleClips.Count)];
            idleAudio.Play();
        }
        */
    }

    IEnumerator LoadAudioClips()
    {
        List<(string path, List<AudioClip> clips)> audioClips = new(){
            ("Idle",idleClips),
            ("Movement",walkingClips),
            ("Attack",attackClips)
        };
        foreach (var (path, clips) in audioClips)
        {
            string soundPath = Path.Combine(Application.dataPath, "Sound", "Alien", path);
            var dir = new DirectoryInfo(soundPath);
            var info = dir.GetFiles("*.wav");

            if (info.Length == 0)
            {
                Debug.LogWarning($"No .wav files found in the {path} folder!");
                yield break;
            }

            var fileNames = info.Select(x => x.FullName).ToList();
            yield return LoadAudio(clips, fileNames);
        }
    }

    IEnumerator LoadAudio(List<AudioClip> audioClips, List<string> paths)
    {
        foreach (string path in paths)
        {
            using var www = UnityWebRequestMultimedia.GetAudioClip("file://" + path, AudioType.WAV);
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
                if (clip)
                    audioClips.Add(clip);
                else
                    Debug.LogError("Failed to create AudioClip from file: " + path);
            }
        }
    }
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
 */