using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class BezierCurvesScript : MonoBehaviour
{
    private float resolution = 10;
    private float largeur_chemin = .1f; //La largeur du chemin (normalement indexe sur les unites unity)
    private Vector3 point_a, point_b; //Le point de depart et d'arrive du chemin
    private float pas; //Pour calculer les differents points le long de la courbe de bezier
    private Vector3[] points_controle; //Pour stocker nos points de controles
    private Vector3 a_to_b; //Le vecteur reliant les deux
    private Vector3[] bezier_points; //Les differents points qu'on calcule sur cette courbe

    public void init_bezier_curve(GameObject depart, GameObject arrive)
    {
        init_control_points(depart.transform.position, arrive.transform.position);
        fill_bezier_table();
        create_path();
    }

    /// <summary>
    /// Initialise les 4 points de controle
    /// </summary>
    private void init_control_points(Vector3 depart, Vector3 arrive)
    {
        // point_a = transform.GetChild(0).transform.position;
        // point_b = transform.GetChild(1).transform.position;
        point_a = depart;
        point_b = arrive;
        points_controle = new Vector3[4];

        //Le premier et dernier sont fixes par l'humain
        points_controle[0] = point_a;
        points_controle[3] = point_b;

        //Ici, le deuxieme et troisieme sont calcules
        //Peut etre que les fixer par l'humain donnerait de meilleurs resultats ?
        a_to_b = point_b - point_a;
        points_controle[1] = get_new_control_point(point_a, a_to_b);
        a_to_b = point_a - point_b;
        points_controle[2] = get_new_control_point(point_b, a_to_b);
    }

    /// <summary>
    /// Calcule un nouveau point de controle a partir des points de controle existants
    /// </summary>
    /// <param name="point">Le point de controle de depart</param>
    /// <param name="vector">Le vecteur liant les deux points de controles fixes</param>
    /// <returns>Un point de controle "virtuel", calcule a partir des points de controle fixes</returns>
    private Vector3 get_new_control_point(Vector3 point, Vector3 vector)
    {
        return point + (vector + new Vector3(-vector.z, 0f, vector.x)).normalized * (vector.magnitude * 0.5f);
    }

    /// <summary>
    /// Calcule tous les points le long de la courbe de bezier
    /// </summary>
    private void fill_bezier_table()
    {
        pas = (int)(Vector3.Distance(point_a, point_b) * resolution);
        bezier_points = new Vector3[(int)(pas + 1)];
        for (int i = 0; i < pas + 1; i++) bezier_points[i] = compute_bezier(points_controle[0], points_controle[1], points_controle[2], points_controle[3], i / pas);
    }

    /// <summary>
    /// Creer le mesh de notre chemin
    /// </summary>
    private void create_path()
    {
        GetComponent<MeshFilter>().mesh = calculate_mesh();
        GetComponent<MeshFilter>().sharedMesh = GetComponent<MeshFilter>().mesh;
        GetComponent<MeshRenderer>().material.mainTextureScale = new Vector2(largeur_chemin, a_to_b.magnitude);
    }

    /// <summary>
    /// Calcule le mesh a partir des points le long de la courbe
    /// </summary>
    /// <returns>Le mesh constituant notre chemin</returns>
    private Mesh calculate_mesh()
    {
        //On initialise des variables
        Vector3[] vertices = new Vector3[bezier_points.Length * 2];
        Vector2[] uv = new Vector2[bezier_points.Length * 2];
        int[] triangles = new int[2 * (bezier_points.Length - 1) * 3];
        int vertice_index = 0;
        int triangle_index = 0;
        float uv_index;
        Vector3 forward;
        Vector3 left;

        //On effecture ces calculs pour chaque point le long de la courbe
        for(int i = 0; i < bezier_points.Length; i++)
        {
            //Le vecteur direction au point i, moyenne des vecteurs (i-1, i) et (i, i+1)
            forward = Vector3.zero;
            if (i < bezier_points.Length - 1) forward += bezier_points[i + 1] - bezier_points[i];
            if (i > 0) forward += bezier_points[i] - bezier_points[i - 1];
            forward.Normalize();
            //On en deduit un autre vecteur pour calculer nos triangles
            left = new Vector3(-forward.z, 0, forward.x);

            //On ajoute les sommets a "gauche" et a "droite" du point i
            vertices[vertice_index] = bezier_points[i] + left * largeur_chemin * 0.5f;
            vertices[vertice_index+1] = bezier_points[i] - left * largeur_chemin * 0.5f;

            uv_index = i / (float)(bezier_points.Length - 1);
            uv[vertice_index] = new Vector2(0, uv_index);
            uv[vertice_index + 1] = new Vector2(1, uv_index);

            //On initialise les indexs des sommets de nos triangles
            if(i < bezier_points.Length - 1)
            {
                triangles[triangle_index] = vertice_index;
                triangles[triangle_index + 1] = vertice_index + 2;
                triangles[triangle_index + 2] = vertice_index + 1;

                triangles[triangle_index + 3] = vertice_index + 1;
                triangles[triangle_index + 4] = vertice_index + 2;
                triangles[triangle_index + 5] = vertice_index + 3;
            }

            //On incremente les compteurs
            vertice_index += 2;
            triangle_index += 6;
        }

        //On cree le nouveau mesh d'apres nos calculs
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uv;
        return mesh;
    }

    //Permet de calculer les coordonnes d'un point le long de la courbe de bezier
    private Vector3 compute_bezier(Vector3 point0, Vector3 point1, Vector3 point2, Vector3 point3, float t)
    {
        return Mathf.Pow(1 - t, 3) * point0 +
            3 * Mathf.Pow(1 - t, 2) * t * point1 +
            3 * (1 - t) * (t * t) * point2 +
            (t * t * t) * point3;
    }
}
