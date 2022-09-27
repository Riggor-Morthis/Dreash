using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetStatique : MonoBehaviour
{

    [SerializeField]
    GameObject EnsembleChemin;
    [SerializeField]
    GameObject ImageTargetT;

    // Start is called before the first frame update
    void Start()
    {

        /* ArrayList Al=canvasObj.GetComponent<GestionnaireGraph>().getGraph();
         NodeM C;*/

        //GameObject[] targetStatique;


    }

    public void launch()
    {
        MeshFilter[] meshFilters = EnsembleChemin.GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];

        int i = 0;
        while (i < meshFilters.Length)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.worldToLocalMatrix;
            //combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            //Matrix4x4.TRS(meshFilters[i].transform.position,meshFilters[i].transform.localRotation, meshFilters[i].transform.localScale);
            meshFilters[i].gameObject.SetActive(false);

            i++;
        }
        ImageTargetT.transform.GetComponentInChildren<MeshFilter>().mesh = new Mesh();
        ImageTargetT.transform.GetComponentInChildren<MeshFilter>().mesh.CombineMeshes(combine);
        ImageTargetT.transform.gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
