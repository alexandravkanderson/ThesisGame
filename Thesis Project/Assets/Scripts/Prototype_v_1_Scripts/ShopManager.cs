using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class ShopManager : MonoBehaviour
{
    [SerializeField] private GameObject drawerView;
    
    // DRAWER TOGGLE
    [SerializeField] private Vector3 closedDrawerPosition = new Vector3(0, 80, 0);
    [SerializeField] private Vector3 expandedDrawerPosition = new Vector3(0, 280, 0);
    
    [SerializeField] private float duration = 2.0f;
    
    [SerializeField] private bool isDrawerExpanded = false;
    [SerializeField] private bool isDrawerMoving = false;
    
    // Start is called before the first frame update
    void Start()
    {
        drawerView = GameObject.Find("DrawerView");
        
        // Set the initial position of the drawer
        drawerView.GetComponent<RectTransform>().anchoredPosition = closedDrawerPosition;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void ToggleDrawer()
    {
        if (isDrawerExpanded && !isDrawerMoving)
        {
            StartCoroutine(ExecuteToggleDrawer(expandedDrawerPosition, closedDrawerPosition, duration));
        }
        else if (!isDrawerExpanded && !isDrawerMoving)
        {
            StartCoroutine(ExecuteToggleDrawer(closedDrawerPosition, expandedDrawerPosition, duration));
        }
    }

    private IEnumerator ExecuteToggleDrawer(Vector3 startPosition, Vector3 targetPosition, float duration)
    {
        isDrawerMoving = true;
        
        float elapsedTime = 0.0f;

        while (elapsedTime < duration)
        {
            drawerView.GetComponent<RectTransform>().anchoredPosition = Vector3.Lerp(startPosition, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        // Set the final position
        drawerView.GetComponent<RectTransform>().anchoredPosition = targetPosition;
        isDrawerMoving = false;
    }
}
