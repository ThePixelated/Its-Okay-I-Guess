using System.Drawing;
using UnityEngine;

public class MouseInteract : MonoBehaviour
{
    //[SerializeField] private Transform originalPos;
    //[SerializeField] private GameObject card;

    [SerializeField] private GameObject selectedObject;

    private Vector2 mousePos = new Vector2();
    private Vector3 point = new Vector3();
    private bool touchToggle = false;
    private string objectName;
    private Camera cam;

    void Start()
    {
        cam = Camera.main;
    }

    void OnGUI()
    {
        Event currentEvent = Event.current;

        // Get the mouse position from Event.
        // Note that the y position from Event is inverted.
        mousePos.x = currentEvent.mousePosition.x;
        mousePos.y = cam.pixelHeight - currentEvent.mousePosition.y;

        point = cam.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, Mathf.Abs(cam.transform.position.z)));

        GUILayout.BeginArea(new Rect(20, 20, 250, 120));
        GUILayout.Label("Screen pixels: " + cam.pixelWidth + ":" + cam.pixelHeight);
        GUILayout.Label("Mouse position: " + mousePos);
        GUILayout.Label("World position: " + point.ToString("F3"));
        GUILayout.Label("Mouse is pressed: " + touchToggle);
        GUILayout.Label("Object selected: " + objectName);
        GUILayout.EndArea();
    }

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            touchToggle = true;
            Collider2D targetObject = Physics2D.OverlapPoint(point);

            if (targetObject != null && targetObject.gameObject.tag == "Card")
            {
                objectName = targetObject.name;
                selectedObject.transform.position= new Vector3(point.x, 0.82f, -1.71f);

                var rigidObj = selectedObject.GetComponent<Rigidbody>();
                rigidObj.useGravity = false;
                rigidObj.constraints = RigidbodyConstraints.FreezeRotationZ;
            }

            else if (targetObject != null)
            {
                Debug.LogWarning("Object ga kedetect woi");
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            objectName = "";
            touchToggle = false;
            
            var rigidObj = selectedObject.GetComponent<Rigidbody>();
            rigidObj.useGravity = true;
            //rigidObj.constraints = RigidbodyConstraints.f;
            
            selectedObject.transform.position = new Vector3(point.x, 0.82f, selectedObject.transform.position.z);
        }
    }
}
