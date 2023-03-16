using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKFootSolver : MonoBehaviour
{
    [SerializeField] private LayerMask terrainLayer;
    [SerializeField] private Transform body;
    [SerializeField] private IKFootSolver otherFoot; // reference the other foot
    [SerializeField] private float speed = 5, stepDistance = 0.3f, stepLength = 0.3f, stepHeight = 0.3f; // values for the foot. 
    [SerializeField] private Vector3 footPostOffset, footRotOffset;

    private float _footspacing, _lerp;
    private Vector3 _oldPos, _currentPos, _newPos;
    private Vector3 _oldNorm, _currentNorm, _newNorm;
    private bool _isFirstStep = true;


    // Start is called before the first frame update
    void Start()
    {


        _footspacing = transform.localPosition.x;
        _currentPos = _newPos = _oldPos = transform.position;
        _currentNorm = _newNorm = _oldNorm = transform.up;

        _lerp = 1;

    }

    // Update is called once per frame
    void Update()
    {
        transform.position = _currentPos + footPostOffset;
        transform.rotation = Quaternion.LookRotation(_currentNorm) * Quaternion.Euler(footRotOffset);

        Ray ray = new Ray(body.position + (body.right * _footspacing) + (Vector3.up * 2), Vector3.down);

        if( Physics.Raycast(ray, out RaycastHit hit, 10, terrainLayer.value))
        {
            if(_isFirstStep || (Vector3.Distance(_newPos, hit.point) > stepDistance && !otherFoot.IsMoving() && !IsMoving()))
            {
                _lerp = 0;
                int direction = body.InverseTransformPoint(hit.point).z > body.InverseTransformPoint(_newPos).z 
                    ? 1 
                    : -1;
                _newPos = hit.point + (body.forward * (direction * stepLength)); // + footPostOffset;
                _newPos = hit.normal; // footRotOffset;


            }

        }
        
        if(_lerp < 1)
        {
            Vector3 tempPos = Vector3.Lerp(_oldPos, _newPos, _lerp);
            tempPos.y += Mathf.Sin(_lerp * Mathf.PI) * stepHeight;

            _currentPos = tempPos;
            _currentNorm = Vector3.Lerp(_oldNorm, _newNorm, _lerp);
            _lerp += Time.deltaTime * speed;



        }

        else
        {
            _oldPos = _newPos;
            _oldNorm = _newNorm;
        }

    }

    public bool IsMoving()
    {
        return _lerp < 1;

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(_newPos, 0.1f);
    }

}
