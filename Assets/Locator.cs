using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Locator : MonoBehaviour
{
    [Range(5,10)]
    public float rotationSpeed;

    public GameObject arrowDir;
    public GameObject secondArrow;

    public Player enemy;

    void Update()
    {
        Vector3Int leftSide = new Vector3Int((int)-transform.right.x, (int)-transform.right.y, (int)-transform.right.z) * 6;       
        Vector3Int rightSide = new Vector3Int((int)transform.right.x, (int)transform.right.y, (int)transform.right.z) * 6;
        Vector3Int backSide = new Vector3Int((int)-transform.forward.x, (int)-transform.forward.y, (int)-transform.forward.z) * 6;
        Vector3Int forwardSide = new Vector3Int((int)transform.forward.x, (int)transform.forward.y, (int)transform.forward.z) * 6;
        Vector3Int downwardSide = new Vector3Int((int)-transform.up.x, (int)-transform.up.y, (int)-transform.up.z) * 6;

        int leftSideBoundary = GetAxisBoundary(leftSide);
        int rightSideBoundary = GetAxisBoundary(rightSide);
        int backSideBoundary = GetAxisBoundary(backSide);
        int forwardSideBoundary = GetAxisBoundary(forwardSide);
        int downwardSideBoundary = GetAxisBoundary(downwardSide);        

        Quaternion[] rot = new Quaternion[4]
        {
            Quaternion.Euler(0,0,0),
            Quaternion.Euler(0,90,0),
            Quaternion.Euler(0,180,0),
            Quaternion.Euler(0,270,0)
        };

        if (!enemy.isRotating || !enemy.isMoving)
        {
            if ((enemy.newPos.x == leftSide.x && enemy.newPos.x == leftSideBoundary) ||
                (enemy.newPos.y == leftSide.y && enemy.newPos.y == leftSideBoundary) ||
                (enemy.newPos.z == leftSide.z && enemy.newPos.z == leftSideBoundary))
            {
                arrowDir.SetActive(true);
                secondArrow.SetActive(false);
                arrowDir.transform.localRotation = Quaternion.Lerp(arrowDir.transform.localRotation, rot[3], rotationSpeed * Time.deltaTime);
            }

            else if ((enemy.newPos.x == rightSide.x && enemy.newPos.x == rightSideBoundary) ||
                (enemy.newPos.y == rightSide.y && enemy.newPos.y == rightSideBoundary) ||
                (enemy.newPos.z == rightSide.z && enemy.newPos.z == rightSideBoundary))
            {
                arrowDir.SetActive(true);
                secondArrow.SetActive(false);
                arrowDir.transform.localRotation = Quaternion.Lerp(arrowDir.transform.localRotation, rot[1], rotationSpeed * Time.deltaTime);
            }

            else if ((enemy.newPos.x == backSide.x && enemy.newPos.x == backSideBoundary) ||
                (enemy.newPos.y == backSide.y && enemy.newPos.y == backSideBoundary) ||
                (enemy.newPos.z == backSide.z && enemy.newPos.z == backSideBoundary))
            {
                arrowDir.SetActive(true);
                secondArrow.SetActive(false);
                arrowDir.transform.localRotation = Quaternion.Lerp(arrowDir.transform.localRotation, rot[2], rotationSpeed * Time.deltaTime);
            }

            else if ((enemy.newPos.x == forwardSide.x && enemy.newPos.x == forwardSideBoundary) ||
                (enemy.newPos.y == forwardSide.y && enemy.newPos.y == forwardSideBoundary) ||
                (enemy.newPos.z == forwardSide.z && enemy.newPos.z == forwardSideBoundary))
            {
                arrowDir.SetActive(true);
                secondArrow.SetActive(false);
                arrowDir.transform.localRotation = Quaternion.Lerp(arrowDir.transform.localRotation, rot[0], rotationSpeed * Time.deltaTime);
            }

            else
            {
                if ((enemy.newPos.x == downwardSide.x && enemy.newPos.x == downwardSideBoundary) ||
                (enemy.newPos.y == downwardSide.y && enemy.newPos.y == downwardSideBoundary) ||
                (enemy.newPos.z == downwardSide.z && enemy.newPos.z == downwardSideBoundary))
                {
                    secondArrow.SetActive(true);
                    arrowDir.SetActive(true);
                }

                Vector3Int upawardSide = new Vector3Int((int)transform.up.x, (int)transform.up.y, (int)transform.up.z) * 6;
                int upwardSideBoundary = GetAxisBoundary(upawardSide);

                if ((enemy.newPos.x == upawardSide.x && enemy.newPos.x == upwardSideBoundary) ||
                (enemy.newPos.y == upawardSide.y && enemy.newPos.y == upwardSideBoundary) ||
                (enemy.newPos.z == upawardSide.z && enemy.newPos.z == upwardSideBoundary))
                {
                    arrowDir.SetActive(false);
                    secondArrow.SetActive(false);
                }
            }
        }
    }

    int GetAxisBoundary(Vector3Int side)
    {
        int value = new int();

        if (side.x == 6 || side.y == 6 || side.z == 6)
            value = 6;
        if (side.x == -6 || side.y == -6 || side.z == -6)
            value = -6;

        return value;
    }
}
