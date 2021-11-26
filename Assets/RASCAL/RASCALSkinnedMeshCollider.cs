using System.Collections;
using System;
using System.Linq;
using System.Threading;
using System.Collections.Generic;
using UnityEngine;
using RASCAL;

#if NET_4_6 || CSHARP_7_3_OR_NEWER
using System.Threading.Tasks;
#endif

[AddComponentMenu("Physics/RASCAL Skinned Mesh Collider", 0)]
[ExecuteInEditMode]
public class RASCALSkinnedMeshCollider : MonoBehaviour {

    [Header("Generation")]

    [Tooltip("This will process and generate all necessary data to create the bone-meshes used in collision and also generate some initial collision shapes when the game starts.")]
    public bool generateOnStart = true;

    [Tooltip("This will enable continuous asynchronous updating of collision shapes when the game starts. You may find that the initial collision shapes work well enough " +
        "for you and you dont even need to update them which would save on performance for sure, but if the mesh deforms due to its bones being moved the shapes wont match " +
        "and could cause some pretty big inaccuracies. Alternatively, if you know your mesh will only need to be updated at certain points, you can call either immediate or " +
        "asynchronous reconstruction of the collision shapes manually via the provided functions in the script.")]
    public bool enableUpdatingOnStart = false;

    [Tooltip("This generates collision on startup immediately, rather than using the asynchronous method which could take second or two to fully generate. " +
        "This obviously comes at the cost of longer initial game load times. " +
        "If you don't need the collision immediately within a few seconds of the game loading, it is recommended not to enable this.")]
    public bool immediateStartupCollision = false;

#if UNITY_2019_3_OR_NEWER
    [Tooltip("Uses a new feature in Unity to bake collision mesh data manually on a separate thread. " +
        "The baking step is the most costly step of updating meshes, so enabling this leaves the main thread free to do more of the stuff that requires it. " +
        "However this comes at a moderate tradeoff in that each individual collider will take longer to update due to the overhead threading. " +
        "On a modern computer this is still probably faster overall just because multiple different colliders can be processed at a time on separate threads.")]
    public bool useThreadedColMeshBaking = true;
#endif

    [Tooltip("When enabled, only unique triangles will be used between all the bone-meshes. This prevents mesh overlapping but because of how triangles are chosen " +
        "without any care for which bone the triangle is more significant to, this can lead to messy bone-meshes that may impact results of certain collisions. It's bit faster and uses less memory, " +
        "this option should probably be on unless you notice some otherwise bad collision meshes being generated as a result of it.")]
    public bool onlyUniqueTriangles = true;

    [Tooltip("Enable the convex setting of the mesh colliders. This option obviously leads to inaccuracies in the overall mesh and you may need to lower the max polygons per mesh to avoid errors, " +
        "but convex meshes should allow for non-kinematic rigid bodies to be used if that's something that you need.")]
    public bool convexMeshColliders = false;

    [Tooltip("Splits each collision mesh up by material. For example if your skinned mesh has 2 materials it will create 1 collider for all triangles with the first material, and another for the second material. " +
        "This is useful and required mostly for applying different physics materials to the colliders based on material or excluding mesh parts based on material. To do that, use the material association list and " +
        "the exclusion list.")]
    public bool splitCollisionMeshesByMaterial = false;

    [Tooltip("You almost certainly don't need this. But it's included just in case. It basically makes it so meshes with no bones and only blendshapes get transformed differently. " +
        "But it should be fine by default, this should be a last resort troubleshooting step.")]
    public bool zeroBoneMeshAlternateTransform = false;

    [Tooltip("Clears ALL mesh colliders under the component when calling the clear function, not just colliders currently associated with this component. Be careful with this.")]
    public bool clearAllMeshColliders = false;


    [Tooltip("Material to apply to the mesh colliders. If you need more granular control for materials on what bones youll need to add a RASCALProperties component to the bone transform or skinned-mesh transform. " +
        "The priority goes from highest to lowest: Material-Association-List -> Bone-Transform -> SkinnedMesh-Transform -> PhysicsMaterial-Variable")]
    public PhysicMaterial physicsMaterial = null;

    [Tooltip("The maximum number of triangles/polygons to allow in each collision shape. The mesh will be split up into multiple mesh colliders if it contains more than this amount. " +
        "Splitting large meshes up can help with asynchronous collision generate frame rate stability as large meshes may cause hitching when updating mesh colliders. " +
        "Splitting the mesh up however can very slightly make it take longer to update all collision shapes, this is probably negligible though.")]
    public int maxColliderTriangles = 5000;

    [Tooltip("When generating colliders, the bone weight for the vertex must be above this for the vertex to be added to the collider.")]
    public float boneWeightThreshold = 0f;

    [Tooltip("Use this list to associate materials in the skinned mesh(es) with physics materials that will be added to the mesh colliders. If your mesh has multiple materials make sure to check the option to " +
        "split the collision meshes by material.")]
    public List<PhysicsMaterialAssociation> materialAssociationList = new List<PhysicsMaterialAssociation>();



