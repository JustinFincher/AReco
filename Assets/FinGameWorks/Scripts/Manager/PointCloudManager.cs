using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CI.TaskParallel;
using HullDelaunayVoronoi.Delaunay;
using HullDelaunayVoronoi.Primitives;
using UnityEngine;
using UnityEngine.XR.iOS;
using Vectrosity;

namespace FinGameWorks.Scripts.Manager
{
    public class PointCloudManager : Singleton<PointCloudManager>
    {
        private DelaunayTriangulation3 delaunay;


        public Vector3[] PointCloudData;
        public Material lineMaterial;
        public int lineWidth;

        private List<VectorLine> lines;

        public ParticleSystem pointCloudParticlePrefab;
        public int maxPointsToShow;
        public float particleSize = 1.0f;

        private ParticleSystem currentPS;
        private ParticleSystem.Particle[] particles;

        public float maxPointWireFrameLength = 5;

        private bool frameUpdated;
        private bool delaunaySourceUpdated;

        public float PointCloudDeLauncyInterval = 0.2f;

        public bool use3DLine = false;

        private void Start()
        {
            if (!use3DLine)
            {
//                VectorLine.canvas.sortingOrder = -1;
            }

            UnityARSessionNativeInterface.ARFrameUpdatedEvent += ARFrameUpdated;
            currentPS = Instantiate(pointCloudParticlePrefab);
            frameUpdated = false;
            delaunaySourceUpdated = false;
            InvokeRepeating("GenerateDelaunay", PointCloudDeLauncyInterval, PointCloudDeLauncyInterval);
        }

        public void ARFrameUpdated(UnityARCamera camera)
        {
            PointCloudData = camera.pointCloudData;
            frameUpdated = true;
            delaunaySourceUpdated = true;
        }

        IEnumerator GenerateDelaunayEnumerator()
        {
            if (PointCloudData != null && PointCloudData.Length > 0 && maxPointsToShow > 0)
            {
                Camera mainCam = Camera.main;
                ;

                int numPoint = Mathf.Min(PointCloudData.Length, maxPointsToShow);
                Vertex3[] vertices = new Vertex3[numPoint];
                int renderableIndex = 0;
                for (int i = 0; i < PointCloudData.Length; i++)
                {
                    if (renderableIndex >= numPoint)
                    {
                        break;
                    }

                    Vector3 viewPos = mainCam.WorldToViewportPoint(PointCloudData[i]);
                    if (viewPos.x >= -0.1 && viewPos.x <= 1.1 && viewPos.y >= 0 && viewPos.y <= 1.1 &&
                        viewPos.z > -0.1)
                    {
                        vertices[renderableIndex] = new Vertex3(PointCloudData[i].x, PointCloudData[i].y,
                            PointCloudData[i].z);
                        renderableIndex++;
                    }
                }

                delaunay = new DelaunayTriangulation3();
                delaunay.Generate(vertices);

                
                if (lines != null)
                {
                    VectorLine.Destroy(lines);
                }
                foreach (var vectorObject3D in Component.FindObjectsOfType<VectorObject3D>())
                {
                    vectorObject3D.Destroy();
                }

                lines = new List<VectorLine>();
                if (delaunay != null)
                {
                    Debug.Log("Generating Delaunay Wireframe");
                    foreach (DelaunayCell<Vertex3> cell in delaunay.Cells)
                    {
                        Vector3 a1 = new Vector3(cell.Simplex.Vertices[0].X, cell.Simplex.Vertices[0].Y,
                            cell.Simplex.Vertices[0].Z);
                        Vector3 a2 = new Vector3(cell.Simplex.Vertices[1].X, cell.Simplex.Vertices[1].Y,
                            cell.Simplex.Vertices[1].Z);
                        float aLength = (a1 - a2).magnitude;
                        if (aLength < maxPointWireFrameLength)
                        {
                            VectorLine a;
                            if (use3DLine)
                            {
                                a = VectorLine.SetLine3D(new Color(1, 1, 1, 0.5f),
                                    a1, a2);
                            }
                            else
                            {
                                a = VectorLine.SetLine(new Color(1, 1, 1, 0.5f),
                                    Camera.main.WorldToScreenPoint(a1), Camera.main.WorldToScreenPoint(a2));
                            }

                            a.SetWidth(lineWidth);
                            a.material = lineMaterial;
                            lines.Add(a);
                        }

                        Vector3 b1 = new Vector3(cell.Simplex.Vertices[2].X, cell.Simplex.Vertices[2].Y,
                            cell.Simplex.Vertices[2].Z);
                        Vector3 b2 = new Vector3(cell.Simplex.Vertices[1].X, cell.Simplex.Vertices[1].Y,
                            cell.Simplex.Vertices[1].Z);
                        float bLength = (b1 - b2).magnitude;
                        if (bLength < maxPointWireFrameLength)
                        {
                            VectorLine b;
                            if (use3DLine)
                            {
                                b = VectorLine.SetLine3D(new Color(1, 1, 1, 0.5f),
                                    b1, b2);
                            }
                            else
                            {
                                b = VectorLine.SetLine(new Color(1, 1, 1, 0.5f),
                                    Camera.main.WorldToScreenPoint(b1), Camera.main.WorldToScreenPoint(b2));
                            }

                            b.SetWidth(lineWidth);
                            b.material = lineMaterial;
                            lines.Add(b);
                        }

                        Vector3 c1 = new Vector3(cell.Simplex.Vertices[0].X, cell.Simplex.Vertices[0].Y,
                            cell.Simplex.Vertices[0].Z);
                        Vector3 c2 = new Vector3(cell.Simplex.Vertices[2].X, cell.Simplex.Vertices[2].Y,
                            cell.Simplex.Vertices[2].Z);
                        float cLength = (c1 - c2).magnitude;
                        if (cLength < maxPointWireFrameLength)
                        {
                            VectorLine c;
                            if (use3DLine)
                            {
                                c = VectorLine.SetLine3D(new Color(1, 1, 1, 0.5f),
                                    c1, c2);
                            }
                            else
                            {
                                c = VectorLine.SetLine(new Color(1, 1, 1, 0.5f),
                                    Camera.main.WorldToScreenPoint(c1), Camera.main.WorldToScreenPoint(c2));
                            }

                            c.SetWidth(lineWidth);
                            c.material = lineMaterial;
                            lines.Add(c);
                        }
                    }
                }
            }

            yield return null;
        }

        public void GenerateDelaunay()
        {
            if (delaunaySourceUpdated == true)
            {
                Debug.Log("delaunaySourceUpdated = true, Process New Frame");
                delaunaySourceUpdated = false;

                StartCoroutine(GenerateDelaunayEnumerator());
            }
        }

        private void Update()
        {
            if (frameUpdated == true)
            {
                if (PointCloudData != null && PointCloudData.Length > 0 && maxPointsToShow > 0)
                {
                    int numPoint = Mathf.Min(PointCloudData.Length, maxPointsToShow);
                    ParticleSystem.Particle[] particles = new ParticleSystem.Particle[numPoint];

                    for (int i = 0; i < numPoint; i++)
                    {
                        particles[i].position = PointCloudData[i];
                        particles[i].startColor = new Color(1.0f, 1.0f, 1.0f);
                        particles[i].startSize = particleSize;
                    }

                    currentPS.SetParticles(particles, numPoint);
                }
                else
                {
                    ParticleSystem.Particle[] particles = new ParticleSystem.Particle[1];
                    particles[0].startSize = 0.0f;
                    currentPS.SetParticles(particles, 1);
                }

                frameUpdated = false;
            }
        }
    }
}