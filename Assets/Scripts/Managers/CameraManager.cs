using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private Player_Controller _player;
    [SerializeField] private float _distanceFromCameraToPlayer;
    private const string _playerTag = "Player";
    private const string _consumableTag = "Consumable";

    private ObjectFader _fader;
    private Vector3 _directionToPlayer;

    private void Start()
    {
        _directionToPlayer = _player.transform.position - transform.position;
        _distanceFromCameraToPlayer = Vector3.Distance(transform.position, _player.transform.position);
    }
    private void Update()
    {
        Ray ray = new (transform.position, _directionToPlayer);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, _distanceFromCameraToPlayer))
        {
            if (!hit.collider)
                return;

            Collider hitCollider = hit.collider;
            if (hitCollider.CompareTag(_playerTag)) // if catches player and nothing in between
            {
                if (_fader)
                    _fader.ShouldFade = false;

                _fader = null;
            }
            else if (hitCollider.CompareTag(_consumableTag))
            {
                _fader = hitCollider.GetComponent<ObjectFader>();

                if (_fader)
                    _fader.ShouldFade = true;
            }
        }
    }
}