    [Header("Updating")]

    [Tooltip("Amount of time in milliseconds the asynchronous generation should be allowed to run per frame while idling. It is idling when the mesh isnt changing enough " +
        "to warrant rebuilding the collision shapes. (this setting doesn't affect immediate updating)")]
    public double idleCpuBudget = 0.2;

    [Tooltip("Amount of time in milliseconds the asynchronous generation should be allowed to run per frame while active. It is active when any of the collision shapes are actively being rebuilt. " +
        "This allows more time to rebuild collision shapes which means they will update faster at the cost of performance. (this setting doesn't affect immediate updating)")]
    public double activeCpuBudget = 1;

    [Tooltip("An amount by which the bone mesh needs to change in order for its collision to be rebuilt. The purpose of this is to allow for slight changes " +
        "in the mesh before comepletely rebuilding, which slightly improves performance at the cost of accuracy. The value for this should likely be quite small but you should play around with it to see what works best for you.")]
    public float meshUpdateThreshold = 0.02f;
    private float updateThreshold;



    [Serializable]
    public class PhysicsMaterialAssociation {
        public Material material;
        public PhysicMaterial physicsMaterial;
    }

    public List<SkinnedMeshRenderer> excludedSkins = new List<SkinnedMeshRenderer>();
    public List<Transform> excludedBones = new List<Transform>();
    public List<Material> excludedMaterials = new List<Material>();


    [HideInInspector]
    public Skinfo[] skinfos = null;

    public bool noMeshData { get { return skinfos == null || skinfos.Length == 0; } }

