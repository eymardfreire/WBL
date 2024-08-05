using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshCollider))]
public class MeshDeformer : MonoBehaviour
{
    //public float impactRadius = 1f;
    public GameObject explosionEffectPrefab;

    public void DeformMesh(Vector3 point, float craterSizeInMeters, float impactRadius)
    {
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        Vector3[] vertices = mesh.vertices;

        Vector3 localImpactPoint = transform.InverseTransformPoint(point);
        float sqrImpactRadius = impactRadius * impactRadius;

    // Instantiate the explosion effect at the point of impact
    if (explosionEffectPrefab != null)
    {
        // The position for the particle effect should be in world space
        Vector3 worldImpactPoint = transform.TransformPoint(localImpactPoint);
        Instantiate(explosionEffectPrefab, worldImpactPoint, Quaternion.identity);
    }

    // Find vertices within impact radius
    for (int i = 0; i < vertices.Length; i++)
    {
        float sqrDistance = (vertices[i] - localImpactPoint).sqrMagnitude;
        if (sqrDistance < sqrImpactRadius)
        {
            float distance = Mathf.Sqrt(sqrDistance);
            float normalizedDistance = distance / impactRadius;
            // The closer the vertex is to the impact point, the more it will be moved down.
            float deformationAmount = (1 - normalizedDistance) * craterSizeInMeters;
            // Move the vertex down along the mesh's local Y-axis
            vertices[i] += transform.up * deformationAmount;
        }
    }

    // Update the mesh with the new vertex positions
    mesh.vertices = vertices;
    mesh.RecalculateBounds();
    mesh.RecalculateNormals();

    // Update the mesh collider to match the deformed mesh
    GetComponent<MeshCollider>().sharedMesh = mesh;
    }

    // Call this method when you want to deform the mesh
    public void ApplyDeformation(Vector3 impactPoint, float impactRadius)
    {
    // Since we need to pass a crater size as well, let's assume it's the same as the impact radius for now
    DeformMesh(impactPoint, impactRadius, impactRadius);
    }
}
