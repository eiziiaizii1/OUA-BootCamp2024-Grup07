using StarterAssets;
using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CharacterConfig
{
    public CharacterType characterType;
    public GameObject model;
    public Avatar avatar;
    public AnimatorOverrideController animatorController;
    public CharacterProperties properties;
}

public class CharacterTransformation : MonoBehaviour
{
    private ThirdPersonController thirdPersonController;
    private Animator animator;

    [SerializeField] private CharacterConfig[] characterConfigs; // All configs in one array
    [SerializeField] private float transformationCooldown = 3f;
    private float nextTransformationTime = 0f;

    private int currentIndex;
    [SerializeField] GameObject currentChild;

    [SerializeField] GameObject vfxPrefab;
    [SerializeField] GameObject vfxChild;
    private Transform playerTransform;
    private CharacterController characterController;
    private Quaternion initialVFXRotation;

    private void Start()
    {
        thirdPersonController = GetComponent<ThirdPersonController>();
        animator = GetComponent<Animator>();
        //SetCharacter(0); // Default is Human

        playerTransform = gameObject.transform;
        characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        CheckCharacterSwitchInput();

        if (vfxChild != null)
        {
            vfxChild.transform.rotation = initialVFXRotation;
        }
    }

    public void SwitchCharacter(int index)
    {
        if (Time.time > nextTransformationTime && index >= 0 && index < characterConfigs.Length && index != currentIndex)
        {
            SetCharacter(index);
            nextTransformationTime = Time.time + transformationCooldown;
            PlayVFX();
        }
        else
        {
            Debug.Log($"You can transform {Math.Clamp(0,nextTransformationTime - Time.time, transformationCooldown)} seconds later!");
        }
    }

    private void SetCharacter(int index)
    {
        if (currentChild != null)
            currentChild.SetActive(false);

        var config = characterConfigs[index];
        currentChild = config.model;
        currentChild.SetActive(true);

        thirdPersonController.SetCharacterProperties(config.properties);
        thirdPersonController._currentCharacterType = config.characterType;

        animator.avatar = config.avatar;
        animator.runtimeAnimatorController = config.animatorController;

        currentIndex = index;
    }

    private void CheckCharacterSwitchInput()
    {
        int totalCharacterNum = characterConfigs.Length;
        for (int i = 1; i < totalCharacterNum; i++)
        {
            if (Input.GetKeyDown(i.ToString()))
            {
                SwitchCharacter(i);
                return;
            }
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            SwitchCharacter(0);
        }
    }

    void PlayVFX()
    {
        if (vfxPrefab != null && playerTransform != null)
        {
            if (vfxChild != null)
            {
                Destroy(vfxChild);
            }
            Vector3 vfxPosition = new Vector3(
                playerTransform.position.x,
                playerTransform.position.y + (characterController.height / 2f),
                playerTransform.position.z);

            vfxChild = Instantiate(vfxPrefab, vfxPosition, playerTransform.rotation);
            vfxChild.transform.SetParent(playerTransform);

            initialVFXRotation = vfxChild.transform.rotation;
        }
    }

}