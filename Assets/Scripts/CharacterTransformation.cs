using StarterAssets;
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
    [SerializeField] private float transformationCooldown = 5f;
    private float nextTransformationTime = 0f;

    private int currentIndex;
    [SerializeField] GameObject currentChild;

    private void Start()
    {
        thirdPersonController = GetComponent<ThirdPersonController>();
        animator = GetComponent<Animator>();
        //SetCharacter(0); // Default is Human
    }

    private void Update()
    {
        CheckCharacterSwitchInput();
    }

    public void SwitchCharacter(int index)
    {
        if (Time.time > nextTransformationTime && index >= 0 && index < characterConfigs.Length && index != currentIndex)
        {
            SetCharacter(index);
            nextTransformationTime = Time.time + transformationCooldown;
        }
        else
        {
            Debug.Log($"You can transform {nextTransformationTime - Time.time} seconds later!");
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
        if (Input.inputString.Length > 0 && int.TryParse(Input.inputString, out int index))
        {
            SwitchCharacter(index);
        }
    }
}