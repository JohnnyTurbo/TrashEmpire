using System;
using Unity.Mathematics;
using UnityEngine;

namespace TMG.TrashEmpire.AuthoringAndMono
{
    [RequireComponent(typeof(Camera))]
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private float _moveSpeed;
        [SerializeField] private float _scrollSpeed;
        [SerializeField] private float _minimumDistanceFromGround;
        [SerializeField] private float _maximumDistanceFromGround;
        [SerializeField] private Vector3 _camMinimums;
        [SerializeField] private Vector3 _camMaximums;

        private Vector3 _camDifference;

        private void Update()
        {
            var camMovement = new Vector3
            {
                x = Input.GetAxis("Horizontal") * _moveSpeed * Time.deltaTime,
                y = Input.GetAxis("Mouse ScrollWheel") * _scrollSpeed * -1f/** Time.deltaTime*/,
                z = Input.GetAxis("Vertical") * _moveSpeed * Time.deltaTime
            };
            if (camMovement != Vector3.zero)
            {
                transform.Translate(camMovement, Space.World);
                
                // todo: add layer mask
                if (Physics.Raycast(transform.position, Vector3.down, out var hit))
                {
                    if (hit.distance < _minimumDistanceFromGround)
                    {
                        var difference = _minimumDistanceFromGround - hit.distance;
                        transform.Translate(0f, difference, 0f, Space.World);
                    }
                    else if (hit.distance > _maximumDistanceFromGround)
                    {
                        var difference = hit.distance - _maximumDistanceFromGround;
                        transform.Translate(0f, -difference, 0f, Space.World);
                    }
                }

                var position = transform.position;
                position = new Vector3
                {
                    x = Mathf.Clamp(position.x, _camMinimums.x, _camMaximums.x),
                    y = Mathf.Clamp(position.y, _camMinimums.y, _camMaximums.y),
                    z = Mathf.Clamp(position.z, _camMinimums.z, _camMaximums.z)
                };
                transform.position = position;
            }
        }

        private void OnDrawGizmosSelected()
        {
            _camDifference = _camMaximums - _camMinimums;
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(new Vector3
            {
                x = _camDifference.x * 0.5f,
                y = _camDifference.y * 0.5f,
                z = _camDifference.z * 0.5f

            }, _camDifference);
        }
    }
}