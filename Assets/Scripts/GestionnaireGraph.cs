using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
public struct NodeM
{
    public int Haut;
    public int Bas;
    public int Droite;
    public int Gauche;

    public float x, y;
    public UnityEngine.Vector3 orientation;
    public int type;
    public bool visible;
}
*/

public class GestionnaireGraph : MonoBehaviour
{
    [SerializeField]
    GameObject chemin;

    //public Text timerText;
    //GameObject canvasObj;
    //Transform child;
    //ArrayList Graph;
    GameObject[] targetsVisible;
    List<(GameObject depart, GameObject arrive)> liens = new List<(GameObject depart, GameObject arrive)>();
    List<(GameObject voisin, float distance)> voisins = new List<(GameObject voisin, float distance)>();
    GameObject currentChemin;
    bool canUpdate = true;

    /*
    public ArrayList getGraph()
    {
        return Graph;
    }

    void Start()
    {
        Graph = new ArrayList();
        UnityEngine.Vector3 vect = UnityEngine.Vector3.forward;

        canvasObj = GameObject.Find("Canvas");
        child = canvasObj.transform.Find("Text"); //le nom de votre objet UI Text
        timerText = child.GetComponent<Text>();
        if (targetStatique == null)
            targetStatique = GameObject.FindGameObjectsWithTag("TargetStatique");
        if (targetStatique != null)
        {
            for (int i = 0; i < targetStatique.Length; i++)
            {
                NodeM newN;
                newN.Haut = -1;
                newN.Bas = -1;
                newN.Droite = -1;
                newN.Gauche = -1;
                newN.x = targetStatique[i].transform.position.x;
                newN.y = targetStatique[i].transform.position.y;
                newN.type = 1;
                newN.orientation = targetStatique[i].transform.rotation * vect;
                newN.visible = true;
                Graph.Add(newN);

            }

        }
    }

    void Update()
    {
        UnityEngine.Vector3 v12 = GameObject.Find("ImageTarget").transform.position;
        UnityEngine.Vector3 v22 = GameObject.Find("ImageTarget (1)").transform.position;
        float distance = Mathf.Sqrt(Mathf.Pow(v12.x - v22.x, 2) + Mathf.Pow(v12.y - v22.y, 2) + Mathf.Pow(v12.z - v22.z, 2));
        //timerText.text = ("Distance:" + distance);


        if (targetStatique != null)
        {
            targetStatique = GameObject.FindGameObjectsWithTag("TargetStatique");
            for (int i = 0; i < targetStatique.Length; i++)
            {
                NodeM actu = ((NodeM)Graph[i]);
                actu.orientation = targetStatique[i].transform.rotation * UnityEngine.Vector3.forward;
                Graph[i] = actu;
            }
            NodeM actu1 = ((NodeM)Graph[0]);

            UnityEngine.Vector3 v1 = targetStatique[0].transform.position;
            UnityEngine.Vector3 v2 = targetStatique[1].transform.position;
            distance = Mathf.Sqrt(Mathf.Pow(v1.x - v2.x, 2) + Mathf.Pow(v1.y - v2.y, 2) + Mathf.Pow(v1.z - v2.z, 2));

            //"Distance:" + distance + " " + targetStatique.Length + 
            timerText.text = (" Rotation : " + actu1.orientation.x +" " +actu1.orientation.y+" "+ actu1.orientation.z);
        }
    }
    */

    void Update()
    {
        liens.Clear();
        //On retire les chemins existant
        foreach (Transform child in transform) Destroy(child.gameObject);

        if (canUpdate)
        {
            //On commence par recuperer tous les cubes visibles
            targetsVisible = GameObject.FindGameObjectsWithTag("TargetVisible");

            //Avec au moins 2 cubes, on peut faire des chemins
            if (targetsVisible.Length >= 2)
            {
                //Pour chaque cube visible on va chercher a creer des liens
                foreach (GameObject target in targetsVisible)
                {
                    //On commence par se faire une liste des autres cubes, triee selon la distance qui nous separe
                    //On exclue les voisins avec qui on est deja lie
                    voisins.Clear();
                    foreach (GameObject otherTarget in targetsVisible) if (target != otherTarget) if (!liens.Contains((otherTarget, target)))
                            {
                                voisins.Add((otherTarget, Vector3.Distance(target.transform.position, otherTarget.transform.position)));
                            }
                    if (voisins.Count > 0)
                    {
                        voisins.Sort((a, b) => a.distance.CompareTo(b.distance));

                        //On se lie maintenant au voisin le plus proche
                        liens.Add((target, voisins[0].voisin));
                    }
                }

                //On peut creer un chemin a chaque lien
                foreach ((GameObject depart, GameObject arrive) lien in liens)
                {
                    currentChemin = GameObject.Instantiate(chemin, transform);
                    currentChemin.GetComponent<BezierCurvesScript>().init_bezier_curve(lien.depart, lien.arrive);
                }
            }
        }
    }

    public void Launch()
    {
        //On arrete de faire nos updates et nos calculs vu que le terrain est freeze
        canUpdate = false;

        if(targetsVisible.Length > 0)
        {
            //On trouve la salle la plus "au centre"
            Vector3 centre = Vector3.zero;
            foreach (GameObject target in targetsVisible) centre += target.transform.position;
            centre /= targetsVisible.Length;
            (GameObject salle, float distance) plusProche = (targetsVisible[0], Vector3.Distance(targetsVisible[0].transform.position, centre));
            for(int i = 1; i < targetsVisible.Length; i++) if(Vector3.Distance(targetsVisible[i].transform.position, centre) < plusProche.distance) plusProche = (targetsVisible[i], Vector3.Distance(targetsVisible[i].transform.position, centre));

            //Maintenant, on recupere son parent et on enleve les enfants qu'il possede
            GameObject carteParent = plusProche.salle.transform.parent.gameObject;
            foreach (Transform child in carteParent.transform) Destroy(child.gameObject);

            //On va maintenant lui confier chacune des salles
            foreach (GameObject target in targetsVisible) GameObject.Instantiate(target, target.transform.position, target.transform.rotation, carteParent.transform);

            //On va aussi lui confier chacun de nos enfants (les chemins)
            foreach (Transform child in transform) GameObject.Instantiate(child.gameObject, child.position, child.rotation, carteParent.transform);
        }
        //Il est grand temps de nous auto-detruire
        //Destroy(gameObject);
    }
}
