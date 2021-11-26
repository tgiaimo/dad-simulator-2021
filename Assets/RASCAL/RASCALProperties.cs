using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RASCALProperties : MonoBehaviour {

    [Tooltip("If checked, this material will override any other physics material settings.")]
    public bool overrideOtherMats = false;

    [Tooltip("The physics material to apply to the bone mesh colliders.")]
    public PhysicMaterial physicsMaterial;

    [Tooltip("When generating colliders, the bone weight for the vertex must be above this for the vertex to be added to the collider.")]
    public float boneWeightThreshold;

    [Tooltip("Overrides the RASCAL convex setting when applied to a bone or skinned mesh.")]
    public bool convex = false;

    [Tooltip("If present, will control which vertices of the mesh are included for collision. White is included, black is excluded. " +
        "NOTE: Only used when this component is on a SkinnedMeshRenderer.")]
    public Texture2D exclusionMap;

    [Range(0, 3)]
    public int exclusionMapChannel;


    Color32[] exclusionPixels;
    List<Vector2> tmpExclusionUvs = null;



    public bool InitExclusionMap(Mesh skinnedMesh) {
        if(!exclusionMap) {
            return false;
        }

        if(exclusionMap) {
            tmpExclusionUvs = new List<Vector2>();
            skinnedMesh.GetUVs(exclusionMapChannel, tmpExclusionUvs);
            if(tmpExclusionUvs.Count == 0) {
                Debug.LogError($"RASCAL: No UVs on mesh for exclusion map in UV channel {exclusionMapChannel}.");
                return false;
            }
        }

        try {
            exclusionPixels = exclusionMap.GetPixels32();
        } catch {
            Debug.LogError("RASCAL: Couldn't read the exclusion map. Make sure it is readable in texture import settings!");
            return false;
        }

        return true;
    }

    public bool GetExcludedVert(int vertexIdx) {
        Vector2 uv = tmpExclusionUvs[vertexIdx];
        uv.x %= 1f;
        uv.y %= 1f;
        uv.x *= exclusionMap.width - 1;
        uv.y *= exclusionMap.height - 1;

        return exclusionPixels[
            Mathf.RoundToInt(uv.y) * exclusionMap.width + Mathf.RoundToInt(uv.x)
        ].r <= 128;
    }

    public void FinishExlusions() {
        tmpExclusionUvs = null;
        exclusionPixels = null;
    }

}
