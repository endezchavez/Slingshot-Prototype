using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slingshot : MonoBehaviour
{
    public Transform launchPoint;

    [SerializeField] 
    private float pullbackSpeed = 3f;
    [SerializeField]
    private float minLaunchForce = 10f;
    [SerializeField]
    private float maxLaunchForce = 30f;
    [SerializeField]
    private float reloadTime = 1f;
    [SerializeField]
    private float maxPullbackDistance = 10f;

    public GameObject projectilePrefab;
    public Transform reloadPos;

    public Transform bandCentre;
    public Rigidbody projectileRB;
    Transform cameraTransform;

    private InputManager inputManager;
    private Rigidbody bandRB;

    private float currentPullbackDistance;

    private float launchForce;

    private bool hasBandBeenReleased = false;
    private bool hasReloaded = true;

    private void Start()
    {
        inputManager = InputManager.Instance;
        bandRB = bandCentre.GetComponent<Rigidbody>();
        cameraTransform = Camera.main.transform;

    }

    // Update is called once per frame
    void Update()
    {
        if (inputManager.PlayerIsCharging() && hasReloaded)
        {
            if (!bandRB.isKinematic)
            {
                bandRB.isKinematic = true;
            }
            if(Vector3.Distance(bandCentre.position, launchPoint.position) <= maxPullbackDistance)
            {
                Vector3 dir = (cameraTransform.position - launchPoint.position).normalized;
                bandCentre.position += dir * Time.deltaTime * pullbackSpeed;
            }
            
        }
        else
        {
            
            if (bandRB.isKinematic)
            {

                currentPullbackDistance = Vector3.Distance(bandCentre.position, launchPoint.position);

                bandRB.isKinematic = false;

                hasBandBeenReleased = true;
                hasReloaded = false;
                StartCoroutine(ReloadTimer());
            }

            if (hasBandBeenReleased && ShouldProjectileLaunch())
            {

                Launch();
            }
        }

        //Debug.DrawRay(bandCentre.position, -transform.forward * 10f, Color.green, 1);

    }

    void Launch()
    {
        
        projectileRB.transform.parent = null;
        projectileRB.isKinematic = false;


        launchForce = minLaunchForce + ((Mathf.InverseLerp(0f, maxPullbackDistance, currentPullbackDistance)) * (maxLaunchForce - minLaunchForce));

        projectileRB.AddForce(launchPoint.forward * launchForce, ForceMode.Impulse);
        projectileRB.transform.GetComponent<SphereCollider>().isTrigger = false;
        projectileRB.collisionDetectionMode = CollisionDetectionMode.Continuous;

        hasBandBeenReleased = false;
        currentPullbackDistance = 0f;


    }

    bool ShouldProjectileLaunch()
    {
        return Vector3.Distance(projectileRB.position, launchPoint.position) < 0.5f;
    }

    IEnumerator ReloadTimer()
    {
        yield return new WaitForSeconds(reloadTime);
        Reload();

    }

    private void Reload()
    {
        GameObject obj = Instantiate(projectilePrefab, reloadPos);
        projectileRB = obj.GetComponent<Rigidbody>();
        hasReloaded = true;
    }
}
