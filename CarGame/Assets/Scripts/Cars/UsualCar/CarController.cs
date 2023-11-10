using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

public partial class CarController : Car //AxelInfo
{
    [SerializeField] CarModel carModel;
    [SerializeField] CarView carView;
    [SerializeField] GameObject car;
    [SerializeField] Rigidbody rigidbody;
    [SerializeField] bool isAIControlledCar = false;
    [SerializeField] AndroidInput agent;
    [SerializeField] SelfDrivingCar pathFollower;
    [SerializeField] GameObject racingCameraObject;
    [SerializeField] GameObject carHeadlightFlare1;
    [SerializeField] GameObject carHeadlightFlare2;
    [SerializeField] GameObject nitrous1;
    [SerializeField] GameObject nitrous2;
    [SerializeField] AudioSource gasAudio;
    [SerializeField] AudioSource brakeAudio;
    [SerializeField] AudioSource nitrousAudio;
    [SerializeField] GameObject brakeLight1;
    [SerializeField] GameObject brakeLight2;
    [SerializeField] AudioSource crashAudio;
    [SerializeField] GameObject interiorView;
    [SerializeField] GameObject nitrousUI;
    [SerializeField] List<MeshRenderer> bodyMeshRenderers;
    [SerializeField] List<GameObject> bodyGameObjects;
    [SerializeField] MeshCollider bodyCollider;
    [SerializeField] GameObject rearViewMirrorCamera;
    bool nitrousReady = false;
    [SerializeField] bool raceStarted;
    float nitrousTimer;

    public override void Start()
    {
        base.Start();
        if (isAIControlledCar)
            Events.CarSpawnedToTrack?.Invoke(this);
        Events.RaceStarted += HandleRaceStarted;
        if(!isAIControlledCar)
        {
            //carModel.Label = GameManager.GetUsername();
        }
        if(nitrous1 != null)
        {
            nitrous1.SetActive(false);
        }
        if (nitrous2 != null)
        {
            nitrous2.SetActive(false);
        }
    }

    public override void Update()
    {
        base.Update();
        if (isAIControlledCar || !raceStarted) { return; }
        Quaternion tempRotation = transform.rotation;
        tempRotation.x = 0;
        tempRotation.y = 0;
        transform.rotation = tempRotation;
        if(Input.GetKeyDown(KeyCode.C))
        {
            interiorView.SetActive(!interiorView.activeInHierarchy);
        }
        if(nitrousReady && Input.GetKeyDown(KeyCode.N))
        {
            UseNitrous();
        }
        nitrousUI.SetActive(nitrousReady);

        foreach(MeshRenderer meshRenderer in bodyMeshRenderers)
        {
            meshRenderer.enabled = !interiorView.activeInHierarchy;
        }

        foreach(GameObject bodyGameObject in bodyGameObjects)
        {
            bodyGameObject.SetActive(!interiorView.activeInHierarchy);
        }
        bodyCollider.enabled = !interiorView.activeInHierarchy;
        rearViewMirrorCamera.SetActive(interiorView.activeInHierarchy);
        if(Input.GetKey(KeyCode.UpArrow))
        {
            HandleGasPedal();
        }
        else if(Input.GetKeyDown(KeyCode.DownArrow))
        {
            HandleBrakePedal();
        }
        brakeLight1.SetActive(Input.GetKey(KeyCode.DownArrow));
        brakeLight2.SetActive(Input.GetKey(KeyCode.DownArrow));

        if(nitrousReady && nitrousTimer >= 30.0f)
        {
            nitrousReady = true;
        } else if(!nitrousReady)
        {
            nitrousTimer += Time.deltaTime;
        }
    }

    private void OnDestroy()
    {
        Events.RaceStarted -= HandleRaceStarted;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(crashAudio != null && !crashAudio.isPlaying)
        {
            crashAudio.Play();
        }
    }

    private void UseNitrous()
    {
        nitrousReady = false;
        nitrousUI.SetActive(false);
        ToggleNitrous(true);
        IncreaseSpeed();
        Invoke("HideNitrousEffect", 5.0f);
        nitrousTimer = 0;
    }

