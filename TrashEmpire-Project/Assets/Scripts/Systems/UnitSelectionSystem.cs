using System;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace TMG.TrashEmpire
{
    public class UnitSelectionSystem : MonoBehaviour
    {
        private float3 mouseStartPos;
        private float3 mouseEndPos;
        private Camera _mainCamera;
        private EntityManager _entityManager;
        private bool isDragging;
        
        private void Start()
        {
            _mainCamera = Camera.main;
            _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        }

        private void OnGUI()
        {
            if (isDragging)
            {
                var rect = GetScreenRect(mouseStartPos, Input.mousePosition);
                DrawScreenRect(rect, new Color(0.8f, 0.8f, 0.95f, 0.1f));
                DrawScreenRectBorder(rect,1, Color.blue);
            }
        }

        public static Rect GetScreenRect(Vector3 screenPosition1, Vector3 screenPosition2)
        {
            // Move origin from bottom left to top left
            screenPosition1.y = Screen.height - screenPosition1.y;
            screenPosition2.y = Screen.height - screenPosition2.y;
            // Calculate corners
            var topLeft = Vector3.Min(screenPosition1, screenPosition2);
            var bottomRight = Vector3.Max(screenPosition1, screenPosition2);
            // Create Rect
            return Rect.MinMaxRect(topLeft.x, topLeft.y, bottomRight.x, bottomRight.y);
        }
        static Texture2D _whiteTexture;
        public static Texture2D WhiteTexture
        {
            get
            {
                if (_whiteTexture == null)
                {
                    _whiteTexture = new Texture2D(1, 1);
                    _whiteTexture.SetPixel(0, 0, Color.white);
                    _whiteTexture.Apply();
                }

                return _whiteTexture;
            }
        }
        public static void DrawScreenRect(Rect rect, Color color)
        {
            GUI.color = color;
            GUI.DrawTexture(rect, WhiteTexture);
        }

        public static void DrawScreenRectBorder(Rect rect, float thickness, Color color)
        {
            //Top
            DrawScreenRect(new Rect(rect.xMin, rect.yMin, rect.width, thickness), color);
            // Left
            DrawScreenRect(new Rect(rect.xMin, rect.yMin, thickness, rect.height), color);
            // Right
            DrawScreenRect(new Rect(rect.xMax - thickness, rect.yMin, thickness, rect.height), color);
            // Bottom
            DrawScreenRect(new Rect(rect.xMin, rect.yMax - thickness, rect.width, thickness), color);
        }
        
        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                // Start clicking
                mouseStartPos = Input.mousePosition;
                isDragging = true;
            }

            if (Input.GetMouseButton(0))
            {
                // While dragging
                
            }

            if (Input.GetMouseButtonUp(0))
            {
                // Mouse no longer held
                mouseEndPos = Input.mousePosition;
                isDragging = false;
            }
        }
    }
}