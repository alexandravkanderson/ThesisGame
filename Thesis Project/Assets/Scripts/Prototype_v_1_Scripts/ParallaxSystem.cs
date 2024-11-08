using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxSystem : MonoBehaviour
{
    // FOR BACKGROUND / FOREGROUND PARALLAX
    [System.Serializable] public class ParallaxLayer
    {
        public Transform parallaxLayer;  // Layer of background / foreground
        public float parallaxScale;      // Scale of parallax moving
        public float layerWidth;         // Width of layer (for infinite looping)
        public bool isLooping;           // Is the layer looping? (infinite scrolling)
    }

    public List<ParallaxLayer> layers;   // All the background / foreground layers
    public float smoothingSpeed = 1.0f;
    
    // CAMERA
    [SerializeField] private Transform camera;
    private Vector3 previousCameraPosition;
    
    // Start is called before the first frame update
    void Start()
    {
        // CAMERA WIRING
        if (Camera.main != null) camera = Camera.main.transform;
        previousCameraPosition = camera.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        foreach (ParallaxLayer layer in layers)
        {
            // PARALLAX EFFECT
            float parallax = (previousCameraPosition.x - camera.position.x) * layer.parallaxScale;
            float targetX = layer.parallaxLayer.position.x + parallax;
            Vector3 target = 
                new Vector3(targetX, layer.parallaxLayer.position.y, layer.parallaxLayer.position.z);
            
            // LOOPING / INFINITE SCROLLING
            if (layer.isLooping)
            {
                // ... going right
                if (camera.position.x > layer.parallaxLayer.position.x + layer.layerWidth)
                {
                    Vector3 newLayerPosition = 
                        new Vector3(
                            layer.parallaxLayer.position.x + 2 * layer.layerWidth, 
                            layer.parallaxLayer.position.y, 
                            layer.parallaxLayer.position.z);
                
                    layer.parallaxLayer.position = newLayerPosition;
                }
                // ... going left
                else if (camera.position.x < layer.parallaxLayer.position.x - layer.layerWidth)
                {
                    Vector3 newLayerPosition = 
                        new Vector3(
                            layer.parallaxLayer.position.x - 2 * layer.layerWidth, 
                            layer.parallaxLayer.position.y, 
                            layer.parallaxLayer.position.z);
                
                    layer.parallaxLayer.position = newLayerPosition;
                }
            }
        }
        
        // UPDATE CAMERA POSITION
        previousCameraPosition = camera.position;
    }
}
