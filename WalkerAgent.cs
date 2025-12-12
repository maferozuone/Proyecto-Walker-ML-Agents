using System;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgentsExamples;
using Unity.MLAgents.Sensors;
using BodyPart = Unity.MLAgentsExamples.BodyPart;
using Random = UnityEngine.Random;

public class WalkerAgent : Agent
{
    [Header("Walk Speed")]
    [Range(0.1f, 10)]
    [SerializeField]
    private float m_TargetWalkingSpeed = 5;

    public float MTargetWalkingSpeed
    {
        get { return m_TargetWalkingSpeed; }
        set { m_TargetWalkingSpeed = Mathf.Clamp(value, .1f, m_maxWalkingSpeed); }
    }

    const float m_maxWalkingSpeed = 10;
    public bool randomizeWalkSpeedEachEpisode;
    private Vector3 m_WorldDirToWalk = Vector3.right;

    [Header("Target To Walk Towards")]
    public Transform target;

    [Header("Body Parts")]
    public Transform hips, spine, head;
    public Transform thighL, shinL, footL;
    public Transform thighR, shinR, footR;
    public Transform armL, forearmL, handL;
    public Transform armR, forearmR, handR;

    OrientationCubeController m_OrientationCube;
    DirectionIndicator m_DirectionIndicator;
    JointDriveController m_JdController;
    EnvironmentParameters m_ResetParams;

    public override void Initialize()
    {
        m_OrientationCube = GetComponentInChildren<OrientationCubeController>();
        m_DirectionIndicator = GetComponentInChildren<DirectionIndicator>();
        m_JdController = GetComponent<JointDriveController>();

        // Set up body parts
        m_JdController.SetupBodyPart(hips);
        m_JdController.SetupBodyPart(spine);
        m_JdController.SetupBodyPart(head);
        m_JdController.SetupBodyPart(thighL);
        m_JdController.SetupBodyPart(shinL);
        m_JdController.SetupBodyPart(footL);
        m_JdController.SetupBodyPart(thighR);
        m_JdController.SetupBodyPart(shinR);
        m_JdController.SetupBodyPart(footR);
        m_JdController.SetupBodyPart(armL);
        m_JdController.SetupBodyPart(forearmL);
        m_JdController.SetupBodyPart(handL);
        m_JdController.SetupBodyPart(armR);
        m_JdController.SetupBodyPart(forearmR);
        m_JdController.SetupBodyPart(handR);

        m_ResetParams = Academy.Instance.EnvironmentParameters;
    }

    public override void OnEpisodeBegin()
    {
        foreach (var bodyPart in m_JdController.bodyPartsDict.Values)
            bodyPart.Reset(bodyPart);

        hips.rotation = Quaternion.Euler(0, Random.Range(0.0f, 360.0f), 0);
        UpdateOrientationObjects();

        MTargetWalkingSpeed = randomizeWalkSpeedEachEpisode
            ? Random.Range(0.1f, m_maxWalkingSpeed)
            : MTargetWalkingSpeed;
    }

