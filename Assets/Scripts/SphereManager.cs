using System.Collections;
using UnityEngine;

public class SphereManager : MonoBehaviour
{
    [SerializeField] private int sphereNum = 1000;

    [SerializeField] private Vector3 volumeMin = new(-100, -100, -100);
    [SerializeField] private Vector3 volumeMax = new(100, 100, 100);

    [SerializeField] private float sizeMin = 0.1f;
    [SerializeField] private float sizeMax = 2.0f;

    [SerializeField] private Color selectedColor = Color.red;
    [SerializeField] private Color defaultColor = Color.white;

    private readonly string sphereTag = "Sphere";

    private GameObject selectedSphere;

    [SerializeField] private RayHitSystem rayHitSystem;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(GenerateSphere());
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            SphereOnClick();
        }
    }

    private void CreateSphere()
    {
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.transform.position = new(
               Random.Range(volumeMin.x, volumeMax.x),
               Random.Range(volumeMin.y, volumeMax.y),
               Random.Range(volumeMin.z, volumeMax.z)
           );
        float size = Random.Range(sizeMin, sizeMax);
        sphere.transform.localScale = new Vector3(size, size, size);

        Renderer sphereRenderer = sphere.GetComponent<Renderer>();
        sphereRenderer.material.color = defaultColor;
        sphere.tag = sphereTag;

        sphere.transform.parent = this.transform;
    }

    //Generate spheres. Use Coroutine for optimization
    IEnumerator GenerateSphere()
    {
        Debug.Log("Sphere generation in progress. Please wait.");
        for (int i = 0; i < sphereNum; i++)
        {
            CreateSphere();

            if (i % 10 == 0)
            {
                yield return null;
            }
        }
        StartRayhitSystem();
    }

    private void StartRayhitSystem()
    {
        Debug.Log("Initializing Rayhit System. Please wait");
        rayHitSystem.gameObject.SetActive(true);
    }

    private void SphereOnClick()
    {
        //Get mouse position
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            GameObject hitObject = hit.collider.gameObject;

            if (hitObject.CompareTag(sphereTag))
            {
                //If a sphere has already been selected reset color
                if (selectedSphere != null)
                {
                    Renderer selectedRenderer = selectedSphere.GetComponent<Renderer>();
                    selectedRenderer.material.color = defaultColor;
                }
                // Update the selected sphere
                selectedSphere = hitObject;
                Renderer hitRenderer = hitObject.GetComponent<Renderer>();
                hitRenderer.material.color = selectedColor;
            }
        }
    }
}
