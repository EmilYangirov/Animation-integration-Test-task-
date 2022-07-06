using System.Collections;
using UnityEngine;

[RequireComponent(typeof(KillEnemy))]
public class CharacterController : MonoBehaviour
{
    [SerializeField] private float moveSpeed, rotationSpeed;
    [SerializeField] private Transform rotateBone;
    [SerializeField] private float maxAngle, minHeightLook, maxHeightLook;
    [SerializeField] private Vector3 rotationOffset;

    [HideInInspector] public bool movingToTarget, killingEnemy;

    private Vector3 lookDirection;
    private float lastYRotation;
    private Animator anim;
    private Camera camera;

    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        camera = Camera.main;
    }

    private void FixedUpdate()
    {
        if (movingToTarget || killingEnemy)
            return;

        Move();        
    }

    private void LateUpdate()
    {
        if (movingToTarget || killingEnemy)
            return;

        RotateByMouse();
    }

    private void Move()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        if (horizontal == 0 && vertical == 0)
        {
            anim.SetBool("run", false);
            return;
        }            

        anim.SetBool("run", true);
        anim.SetFloat("Vertical", vertical);

        if (vertical >= 0)
            anim.SetFloat("Horizontal", horizontal);
        else
            anim.SetFloat("Horizontal", 0);

        var direction = transform.forward * vertical + transform.right * horizontal;
        transform.Translate(direction * moveSpeed * Time.deltaTime, Space.World);        
    }

    public IEnumerator MoveToTarget(Vector3 targetPosition, Quaternion rot)
    {
        movingToTarget = true;

        while (Vector3.Distance(transform.position, targetPosition) > 0.5)
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, rot, rotationSpeed * Time.deltaTime);

            anim.SetBool("run", true);
            anim.SetFloat("Vertical", 1);

            yield return null;
        }

        anim.SetBool("run", false);
        anim.SetFloat("Vertical", 0);
        transform.position = targetPosition;
        transform.rotation = rot;
        movingToTarget = false;
    }


    private void RotateByMouse()
    {
        bool rotate;
        RotateBody(out rotate);

        if (!rotate)
            return;

        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.transform.tag == "Player" || hit.transform.tag == "root")
                return;

            Vector3 direction = hit.point - transform.position;
            var newRot = Quaternion.LookRotation(new Vector3(direction.x, transform.position.y, direction.z));
            transform.rotation = Quaternion.Lerp(transform.rotation, newRot, rotationSpeed * Time.fixedDeltaTime);
        }

    }
    private void RotateBody(out bool rotateCharacter)
    {
        rotateCharacter = false;
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if(Physics.Raycast(ray, out hit))
        {
            if(hit.transform.tag != "Player" && hit.transform.tag != "root")
                lookDirection = (hit.point - rotateBone.position).normalized;

            var offset = Quaternion.Euler(0, -90, -78.709f);
            var newRotation = (Quaternion.LookRotation(lookDirection) * offset).eulerAngles + rotationOffset;

            //find correct rotation
            var rot = (transform.rotation * offset).eulerAngles + rotationOffset;

            float x = transform.eulerAngles.x + rotationOffset.x;
                        
            if (newRotation.y >= rot.y - maxAngle && newRotation.y <= rot.y + maxAngle)
                lastYRotation = newRotation.y;
            else
                rotateCharacter = true;

            float y = Mathf.Clamp(lastYRotation, rot.y - maxAngle, rot.y + maxAngle);
            float z = Mathf.Clamp(newRotation.z, rot.z + minHeightLook, rot.z + maxHeightLook);

            rotateBone.rotation = Quaternion.Euler(x, y, z);
        }
    }
}
