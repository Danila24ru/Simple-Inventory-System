using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Inventory
{
    /// <summary>
    ///  Component for dragging 3d object with mouse in one plane
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class DragableObject : MonoBehaviour
    {
        private Rigidbody rigidbody;

        private Vector3 offsetBetweenMouseAndObject;
        private float zPosition;

        private bool isInDrag = false;

        public bool IsInDrag { get { return isInDrag; } }

        private void Awake()
        {
            rigidbody = GetComponent<Rigidbody>();
        }

        private void OnMouseDown()
        {
            Vector3 screenPosition = Camera.main.WorldToScreenPoint(gameObject.transform.position);

            zPosition = screenPosition.z;

            // Store offset = gameobject world pos - mouse world pos
            offsetBetweenMouseAndObject = gameObject.transform.position - ConvertMousePositionToWorldPoint();

            rigidbody.isKinematic = true;
            isInDrag = true;
        }

        private void OnMouseDrag()
        {
            transform.position = ConvertMousePositionToWorldPoint() + offsetBetweenMouseAndObject;
        }

        private void OnMouseUp()
        {
            rigidbody.isKinematic = false;
            isInDrag = false;
        }

        private Vector3 ConvertMousePositionToWorldPoint()
        {
            // Pixel coordinates of mouse (x,y)
            Vector3 mousePoint = Input.mousePosition;

            // z coordinate of game object on screen
            mousePoint.z = zPosition;

            // Convert it to world points
            return Camera.main.ScreenToWorldPoint(mousePoint);
        }
    }
}

