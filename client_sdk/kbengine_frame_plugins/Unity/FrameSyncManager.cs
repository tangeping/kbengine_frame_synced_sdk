using KBEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FrameSyncManager : MonoBehaviour {

    private const string serverSettingsAssetFile = "FrameSyncConfig";

    private static FrameSyncConfig _FrameSyncGlobalConfig; //配置文件

    public static FrameSyncConfig FrameSyncGlobalConfig
    {
        get
        {
            if (_FrameSyncGlobalConfig == null)
            {
                _FrameSyncGlobalConfig = (FrameSyncConfig)Resources.Load(serverSettingsAssetFile, typeof(FrameSyncConfig));
                //Debug.Log("_FrameSyncGlobalConfig.lockedTimeStep:" + _FrameSyncGlobalConfig.lockedTimeStep);
            }

            return _FrameSyncGlobalConfig;
        }
    }
    public static FrameSyncConfig Config
    {
        get
        {
            return FrameSyncGlobalConfig;
        }
    }

    private FP lockedTimeStep;

    private static FrameSyncManager instance;

    public static FP DeltaTime
    {
        get
        {
            if (instance == null)
            {
                return 0;
            }

            return instance.lockedTimeStep;
        }
    }

    private FP renderTime = 0;

    public static FP RenderTime
    {
        get
        {
            if (instance == null)
            {
                return 0;
            }

            return instance.renderTime;
        }

    }

    public const int kThresholdMaxFrame = 30;

    private int thresholdFrame = 1;

    public int ThresholdFrame
    {
        get
        {
            return thresholdFrame;
        }

        set
        {
            thresholdFrame = Mathf.Clamp(value,1, kThresholdMaxFrame);
        }
    }
    private FP timeSlice;
    public static FP TimeSlice
    {
        get
        {
            if (instance == null)
            {
                return 0;
            }

            return instance.timeSlice;
        }
    }

    public static FS_FRAME_DATA CurrFrame
    {
        get
        {
            if(instance == null)
            {
                return null;
            }
            return instance.currFrame;
        }
    }
    private FS_FRAME_DATA currFrame = null;

    public static UInt32 CurrFrameID
    {
        get
        {
            if (instance == null)
            {
                return 0;
            }
            return instance.currFrame.frameid;
        }
    }

    public static FP PastTime
    {
        get
        {
            if (instance == null)
            {
                return 0;
            }
            return instance.currFrame.frameid * instance.lockedTimeStep;
        }
    }

    /**
     * @brief The coroutine scheduler.
     **/
    //private CoroutineScheduler scheduler;

    /**
     * @brief The instant player perfab .
     **/
    public GameObject playerPerfab; 
    /**
     * @brief A dictionary of {@link FrameSyncBehaviour} not linked to any player.
     **/
    private List<KeyValuePair<IFrameSyncBehaviour, FrameSyncManagedBehaviour>> ListManagedBehaviors =
        new List<KeyValuePair<IFrameSyncBehaviour, FrameSyncManagedBehaviour>>();
    /**
     * @brief A dictionary holding a list of {@link FrameSyncBehaviour} belonging to each player.
     **/
    private Dictionary<int, List<FrameSyncManagedBehaviour>> behaviorsByPlayer = new Dictionary<int, List<FrameSyncManagedBehaviour>>();


    private FrameSyncManagedBehaviour NewManagedBehavior(IFrameSyncBehaviour FrameSyncBehavior)
    {                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                    
        FrameSyncManagedBehaviour result = new FrameSyncManagedBehaviour(FrameSyncBehavior);
        return result;
    }

    public int GetBehaviourIndex(List<KeyValuePair<IFrameSyncBehaviour, FrameSyncManagedBehaviour>> Behaviors, IFrameSyncBehaviour bh)
    {
        for (int i = 0; i < Behaviors.Count; i++)
        {
            if (Behaviors[i].Key == bh)
            {
                return i;
            }
        }

        return -1;
    }

    void CreatePlayer()
    {
        for (int i = 0; i < SpaceData.Instance.SpacePlayers.Count; i++)
        {
            Entity entity = SpaceData.Instance.SpacePlayers[i].owner;

            AddPlayer(playerPerfab, entity);
        }
    }
    /**
     * @brief add player  and bind entity to player.
     * @param playerPerfab GameObject's prefab to instantiate.
     * @param e is kbengine.entity to communicate  client.
     **/
    public static GameObject AddPlayer(GameObject go,Entity e)
    {
        GameObject perfab = GameObject.Instantiate(go) as GameObject;
        perfab.name = e.className + "_" + e.id;
        e.renderObj = perfab;
        Debug.Log(message: e.id +".position:"+e.position+",direction:"+e.direction);
        
        InitPlayerBehaviour(perfab, e);

        return perfab;
    }

    /**
     * @brief add player FrameSyncManagedBehaviour to system.
     * @param playerPerfab GameObject's prefab to instantiate.
     * @param e is kbengine.entity to communicate  client.
     **/

    public static void InitPlayerBehaviour(GameObject go,Entity e)
    {
        InitializeGameObject(go, e.position.ToFPVector(), Quaternion.Euler(e.direction).ToFPQuaternion());

        List<FrameSyncManagedBehaviour> playerBehaviours = new List<FrameSyncManagedBehaviour>();

        FrameSyncBehaviour[] behaviours = go.GetComponentsInChildren<FrameSyncBehaviour>();
        for (int index = 0; index < behaviours.Length; index++)
        {
            FrameSyncBehaviour bh = behaviours[index];

            bh.owner = e;
            bh.ownerIndex = e.id;
            bh.localOwner = KBEngineApp.app.player();
            playerBehaviours.Add(instance.NewManagedBehavior((IFrameSyncBehaviour)bh));
        }
        instance.behaviorsByPlayer.Add(e.id, playerBehaviours);
    }

    /**
     * @brief Instantiates a new prefab in a deterministic way.
     * 
     * @param prefab GameObject's prefab to instantiate.
     * @param position Position to place the new GameObject.
     * @param rotation Rotation to set in the new GameObject.
     **/
    public static GameObject SyncedInstantiate(GameObject prefab, FPVector2 position, FPQuaternion rotation)
    {
        return SyncedInstantiate(prefab, new FPVector(position.x, position.y, 0), rotation);
    }

    /**
 * @brief Instantiate a new prefab in a deterministic way.
 * 
 * @param prefab GameObject's prefab to instantiate.
 **/
    public static GameObject SyncedInstantiate(GameObject prefab)
    {
        return SyncedInstantiate(prefab, prefab.transform.position.ToFPVector(), prefab.transform.rotation.ToFPQuaternion());
    }

    /**
     * @brief Instantiates a new prefab in a deterministic way.
     * 
     * @param prefab GameObject's prefab to instantiate.
     * @param position Position to place the new GameObject.
     * @param rotation Rotation to set in the new GameObject.
     **/

    public static GameObject SyncedInstantiate(GameObject prefab, FPVector position, FPQuaternion rotation)
    {
        if (instance != null /*&& instance.lockstep != null*/)
        {
            GameObject go = GameObject.Instantiate(prefab, position.ToVector(), rotation.ToQuaternion()) as GameObject;
            AddSyncBehaviour(go);
            InitializeGameObject(go, position, rotation);
            return go;
        }
        return null;
    }

    public static GameObject AddSyncBehaviour(GameObject go)
    {
        if(go == null || instance ==null)
        {
            return null;
        }

        MonoBehaviour[] monoBehaviours = go.GetComponentsInChildren<MonoBehaviour>();
        for (int index = 0, length = monoBehaviours.Length; index < length; index++)
        {
            MonoBehaviour bh = monoBehaviours[index];

            if (bh is IFrameSyncBehaviour)
            {
                instance.ListManagedBehaviors.Add(new KeyValuePair<IFrameSyncBehaviour, FrameSyncManagedBehaviour>(
                   (IFrameSyncBehaviour)bh, instance.NewManagedBehavior((IFrameSyncBehaviour)bh)
                   ));
            }
        }
        return go;
    }

    public static GameObject RemoveSyncBehaviour(GameObject go)
    {
        if (go == null || instance == null)
        {
            return null;
        }

        List<FrameSyncBehaviour> behavioursToRemove = new List<FrameSyncBehaviour>(go.GetComponentsInChildren<FrameSyncBehaviour>());

        for (int i = 0; i < behavioursToRemove.Count; i++)
        {
            IFrameSyncBehaviour tsmb = behavioursToRemove[i] as IFrameSyncBehaviour;

            int index = instance.GetBehaviourIndex(instance.ListManagedBehaviors, tsmb);
            if (index >= 0)
            {
                instance.ListManagedBehaviors.RemoveAt(index);
            }
        }
        return go;
    }

    /**
 * @brief Destroys a GameObject in a deterministic way.
 * 
 * The method {@link #DestroyFPRigidBody} is called and attached FrameSyncBehaviors are disabled.
 * 
 * @param rigidBody Instance of a {@link FPRigidBody}
 **/
    public static void SyncedDestroy(GameObject go,float delayTime = 0.0f)
    {
        if (instance != null /*&& instance.lockstep != null*/)
        {
            SyncedDisableBehaviour(go);

            FPCollider[] tsColliders = go.GetComponentsInChildren<FPCollider>();
            if (tsColliders != null)
            {
                for (int index = 0, length = tsColliders.Length; index < length; index++)
                {
                    FPCollider tsCollider = tsColliders[index];
                    DestroyFPRigidBody(tsCollider.gameObject, tsCollider.Body);
                }
            }

            FPCollider2D[] tsColliders2D = go.GetComponentsInChildren<FPCollider2D>();
            if (tsColliders2D != null)
            {
                for (int index = 0, length = tsColliders2D.Length; index < length; index++)
                {
                    FPCollider2D tsCollider2D = tsColliders2D[index];
                    DestroyFPRigidBody(tsCollider2D.gameObject, tsCollider2D.Body);
                }
            }
            RemoveSyncBehaviour(go);
            Destroy(go, delayTime);
        }
    }

    /**
     * @brief Disables 'OnSyncedInput' and 'OnSyncUpdate' calls to every {@link IFrameSyncBehaviour} attached.
     **/
    public static void SyncedDisableBehaviour(GameObject gameObject)
    {
        MonoBehaviour[] monoBehaviours = gameObject.GetComponentsInChildren<MonoBehaviour>();

        for (int index = 0, length = monoBehaviours.Length; index < length; index++)
        {
            MonoBehaviour tsb = monoBehaviours[index];

            int i = -1;

            if (tsb is IFrameSyncBehaviour && (i = instance.GetBehaviourIndex(instance.ListManagedBehaviors, (IFrameSyncBehaviour)tsb)) >= 0)

            {
                instance.ListManagedBehaviors[i].Value.disabled = true;
            }
        }
    }

    private static void InitializeGameObject(GameObject go, FPVector position, FPQuaternion rotation)
    {
        ICollider[] tsColliders = go.GetComponentsInChildren<ICollider>();
        if (tsColliders != null)
        {
            for (int index = 0, length = tsColliders.Length; index < length; index++)
            {
                PhysicsManager.instance.AddBody(tsColliders[index]);
            }
        }

        FPTransform rootFPTransform = go.GetComponent<FPTransform>();
        if (rootFPTransform != null)
        {
            rootFPTransform.Initialize();

            rootFPTransform.position = position;
            rootFPTransform.rotation = rotation;
        }

        FPTransform[] FPTransforms = go.GetComponentsInChildren<FPTransform>();
        if (FPTransforms != null)
        {
            for (int index = 0, length = FPTransforms.Length; index < length; index++)
            {
                FPTransform FPTransform = FPTransforms[index];

                if (FPTransform != rootFPTransform)
                {
                    FPTransform.Initialize();
                }
            }
        }

        FPTransform2D rootFPTransform2D = go.GetComponent<FPTransform2D>();
        if (rootFPTransform2D != null)
        {
            rootFPTransform2D.Initialize();

            rootFPTransform2D.position = new FPVector2(position.x, position.y);
            rootFPTransform2D.rotation = rotation.ToQuaternion().eulerAngles.z;
        }

        FPTransform2D[] FPTransforms2D = go.GetComponentsInChildren<FPTransform2D>();
        if (FPTransforms2D != null)
        {
            for (int index = 0, length = FPTransforms2D.Length; index < length; index++)
            {
                FPTransform2D FPTransform2D = FPTransforms2D[index];

                if (FPTransform2D != rootFPTransform2D)
                {
                    FPTransform2D.Initialize();
                }
            }
        }
    }


    /**
     * @brief Removes objets related to a provided player.
     * 
     * @param playerId Target player's id.
     **/
    public static void RemovePlayer(int playerId)
    {
        if (instance != null /*&& instance.lockstep != null*/)
        {
            List<FrameSyncManagedBehaviour> behaviorsList = instance.behaviorsByPlayer[playerId];

            for (int index = 0, length = behaviorsList.Count; index < length; index++)
            {
                FrameSyncManagedBehaviour tsmb = behaviorsList[index];
                tsmb.disabled = true;

                FPCollider[] tsColliders = ((FrameSyncBehaviour)tsmb.FrameSyncBehavior).gameObject.GetComponentsInChildren<FPCollider>();
                if (tsColliders != null)
                {
                    for (int index2 = 0, length2 = tsColliders.Length; index2 < length2; index2++)
                    {
                        FPCollider tsCollider = tsColliders[index2];

                        if (!tsCollider.Body.TSDisabled)
                        {
                            DestroyFPRigidBody(tsCollider.gameObject, tsCollider.Body);
                        }
                    }
                }

                FPCollider2D[] tsCollider2Ds = ((FrameSyncBehaviour)tsmb.FrameSyncBehavior).gameObject.GetComponentsInChildren<FPCollider2D>();
                if (tsCollider2Ds != null)
                {
                    for (int index2 = 0, length2 = tsCollider2Ds.Length; index2 < length2; index2++)
                    {
                        FPCollider2D tsCollider2D = tsCollider2Ds[index2];

                        if (!tsCollider2D.Body.TSDisabled)
                        {
                            DestroyFPRigidBody(tsCollider2D.gameObject, tsCollider2D.Body);
                        }
                    }
                }
            }
            instance.behaviorsByPlayer.Remove(playerId);
        }
    }



    /**
     * @brief The related GameObject is firstly set to be inactive then in a safe moment it will be destroyed.
     * 
     * @param rigidBody Instance of a {@link FPRigidBody}
     **/
    private static void DestroyFPRigidBody(GameObject tsColliderGO, IBody body)
    {
        instance.OnRemovedRigidBody(body);

        tsColliderGO.gameObject.SetActive(false);
    }

    /**
     * @brief Registers an implementation of {@link IFrameSyncBehaviour} to be included in the simulation.
     * 
     * @param FrameSyncBehaviour Instance of an {@link IFrameSyncBehaviour}
     **/
    public static void RegisterIFrameSyncBehaviour(IFrameSyncBehaviour FrameSyncBehaviour)
    {
        if (instance != null /*&& instance.lockstep != null*/)
        {
            instance.ListManagedBehaviors.Add(new KeyValuePair<IFrameSyncBehaviour, FrameSyncManagedBehaviour>(
                FrameSyncBehaviour, instance.NewManagedBehavior(FrameSyncBehaviour)));
        }
    }

    private void Awake()
    {
        instance = this;

        lockedTimeStep = FrameSyncGlobalConfig.lockedTimeStep;

        FPRandom.Init();
    }

    // Use this for initialization
    void Start () {
        PhysicsManager.New(Config);
        PhysicsManager.instance.LockedTimeStep = Config.lockedTimeStep;
        PhysicsManager.instance.Init();

        CreatePlayer();

    }

    FP duration;
    private void FixedUpdate()
    {
        duration += Time.deltaTime;

        if(duration >= DeltaTime)
        {
            duration = 0;
            OnUpateInputData();
        }
    }

    // Update is called once per frame
    void Update () {


        renderTime += Time.deltaTime;

        if(renderTime >= timeSlice)
        {
            renderTime = 0;


            if (SpaceData.Instance.frameList.Count > 0)
            {
                int count = SpaceData.Instance.frameList.Count;
                timeSlice = DeltaTime / (count <= ThresholdFrame ? 1 : count / ThresholdFrame);

                currFrame = SpaceData.Instance.frameList.Dequeue();
                
                Dictionary<int, InputDataBase> dicInputData = new Dictionary<int, InputDataBase>();
                for (int i = 0; i < SpaceData.Instance.SpacePlayers.Count; i++)
                {
                    InputData data = new InputData();
                    data.ownerID = SpaceData.Instance.SpacePlayers[i].ownerID;
                    dicInputData[data.ownerID] = data;
                }

                for (int i = 0; i < currFrame.operation.Count; i++)
                {
                    FS_ENTITY_DATA e = currFrame.operation[i];
                    InputData data = new InputData();
                    data.Deserialize(e);
                    dicInputData[data.ownerID] = data;
                }

                CheckQueuedBehaviours();
                OnStepUpdate(dicInputData.Values.ToList());
                PhysicsManager.instance.UpdateStep();
            }
        }
	}

    void GetLocalData(InputDataBase playerInputData)
    {
        FrameSyncInput.CurrentInputData = (InputData)playerInputData;

        if (behaviorsByPlayer.ContainsKey(playerInputData.ownerID))
        {
            List<FrameSyncManagedBehaviour> managedBehavioursByPlayer = behaviorsByPlayer[playerInputData.ownerID];
            for (int index = 0, length = managedBehavioursByPlayer.Count; index < length; index++)
            {
                FrameSyncManagedBehaviour bh = managedBehavioursByPlayer[index];

                if (bh != null && !bh.disabled)
                {
                    bh.OnSyncedInput();
                }
            }
        }

        FrameSyncInput.CurrentInputData = null;
    }

    void OnUpateInputData()
    {
        InputData data = new InputData();

        data.ownerID = SpaceData.Instance.localPlayer.ownerID;

        GetLocalData(data);

        if(data.Count > 0)
        {
            KBEngine.Event.fireIn("reportFrame", data.Serialize());
        }
        
    }



    void OnStepUpdate(List<InputDataBase> allInputData)
    {
        FrameSyncInput.SetAllInputs(allInputData);

        FrameSyncInput.CurrentSimulationData = null;

        for (int index = 0, length = allInputData.Count; index < length; index++)
        {
            InputDataBase playerInputData = allInputData[index];

            if (behaviorsByPlayer.ContainsKey(playerInputData.ownerID))
            {
                FrameSyncInput.CurrentSimulationData = (InputData)playerInputData;

                List<FrameSyncManagedBehaviour> managedBehavioursByPlayer = behaviorsByPlayer[playerInputData.ownerID];
                for (int index2 = 0, length2 = managedBehavioursByPlayer.Count; index2 < length2; index2++)
                {
                    FrameSyncManagedBehaviour bh = managedBehavioursByPlayer[index2];

                    if (bh != null && !bh.disabled)
                    {
                        bh.OnSyncedUpdate();
                    }
                }
            }

            FrameSyncInput.CurrentSimulationData = null;
        }

        for (int index = 0; index < ListManagedBehaviors.Count; index++)
        {
            FrameSyncManagedBehaviour bh = ListManagedBehaviors[index].Value;

            if (bh != null && !bh.disabled)
            {
                bh.OnSyncedUpdate();
            }
        }
    }

    private void OnRemovedRigidBody(IBody body)
    {
        GameObject go = PhysicsManager.instance.GetGameObject(body);

        if (go != null)
        {
            PhysicsManager.instance.RemoveBody(body);

            RemoveSyncBehaviour(go);
        }
    }

    private void RemoveFromTSMBList(List<FrameSyncManagedBehaviour> fsmbList, List<FrameSyncBehaviour> behaviours)
    {
        List<FrameSyncManagedBehaviour> toRemove = new List<FrameSyncManagedBehaviour>();
        for (int j = 0; j < fsmbList.Count; j++)
        {
            FrameSyncManagedBehaviour fsmb = fsmbList[j];

            if ((fsmb.FrameSyncBehavior is FrameSyncBehaviour) && behaviours.Contains((FrameSyncBehaviour)fsmb.FrameSyncBehavior))
            {
                toRemove.Add(fsmb);
            }
        }

        for (int i = 0; i < toRemove.Count; i++)
        {
            FrameSyncManagedBehaviour tsmb = toRemove[i];
            fsmbList.Remove(tsmb);
        }
    }

    private void CheckQueuedBehaviours()
    {
        foreach (var bhList in behaviorsByPlayer)
        {          
            for (int i = 0; i < bhList.Value.Count; i++)
            {
                FrameSyncManagedBehaviour playerBehaviour = (bhList.Value)[i];
                playerBehaviour.OnSyncedStart();
            }
        }

        for (int j = 0; j < ListManagedBehaviors.Count; j++)
        {
            FrameSyncManagedBehaviour fsmb = ListManagedBehaviors[j].Value;
            fsmb.OnSyncedStart();
        }
    }

}
