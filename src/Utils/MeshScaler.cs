﻿using UnityEngine;

namespace StarSystems.Utils
{
    //Meshscaler
    public class MeshScaler : MonoBehaviour
    {
        public static void ScaleMesh(Mesh Mesh, float Scale)
        {
            //Create new vertices list
            Vector3[] vertices = new Vector3[Mesh.vertexCount];

            //for each vertice in the original mesh
            for (int i = 0; i < Mesh.vertexCount; i++)
            {
                //Scale original vertice and add to vertices list
                Vector3 vertice = Mesh.vertices[i];
                vertice *= Scale;
                vertices[i] = vertice;
            }

            //update the mesh
            Mesh.vertices = vertices;
            Mesh.RecalculateBounds();
            Mesh.RecalculateNormals();
        }
    }
}