    private void HideNitrousEffect()
    {
        DecreaseSpeed();
        ToggleNitrous(false);
    }

    public void DisplayCar(bool shouldShow, bool applyCustomisations = false, bool showHeadlightFlares = false)
    {
        car.SetActive(shouldShow);
        if(applyCustomisations)
        {
            SetCarColorAndRims();
        }
        ToggleHeadlightFlares(showHeadlightFlares);
    }

    public GameObject GetCar() { return car; }
    public string GetCarName()
    {
        return carModel.CarName;
    }
    public float GetCarPrice()
    {
        return carModel.Price;
    }
    public void SetCarColorAndRims()
    {
        carView.SetCarColorAndRims();
    }
    public void SetCarColor(Color color)
    {
        carView.SetCarColor(color);
    }
    public void SetRimMaterial(Material material)
    {
        carView.SetRimMaterial(material);
    }
    public Material[] GetCarBodyMaterials()
    {
        return carView.GetCarBodyMaterials();
    }
    public Material GetRimMaterial()
    {
        return carView.GetRimMaterial();
    }

    public float GetSpeed()
    {
        return rigidbody.velocity.magnitude;
    }
    public void HandleCheckPointWasHit()
    {
        carModel.CheckpointWasHit();
    }
    public void SetCarLabel(string label)
    {
        carModel.Label = label;
    }
    public string GetCarLabel()
    {
        return carModel.Label;
    }

    public void SetDistanceToNextCheckpoint(float distance)
    {
        carModel.DistanceToNextChekpoint = distance;
    }

    public float GetDistanceToNextCheckpoint()
    {
        return carModel.DistanceToNextChekpoint;
    }

    public List<AxleInfo> axelInfos;
    public float maxMotorTorque;
    public float maxSteeringAngle;
    public float motor;

    public void FixedUpdate()
    {
        if(isAIControlledCar || !raceStarted)
        {
            return;
        }

        motor = maxMotorTorque * Input.GetAxis("Vertical");
        float steering = maxSteeringAngle * Input.GetAxis("Horizontal");
        foreach(AxleInfo axleInfo in axelInfos)
        {
            if (axleInfo.steering)
            {
                axleInfo.leftWheel.steerAngle= steering;
                axleInfo.rightWheel.steerAngle= steering;
            }
            if (axleInfo.motor)
            {
                axleInfo.leftWheel.motorTorque = motor;
                axleInfo.rightWheel.motorTorque = motor;
            }
        }
    }

    public void EnableAICar()
    {
        rigidbody.constraints = RigidbodyConstraints.FreezePositionX 
                                | RigidbodyConstraints.FreezePositionZ 
                                | RigidbodyConstraints.FreezePositionY;
        //agent.enabled = true;
        pathFollower.enabled = true;
    }

    public GameObject GetRacingCameraObject()
    {
        return racingCameraObject;
    }

    public void ToggleNitrous(bool shouldShow)
    {
        nitrous1.SetActive(shouldShow);
        nitrous2.SetActive(shouldShow);
        if(shouldShow)
        {
            nitrousAudio.Play();
        } else
        {
            nitrousAudio.Stop();
        }
    }

    public void IncreaseSpeed()
    {
        maxMotorTorque *= 4;
    }

    public void DecreaseSpeed()
    {
        maxMotorTorque /= 4;
    }
    
    void HandleRaceStarted()
    {
        raceStarted = true;
    }

    public void ToggleHeadlightFlares(bool shouldShow)
    {  
        carHeadlightFlare1.SetActive(shouldShow);
        carHeadlightFlare2.SetActive(shouldShow);   
    }
    void HandleGasPedal()
    {
        if(gasAudio.isPlaying)
            return;
        gasAudio.Play();
    }
    void HandleBrakePedal()
    {
        if (brakeAudio.isPlaying)
            return;
        brakeAudio.Play();
    }
}
