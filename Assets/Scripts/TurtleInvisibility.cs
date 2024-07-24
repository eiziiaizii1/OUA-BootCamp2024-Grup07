using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurtleSkill : MonoBehaviour
{
    public bool isInvisible = false;

    [SerializeField] Material visibleMaterial;
    [SerializeField] Material invisibleMaterial;
    private SkinnedMeshRenderer skinnedMeshRenderer;
    private Transform turtleObject;


    [SerializeField] float invisibilityDuration = 10f;
    [SerializeField] float invisibilityCooldown = 10f;
    private float nextInvisiblityTime = 0f;

    void Start()
    {
        turtleObject = gameObject.transform.GetChild(2);
        Transform turtleMesh = turtleObject.GetChild(0);
        skinnedMeshRenderer = turtleMesh.GetComponent<SkinnedMeshRenderer>();
    }

    void Update()
    {
        if (turtleObject.gameObject.activeInHierarchy && Time.time > nextInvisiblityTime && Input.GetKeyDown(KeyCode.C))
        {
            nextInvisiblityTime = invisibilityDuration + invisibilityCooldown + Time.time;
            BecomeInvisible();
        }
    }

    void BecomeInvisible()
    {
        isInvisible = true;
        skinnedMeshRenderer.material = invisibleMaterial;
        StartCoroutine(RevertVisibilityAfterDelay());
    }

    IEnumerator RevertVisibilityAfterDelay()
    {
        yield return new WaitForSeconds(invisibilityDuration);
        skinnedMeshRenderer.material = visibleMaterial;
        isInvisible = false;
    }
}