    public void CollectObservationBodyPart(BodyPart bp, VectorSensor sensor)
    {
        sensor.AddObservation(bp.groundContact.touchingGround);
        sensor.AddObservation(m_OrientationCube.transform.InverseTransformDirection(bp.rb.velocity));
        sensor.AddObservation(m_OrientationCube.transform.InverseTransformDirection(bp.rb.angularVelocity));
        sensor.AddObservation(m_OrientationCube.transform.InverseTransformDirection(bp.rb.position - hips.position));

        if (bp.rb.transform != hips && bp.rb.transform != handL && bp.rb.transform != handR)
        {
            sensor.AddObservation(bp.rb.transform.localRotation);
            sensor.AddObservation(bp.currentStrength / m_JdController.maxJointForceLimit);
        }
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        var cubeForward = m_OrientationCube.transform.forward;
        var velGoal = cubeForward * MTargetWalkingSpeed;
        var avgVel = GetAvgVelocity();

        sensor.AddObservation(Vector3.Distance(velGoal, avgVel));
        sensor.AddObservation(m_OrientationCube.transform.InverseTransformDirection(avgVel));
        sensor.AddObservation(m_OrientationCube.transform.InverseTransformDirection(velGoal));
        sensor.AddObservation(Quaternion.FromToRotation(hips.forward, cubeForward));
        sensor.AddObservation(Quaternion.FromToRotation(head.forward, cubeForward));
        sensor.AddObservation(m_OrientationCube.transform.InverseTransformPoint(target.transform.position));

        foreach (var bodyPart in m_JdController.bodyPartsList)
            CollectObservationBodyPart(bodyPart, sensor);
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        var bpDict = m_JdController.bodyPartsDict;
        var a = actionBuffers.ContinuousActions;
        var i = -1;

        // Joint target rotations
        bpDict[spine].SetJointTargetRotation(a[++i], a[++i], a[++i]);
        bpDict[thighL].SetJointTargetRotation(a[++i], a[++i], 0);
        bpDict[thighR].SetJointTargetRotation(a[++i], a[++i], 0);
        bpDict[shinL].SetJointTargetRotation(a[++i], 0, 0);
        bpDict[shinR].SetJointTargetRotation(a[++i], 0, 0);
        bpDict[footR].SetJointTargetRotation(a[++i], a[++i], a[++i]);
        bpDict[footL].SetJointTargetRotation(a[++i], a[++i], a[++i]);
        bpDict[armL].SetJointTargetRotation(a[++i], a[++i], 0);
        bpDict[armR].SetJointTargetRotation(a[++i], a[++i], 0);
        bpDict[forearmL].SetJointTargetRotation(a[++i], 0, 0);
        bpDict[forearmR].SetJointTargetRotation(a[++i], 0, 0);
        bpDict[head].SetJointTargetRotation(a[++i], a[++i], 0);

        // Joint strengths
        bpDict[spine].SetJointStrength(a[++i]);
        bpDict[head].SetJointStrength(a[++i]);
        bpDict[thighL].SetJointStrength(a[++i]);
        bpDict[shinL].SetJointStrength(a[++i]);
        bpDict[footL].SetJointStrength(a[++i]);
        bpDict[thighR].SetJointStrength(a[++i]);
        bpDict[shinR].SetJointStrength(a[++i]);
        bpDict[footR].SetJointStrength(a[++i]);
        bpDict[armL].SetJointStrength(a[++i]);
        bpDict[forearmL].SetJointStrength(a[++i]);
        bpDict[armR].SetJointStrength(a[++i]);
        bpDict[forearmR].SetJointStrength(a[++i]);


        // =====================================================
        // 🔵 ***REWARD CLEAN DESIGN (solo las esenciales)***
        // =====================================================

        float distance = Vector3.Distance(hips.position, target.position);

        // 1️⃣ Recompensa por velocidad REAL hacia el objetivo
        Vector3 dirToTarget = (target.position - hips.position).normalized;
        float forwardVelocity = Vector3.Dot(hips.GetComponent<Rigidbody>().velocity, dirToTarget);
        AddReward(forwardVelocity * 0.002f);

        // 2️⃣ Penalización por distancia (camina más recto)
        AddReward(-0.0005f * distance);

        // 3️⃣ Recompensa por estabilidad
        if (Vector3.Angle(Vector3.up, hips.up) < 30f)
            AddReward(0.002f);

        // 4️⃣ Llegó a la meta
        if (distance < 0.5f)
        {
            AddReward(2.0f);
            EndEpisode();
        }

        // 5️⃣ Penalización por caída
        if (hips.position.y < 0.2f)
        {
            AddReward(-1.0f);
            EndEpisode();
        }
    }

    void FixedUpdate()
    {
        UpdateOrientationObjects();
    }

    void UpdateOrientationObjects()
    {
        m_WorldDirToWalk = target.position - hips.position;
        m_OrientationCube.UpdateOrientation(hips, target);
        if (m_DirectionIndicator)
            m_DirectionIndicator.MatchOrientation(m_OrientationCube.transform);
    }

    Vector3 GetAvgVelocity()
    {
        Vector3 velSum = Vector3.zero;
        int numOfRb = 0;
        foreach (var item in m_JdController.bodyPartsList)
        {
            numOfRb++;
            velSum += item.rb.velocity;
        }
        return velSum / numOfRb;
    }

    public void TouchedTarget()
    {
        AddReward(1f);
    }
}