    public bool processed { get; private set;} = false;
    public bool processing { get; private set; } = false;


#if NET_4_6 || CSHARP_7_3_OR_NEWER
    private async void Start() {
#else
    private void Start() {
#endif

        if(Application.isPlaying) {
            if(generateOnStart) {
                if(immediateStartupCollision) {
                    ProcessMesh();
                    ImmediateUpdateColliders(true);

                    if(enableUpdatingOnStart) {
                        StartAsyncUpdating(true);
                    }
                } else {
#if NET_4_6 || CSHARP_7_3_OR_NEWER
                    await ProcessMeshAsync();
#else
                    ProcessMesh();
#endif
                    StartAsyncUpdating(enableUpdatingOnStart);
                }
            } else if(!noMeshData) {
                SetBoneParents();
                if(enableUpdatingOnStart) {
                    StartAsyncUpdating(true);
                }
            }
        } else {
            CleanUpMeshes();
        }
    }




    //--------------------------------------------------------------------------------
    //------------------------PROCESSING STUFF---------------------------------------
    //--------------------------------------------------------------------------------


    /// <summary>
    /// Process the skinned mesh(es) to calculate required data and build the data structure
    /// </summary>
    public void ProcessMesh(bool cleanOld = true) {
        ProcessInternal(cleanOld, false);
    }

    /// <summary>
    /// Process the skinned mesh(es) to calculate required data and build the data structure.
    /// <para>This method is run asynchronously.</para>
    /// </summary>
#if NET_4_6 || CSHARP_7_3_OR_NEWER
    public async Task ProcessMeshAsync(bool cleanOld = true) {
        await ProcessInternal(cleanOld, true);
    }
#endif


    const int awaitDelay = 10;

#if NET_4_6 || CSHARP_7_3_OR_NEWER
    async Task ProcessInternal(bool cleanOld, bool async) {
#else
    void ProcessInternal(bool cleanOld, bool async) {
#endif

        if(processing) {
            Debug.LogError($"RASCAL: {this} is already being processed.");
            return;
        }

        processed = false;
        processing = true;
        if(cleanOld) CleanUpMeshes();


        HashSet<SkinnedMeshRenderer> skins = new HashSet<SkinnedMeshRenderer>(
            GetComponentsInChildren<SkinnedMeshRenderer>()
        );

        SkinnedMeshRenderer thisSkin; if(thisSkin = GetComponent<SkinnedMeshRenderer>()) skins.Add(thisSkin);


        skinfos = skins.Where(skin => !excludedSkins.Contains(skin)).Select(skin => new Skinfo() { skinnedMesh = skin, host = this }).ToArray();

        var timer = new System.Diagnostics.Stopwatch();

        foreach(Skinfo skin in skinfos) {
            foreach(bool result in skin.ProcessWeights()) {
#if NET_4_6 || CSHARP_7_3_OR_NEWER
                if(async && !result && timer.Elapsed.TotalMilliseconds > activeCpuBudget) {
                    await Task.Delay(awaitDelay);
                    timer.Restart();
                }
#endif
            }
        }

        foreach(Skinfo skin in skinfos) {
#if NET_4_6 || CSHARP_7_3_OR_NEWER
            await skin.Init(async);
#else
            skin.Init(async);
#endif
        }



        if(skinfos.Length == 0) {
            Debug.Log("No skinned meshes were found under object: " + gameObject);
        } else {

            if(!this) {
                //due to async stuff we definitely need to make sure the scene wasnt stopped or that the component wasnt otherwise destroyed
                //lest we accidentally create unreferenced mesh colliders in edit mode or something
                return;
            }

            //apply processing separately so that we can yield at points since its possible this step takes long enough to cause hitching
            foreach(bool result in ApplyProcessingRoutine(async)) {
#if NET_4_6 || CSHARP_7_3_OR_NEWER
                if(!result && timer.Elapsed.TotalMilliseconds > activeCpuBudget) {
                    await Task.Delay(awaitDelay);
                    timer.Restart();
                }
#endif
            }

        }


        processed = true;
        processing = false;
    }



    IEnumerable<bool> ApplyProcessingRoutine(bool async) {
        foreach(Skinfo skin in skinfos) {

            Vector3[] sNorms = skin.skinnedMesh.sharedMesh.normals;
            List<Vector2> sUVs1 = new List<Vector2>(); skin.skinnedMesh.sharedMesh.GetUVs(0, sUVs1);
            List<Vector2> sUVs2 = new List<Vector2>(); skin.skinnedMesh.sharedMesh.GetUVs(1, sUVs2);
            List<Vector2> sUVs3 = new List<Vector2>(); skin.skinnedMesh.sharedMesh.GetUVs(2, sUVs3);
            List<Vector2> sUVs4 = new List<Vector2>(); skin.skinnedMesh.sharedMesh.GetUVs(3, sUVs4);

            foreach(BoneInfo boneInfo in skin.bones) {
                foreach(BoneMeshColl bone in boneInfo.boneMeshes) {
                    bone.Init();

                    bone.CopyMeshNormals(sNorms);

                    bone.CopyMeshUVs(sUVs1, 0);
                    bone.CopyMeshUVs(sUVs2, 1);
                    bone.CopyMeshUVs(sUVs3, 2);
                    bone.CopyMeshUVs(sUVs4, 3);
                }
                if(async) yield return false;
            }
        }

        yield return true;
    }





    //--------------------------------------------------------------------------------
    //------------------------UPDATING STUFF---------------------------------------
    //--------------------------------------------------------------------------------


    /// <summary>
    /// Immediately updates all colliders.
    /// </summary>
    /// <param name="force">If true, will override the meshUpdateThreshold and force the update.</param>
    public void ImmediateUpdateColliders(bool force = false) {
        if(noMeshData) {
            Debug.Log("No skinned collision mesh data found on " + this.ToString() + ". Processing mesh...");
            ProcessMesh();

        }else if(processing) {
            Debug.LogError($"RASCAL: {this} update colliders called while still processing.");
            return;
        }

        updateThreshold = meshUpdateThreshold * meshUpdateThreshold;
        foreach(var skin in skinfos) skin.Update(force);
    }



    double timerAcc = 0;
    double totalTimeAcc = 0;
    System.Diagnostics.Stopwatch timer = new System.Diagnostics.Stopwatch();

    public delegate void RASCALTimedEvent(double time);
    public event RASCALTimedEvent OnAsyncUpdateYield;
    public event RASCALTimedEvent OnAsyncPassComplete;

    private void AddTime() {
        timerAcc += timer.LapTime();
    }

    private bool CheckFrameYield(double budget) {
        AddTime();
        return timerAcc > budget;
    }

    //double avgCpuDeficit = 0;
    //const double asyncI = 350;
    //const double asyncI2 = asyncI + 1;

    private void ResetTimer(bool doEvent = true) {
        timer.Restart();

        //some weird code measuring how overbudget we went on calculations
        //resetting the timer accumulator to the deficit may help stabilize frame times but im not sure
        //also some stuff for calculating a rolling average of the deficit for diagnostic purposes

        //double deficit = timerAcc - cpuBudget;
        //avgCpuDeficit = (avgCpuDeficit * asyncI + deficit) / asyncI2;
        //timerAcc = deficit;

        if(OnAsyncUpdateYield != null) OnAsyncUpdateYield(timerAcc);

        totalTimeAcc += timerAcc;
        timerAcc = 0;
    }

    [NonSerialized] public bool asyncUpdating = false;

    private Coroutine updatingCoroutine;
    /// <summary>
    /// Starts the asynchronous updating routine, using the CPU budget values.
    /// </summary>
    /// <param name="continuous">If true, will run continuously until stopped, otherwise only runs once.</param>
    /// <returns>A reference to the started coroutine.</returns>
    public Coroutine StartAsyncUpdating(bool continuous) {
        if(noMeshData) {
            Debug.Log("No skinned collision mesh data found on " + this.ToString() + ". Make sure you process the mesh first!");
            return null;
        } else {
            StopAsyncUpdating();
            if(this) {
                //the if(this) check seems silly but its possible this component has been destroyed from stopping the scene
                //and due to the async processing stage, this might get called after its been destroyed, causing errors and stuff.
                updatingCoroutine = StartCoroutine(AsynchronousUpdate(continuous));
            }
            return updatingCoroutine;
        }
    }

    /// <summary>
    /// Stops the asynchronous updating coroutine.
    /// </summary>
    public void StopAsyncUpdating() {
        if(updatingCoroutine != null) {
            StopCoroutine(updatingCoroutine);
        }
        asyncUpdating = false;
    }


    //Psst, hey, if youre reading this, I hate the way this function looks too
    //All the pre-processor checks really ruin it but too lazy to split it up better or something
    //Could just require a higher unity version to avoid all this but im stubborn and want really old Unity compatibility for no good reason
    //You may also be wondering "why NET_4_6 OR CSHARP_7_3_OR_NEWER"
    //Well, annoyingly unity doesnt have a single keyword that encompasses the async/task capability
    //So this was the best I could do. also annoyingly, C# does not allow for the creation of preprocessor macros
    private IEnumerator AsynchronousUpdate(bool continuous) {
        asyncUpdating = true;

        while(processing) {
            yield return null;
        }

        do {
            if(enabled) {
                updateThreshold = meshUpdateThreshold * meshUpdateThreshold;

                foreach(Skinfo skinfo in skinfos) {
#if NET_4_6 || CSHARP_7_3_OR_NEWER
                    if(skinfo.updateTask != null && !skinfo.updateTask.IsCompleted) {
                        yield return new RASCAL.TaskYield(skinfo.updateTask);
                    }
#endif

                    skinfo.skinnedMesh.BakeMesh(skinfo.bakedMesh);
                    skinfo.UpdateRootMatrix();

                    foreach(BoneInfo bone in skinfo.bones) {
                        bone.cachedMatrix = bone.transform.worldToLocalMatrix;
                    }

                    if(CheckFrameYield(idleCpuBudget)) { yield return new WaitForFixedUpdate(); ResetTimer(); }

#if UNITY_5_5_OR_NEWER
                    skinfo.bakedMesh.GetVertices(skinfo.bakedVerts);
#else
                    skinfo.bakedVerts.Clear();
                    skinfo.bakedVerts.AddRange(skinfo.bakedMesh.vertices);
#endif



#if NET_4_6 || CSHARP_7_3_OR_NEWER
                    skinfo.updateTask = Task.Run(() => skinfo.TransformBones());
                    if(!continuous) {
                        yield return new RASCAL.TaskYield(skinfo.updateTask);
                    } else {
                        yield return new WaitForFixedUpdate();
                    }
#endif
                    //skinfo.TransformBones();

                    foreach(BoneInfo bone in skinfo.bones) {
                        foreach(BoneMeshColl boneMesh in bone.boneMeshes) {

#if !NET_4_6 && !CSHARP_7_3_OR_NEWER
                            boneMesh.TransformVertices(skinfo.bakedVerts);
                            if(CheckFrameYield(activeCpuBudget)) { yield return new WaitForFixedUpdate(); ResetTimer(); }
                            boneMesh.isPastThreshold = boneMesh.PastThreshold(boneMesh.transformedVerts);
                            if(CheckFrameYield(activeCpuBudget)) { yield return new WaitForFixedUpdate(); ResetTimer(); }
#endif

                            if(boneMesh.isPastThreshold) {

#if UNITY_2019_3_OR_NEWER
                                if(!useThreadedColMeshBaking) {
#endif
                                boneMesh.collMesh.vertices = boneMesh.transformedVerts;
                                boneMesh.isPastThreshold = false;

                                if(CheckFrameYield(activeCpuBudget)) { yield return new WaitForFixedUpdate(); ResetTimer(); }

                                boneMesh.meshCol.sharedMesh = boneMesh.collMesh;


#if UNITY_2019_3_OR_NEWER
                                } else {
                                    if(boneMesh.bakeTask != null && !boneMesh.bakeTask.IsCompleted) {
                                        yield return new RASCAL.TaskYield(boneMesh.bakeTask);
                                    }
                                    //just let this run asynchronously, the above yield makes sure its done by the next go around
                                    boneMesh.BakeMeshThreaded(convexMeshColliders);
                                }
#endif

                                if(CheckFrameYield(activeCpuBudget)) { yield return new WaitForFixedUpdate(); ResetTimer(); }
                            }
                        }
                    }
                }

                if(CheckFrameYield(idleCpuBudget)) {
                    yield return new WaitForFixedUpdate();
                    ResetTimer();
                } else {
                    AddTime();
                    yield return new WaitForFixedUpdate();
                }


                totalTimeAcc += timerAcc;
                if(OnAsyncPassComplete != null) {
                    OnAsyncPassComplete.Invoke(totalTimeAcc);
                }
                totalTimeAcc = 0;


            } else {
                yield return new WaitForFixedUpdate();
            }

        } while(asyncUpdating && continuous);

        asyncUpdating = false;
    }



    //--------------------------------------------------------------------------------
    //------------------------OTHER STUFF---------------------------------------
    //--------------------------------------------------------------------------------

    [ContextMenu("Generate (READ DOC)")]
    void MenuGenerate() {
        ProcessMesh();
        ImmediateUpdateColliders(true);
    }


    /// <summary>
    /// This just initializes the children of the bone tree with their parents for convenience.
    /// Has to be like this because otherwise you run into cyclical reference issues during serialization.
    /// Also sets up the baked mesh as a new empty mesh just because.
    /// </summary>
    void SetBoneParents() {
        foreach(var skin in skinfos) {
            skin.bakedMesh = new Mesh();
            foreach(var bone in skin.bones) {
                bone.parentSkinfo = skin;
                foreach(var boneMesh in bone.boneMeshes) {
                    boneMesh.parentBone = bone;
                }
            }
        }
    }

    //you probably dont want this bit on but it might be useful if you find yourself in a pickel
    /// <summary>
    /// Removes ALL mesh collider components under the RASCAL component. Perhaps useful for cleaning up colliders created by glitch or mistake.
    /// </summary>
    public void CleanUpAllMeshColliders() {
        MeshCollider[] mcs = GetComponentsInChildren<MeshCollider>();

        foreach(var mc in mcs) {
#if UNITY_EDITOR
            if(Application.isPlaying) {
                DestroyImmediate(mc);
            } else {
                UnityEditor.Undo.DestroyObjectImmediate(mc);
            }
#else
            DestroyImmediate(mc);
#endif
        }
    }

    /// <summary>
    /// Cleans up colliders and other data associated with this RASCAL component.
    /// </summary>
    public void CleanUpMeshes() {

        if(noMeshData) {
            if(clearAllMeshColliders) CleanUpAllMeshColliders();
            return;
        }

        if(updatingCoroutine != null) {
            asyncUpdating = false;
            StopCoroutine(updatingCoroutine);
        }

        foreach(var skin in skinfos) {
            DestroyImmediate(skin.bakedMesh);
            if(skin.bones == null) continue;

            foreach(var bone in skin.bones) {
                foreach(var boneMesh in bone.boneMeshes) {
                    DestroyImmediate(boneMesh.collMesh);
                    DestroyImmediate(boneMesh.meshCol, true);
                }
                bone.boneMeshes = null;
            }
            skin.bones = null;
        }
        skinfos = null;

        if(clearAllMeshColliders) CleanUpAllMeshColliders();
    }


    /// <summary>
    /// Allows for instantiation of an already processed mesh with a RASCAL component.
    /// <para>Call this after instantiating your mesh object that has already been processed to copy the processed data to this instance.</para>
    /// <para>May cause a hitch. Allocates memory.</para>
    /// </summary>
    public void FixInstantiated(RASCALSkinnedMeshCollider source) {

        if(!source.processed || source.processing) {
            Debug.LogError("RASCAL FixInstantiated: Source mesh not processed.");
            return;
        }

        for(int i = 0; i < skinfos.Length; i++) {
            var skin = skinfos[i];
            var srcSkin = source.skinfos[i];
            
            skin.host = this;
            skin.bakedVerts = new List<Vector3>(skin.skinnedMesh.sharedMesh.vertexCount);

            for(int bI = 0; bI < skin.bones.Length; bI++) {
                var bone = skin.bones[bI];
                var srcBone = srcSkin.bones[bI];

                bone.parentSkinfo = skin;


                for(int bcI = 0; bcI < bone.boneMeshes.Count; bcI++) {
                    var col = bone.boneMeshes[bcI];
                    var srcCol = srcBone.boneMeshes[bcI];


                    col.parentBone = bone;

                    //copy the mesh since it needs to be unique
                    col.collMesh = Instantiate(srcCol.collMesh);
                    col.collMeshInstID = col.collMesh.GetInstanceID();

                    //can reuse these
                    col.tris = srcCol.tris;
                    col.distinctVerts = srcCol.distinctVerts;

                    //reset transformed vert arrays
                    int vertexCount = col.vertexCount;
                    col.oldVerts = new Vector3[vertexCount];
                    col.transformedVerts = new Vector3[vertexCount];
                }
            }
        }

        processed = true;
        processing = false;
    }


    //--------------------------------------------------------------------------------
    //------------------------SKIN AND BONE STRUCTURE CLASSES-------------------------
    //--------------------------------------------------------------------------------


    [Serializable]
    public class Skinfo {
        public BoneInfo[] bones;
        public Mesh bakedMesh;
        public SkinnedMeshRenderer skinnedMesh;
        public RASCALSkinnedMeshCollider host;
        public RASCALProperties materialProperties;
        public Matrix4x4 meshRootMatrix = new Matrix4x4();
        public bool noBones = false;

        [NonSerialized] internal List<Vector3> bakedVerts;



#if NET_4_6 || CSHARP_7_3_OR_NEWER
        [NonSerialized] internal Task updateTask = null;
#endif

        internal IEnumerable<bool> ProcessWeights() {
            bakedMesh = new Mesh();
            materialProperties = skinnedMesh.GetComponent<RASCALProperties>();

            bool hasExclusionMap = false;
            if(hasExclusionMap = materialProperties && materialProperties.InitExclusionMap(skinnedMesh.sharedMesh)) {
                yield return false;
            }
            

            if(skinnedMesh.bones.Length == 0) {
                //handle a no-bone/single bone situation just in case
                noBones = true;
                BoneInfo tBone = new BoneInfo() { srcSkin = skinnedMesh, transform = skinnedMesh.transform, host = host, parentSkinfo = this }.Init();
                for(int v = 0; v < skinnedMesh.sharedMesh.vertexCount; v++) {
                    if(hasExclusionMap) {
                        if(materialProperties.GetExcludedVert(v)) {
                            if(v % 2000 == 0) {
                                yield return false;
                            }
                            continue;
                        }
                    }
                    tBone.affectedVerts.Add(v);
                }

                bones = new BoneInfo[] { tBone };
            } else {


                List<BoneInfo> tBones = skinnedMesh.bones.Where(x => x != null).Select(x => new BoneInfo() {
                    srcSkin = skinnedMesh, transform = x, host = host, parentSkinfo = this
                }.Init()).ToList();

                BoneInfo tBone;
                BoneWeight[] weights = skinnedMesh.sharedMesh.boneWeights;

                //v is vertex index in the mesh
                for(int v = 0; v < weights.Length; v++) {

                    if(hasExclusionMap) {
                        if(materialProperties.GetExcludedVert(v)) {
                            if(v % 2000 == 0) {
                                yield return false;
                            }
                            continue;
                        }
                    }

                    var weight = weights[v];

                    //find which bone is weighted the most to this vertex
                    Weight wInfo = new Weight() { weight = weight.weight0, boneIndex = weight.boneIndex0 };
                    if(weight.weight1 > wInfo.weight) {
                        wInfo.weight = weight.weight1;
                        wInfo.boneIndex = weight.boneIndex1;
                    }
                    if(weight.weight2 > wInfo.weight) {
                        wInfo.weight = weight.weight2;
                        wInfo.boneIndex = weight.boneIndex2;
                    }
                    if(weight.weight3 > wInfo.weight) {
                        wInfo.weight = weight.weight3;
                        wInfo.boneIndex = weight.boneIndex3;
                    }

                    tBone = tBones[wInfo.boneIndex];

                    if(tBone.materialProperties) {
                        if(wInfo.weight > tBone.materialProperties.boneWeightThreshold) {
                            tBone.affectedVerts.Add(v);
                        }
                    } else if(materialProperties) {
                        if(wInfo.weight > materialProperties.boneWeightThreshold) {
                            tBone.affectedVerts.Add(v);
                        }
                    } else if(wInfo.weight > host.boneWeightThreshold) {
                        tBone.affectedVerts.Add(v);
                    }

                    if(v % 2000 == 0) {
                        //yield so that the verts can be processed in chunks and yielding in a coroutine to avoid hitching
                        yield return false;
                    }
                }

                bones = tBones.Where(b => b.affectedVerts.Count > 0 && !host.excludedBones.Contains(b.transform)).ToArray();
            }

            if(hasExclusionMap) {
                materialProperties.FinishExlusions();
            }

            yield return true;
        }

#if NET_4_6 || CSHARP_7_3_OR_NEWER
        internal async Task Init(bool async) {
#else
        internal void Init(bool async) {
#endif


            bakedVerts = new List<Vector3>(skinnedMesh.sharedMesh.vertexCount);


            List<int> skinTriList;
            int subMeshIdx = 0;
            int subMeshCount = skinnedMesh.sharedMesh.subMeshCount;

            boneStart:
            if(host.splitCollisionMeshesByMaterial) {
                skinTriList = skinnedMesh.sharedMesh.GetTriangles(subMeshIdx).ToList();
            } else {
                skinTriList = skinnedMesh.sharedMesh.triangles.ToList();
            }


            if(!host.excludedMaterials.Contains(skinnedMesh.sharedMaterials[subMeshIdx])) {

                Action processTask = new Action(() => {
                    //try {


                    foreach(BoneInfo boneInfo in bones) {
                        List<int> listTris = new List<int>();
                        for(int t = 0; t < skinTriList.Count;) {
                            int[] triSet = new int[] { skinTriList[t++], skinTriList[t++], skinTriList[t++] };

                            if(boneInfo.affectedVerts.Contains(triSet[0]) || boneInfo.affectedVerts.Contains(triSet[1]) || boneInfo.affectedVerts.Contains(triSet[2])) {
                                if(host.onlyUniqueTriangles) {
                                    t -= 3;
                                    skinTriList.RemoveRange(t, 3);
                                }

                                listTris.AddRange(triSet);
                            }
                        }

                        if(listTris.Count == 0) {
                            continue;
                        }



                        if(host.convexMeshColliders && listTris.Count <= 3) {
                            continue;
                        }


                        int polyCount = listTris.Count / 3;
                        BoneMeshColl boneMesh = new BoneMeshColl() { parentBone = boneInfo, host = host, skinnedMeshMaterialIndex = subMeshIdx };
                        boneInfo.boneMeshes.Add(boneMesh);

                        if(polyCount > host.maxColliderTriangles) {
                            int meshGroups = Mathf.CeilToInt((float)polyCount / host.maxColliderTriangles);

                            boneMesh.SetTris(listTris.Take(host.maxColliderTriangles * 3));

                            for(int i = 1; i < meshGroups; i++) {
                                BoneMeshColl tBoneMesh = new BoneMeshColl() { parentBone = boneInfo, host = host };

                                int skip = i * host.maxColliderTriangles * 3;

                                if(host.convexMeshColliders && listTris.Count - skip <= 3) {
                                    break;
                                }

                                tBoneMesh.SetTris(listTris.Skip(skip).Take(host.maxColliderTriangles * 3));
                                boneInfo.boneMeshes.Add(tBoneMesh);
                            }
                        } else {
                            boneMesh.SetTris(listTris);
                        }


                    }


                    //} catch(Exception ex) {
                    //    Debug.LogException(ex);
                    //}
                });

#if NET_4_6 || CSHARP_7_3_OR_NEWER
                if(async) {
                    await Task.Run(processTask);
                } else {
                    processTask();
                }
#else
                processTask();
#endif

            }


            if(host.splitCollisionMeshesByMaterial && subMeshIdx < subMeshCount - 1) {
                subMeshIdx++;
                goto boneStart;
            }

            bones = bones.Where(b => b.boneMeshes.Count > 0).ToArray();

            foreach(BoneInfo bone in bones) {
                bone.affectedVerts = null;
            }

            //return this;
        }

        public void UpdateRootMatrix() {
            meshRootMatrix.SetTRS(skinnedMesh.transform.position, skinnedMesh.transform.rotation, Vector3.one);
        }

        public void Update(bool force = false) {
            skinnedMesh.BakeMesh(bakedMesh);

#if UNITY_5_5_OR_NEWER
            bakedMesh.GetVertices(bakedVerts);
#else
            bakedVerts.Clear();
            bakedVerts.AddRange(bakedMesh.vertices);
#endif

            UpdateRootMatrix();
            foreach(BoneInfo bone in bones) {
                bone.Update(bakedVerts, force);
            }
        }


        public void TransformBones() {
            foreach(BoneInfo bone in bones) {
                foreach(BoneMeshColl boneMesh in bone.boneMeshes) {
                    boneMesh.TransformVertices(bakedVerts);
                    boneMesh.isPastThreshold = boneMesh.PastThreshold(boneMesh.transformedVerts);
                }
            }
        }
    }

    struct Weight {
        public int boneIndex;
        public float weight;
    }

    [Serializable]
    public class BoneInfo {
        public Transform transform;
        public Matrix4x4 cachedMatrix;
        public SkinnedMeshRenderer srcSkin;
        public HashSet<int> affectedVerts = new HashSet<int>();
        public List<BoneMeshColl> boneMeshes = new List<BoneMeshColl>();
        public RASCALSkinnedMeshCollider host;
        public RASCALProperties materialProperties;

        [NonSerialized] public Skinfo parentSkinfo;

        public BoneInfo Init() {
            materialProperties = transform.GetComponent<RASCALProperties>();
            return this;
        }

        public void Update(List<Vector3> verts, bool force = false) {
            cachedMatrix = transform.worldToLocalMatrix;
            foreach(BoneMeshColl boneMesh in boneMeshes) {
                boneMesh.Update(verts, force);
            }
        }

    }

    [Serializable]
    public class BoneMeshColl {
        public MeshCollider meshCol;
        public RASCALSkinnedMeshCollider host;
        public int skinnedMeshMaterialIndex;

        public int vertexCount { get { return distinctVerts.Length; } }

        [NonSerialized] public Mesh collMesh;

        [NonSerialized] public int[] tris;
        [NonSerialized] public int[] distinctVerts;

        [NonSerialized] public BoneInfo parentBone;

        [NonSerialized] internal Vector3[] oldVerts;
        [NonSerialized] internal Vector3[] transformedVerts;
        [NonSerialized] internal bool isPastThreshold = false;
        [NonSerialized] internal int collMeshInstID = -1;


        internal BoneMeshColl Init() {
            collMesh = new Mesh();

            meshCol = parentBone.transform.gameObject.AddComponent<MeshCollider>();
            meshCol.convex = host.convexMeshColliders;

            HandlePhysMatInheritence();

            collMesh.vertices = new Vector3[vertexCount];
            collMesh.triangles = tris;

            collMeshInstID = collMesh.GetInstanceID();
            collMesh.name = parentBone.transform.name + collMeshInstID;

            oldVerts = new Vector3[vertexCount];
            transformedVerts = new Vector3[vertexCount];

            return this;
        }

        void HandlePhysMatInheritence() {
            Material skinMat = parentBone.srcSkin.sharedMaterials[skinnedMeshMaterialIndex];
            PhysicsMaterialAssociation materialMatch = host.materialAssociationList.Find(m => m.material == skinMat);

            if(materialMatch != null) {
                meshCol.sharedMaterial = materialMatch.physicsMaterial;
            }

            if(parentBone.materialProperties) {
                if(!meshCol.sharedMaterial || parentBone.materialProperties.overrideOtherMats) {
                    meshCol.sharedMaterial = parentBone.materialProperties.physicsMaterial;
                }

                meshCol.convex = parentBone.materialProperties.convex;
            }

            if(parentBone.parentSkinfo.materialProperties) {
                if(!meshCol.sharedMaterial || parentBone.parentSkinfo.materialProperties.overrideOtherMats) {
                    meshCol.sharedMaterial = parentBone.parentSkinfo.materialProperties.physicsMaterial;
                }

                meshCol.convex = parentBone.parentSkinfo.materialProperties.convex;
            }

            if(parentBone.materialProperties && parentBone.materialProperties.overrideOtherMats) {
                meshCol.sharedMaterial = parentBone.materialProperties.physicsMaterial;
            }

            if(!meshCol.sharedMaterial) {
                meshCol.sharedMaterial = host.physicsMaterial;
            }
        }

        public void SetTris(IEnumerable<int> inTriList) {
            tris = inTriList.ToArray();
            distinctVerts = tris.Distinct().ToArray();

            Dictionary<int, int> triMap = new Dictionary<int, int>(distinctVerts.Length);
            for(int i = 0; i < distinctVerts.Length; i++) {
                triMap.Add(distinctVerts[i], i);
            }

            for(int i = 0; i < tris.Length; i++) {
                tris[i] = triMap[tris[i]];
            }
        }


        internal void CopyMeshUVs(List<Vector2> srcUVs, int uvIdx) {
            if(srcUVs.Count == 0) {
                return;
            }
            var tUVs = new List<Vector2>(distinctVerts.Length);
            for(int i = 0; i < distinctVerts.Length; i++) tUVs.Add(srcUVs[distinctVerts[i]]);
            collMesh.SetUVs(uvIdx, tUVs);
        }

        internal void CopyMeshNormals(Vector3[] srcNorms) {
            Vector3[] tNorms = new Vector3[distinctVerts.Length];
            for(int i = 0; i < distinctVerts.Length; i++) tNorms[i] = srcNorms[distinctVerts[i]];
            collMesh.normals = tNorms;
        }


        public bool PastThreshold(Vector3[] newVerts) {
            float totalDelta = 0;
            for(int i = 0; i < distinctVerts.Length; i++) {
                totalDelta += (newVerts[i] - oldVerts[i]).sqrMagnitude;
            }
            return totalDelta >= host.updateThreshold;
        }

        /// <summary>
        /// Based on the actual mesh vertices, transforms the required verts for this bone into the bones coordinate space.
        /// <para>Assigns transformed verts to the </para>
        /// </summary>
        /// <param name="actualVerts">Full list of vertices from the baked mesh.</param>
        internal void TransformVertices(List<Vector3> actualVerts) {
            if(parentBone.parentSkinfo.noBones && !host.zeroBoneMeshAlternateTransform) {
                for(int i = 0; i < distinctVerts.Length; i++) {
                    oldVerts[i] = transformedVerts[i];
                    transformedVerts[i] = actualVerts[distinctVerts[i]];
                }
            } else {
                Matrix4x4 matrix = parentBone.cachedMatrix * parentBone.parentSkinfo.meshRootMatrix;
                for(int i = 0; i < distinctVerts.Length; i++) {
                    oldVerts[i] = transformedVerts[i];
                    transformedVerts[i] = matrix.MultiplyPoint3x4(actualVerts[distinctVerts[i]]);
                }
            }
        }

        public void Update(List<Vector3> bakedVerts, bool force = false) {
            TransformVertices(bakedVerts);
            bool thresh = force || PastThreshold(transformedVerts);

            if(thresh) {
                collMesh.vertices = transformedVerts;

                meshCol.sharedMesh = collMesh;
            }
        }

#if UNITY_2019_3_OR_NEWER
        [NonSerialized] internal Task bakeTask;

        internal async void BakeMeshThreaded(bool convex) {
            collMesh.vertices = transformedVerts;
            isPastThreshold = false;

            bakeTask = Task.Run(() => {
                Physics.BakeMesh(collMeshInstID, convex);
            });
            await bakeTask;

            if(meshCol) {
                //annoying to have to check if it exists but since its threaded its possible its been destroyed in the meantime, prob from scene closing
                meshCol.sharedMesh = collMesh;
            }
        }
#endif

    }

}
