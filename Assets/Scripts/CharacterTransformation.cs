using StarterAssets;
using System.Collections.Generic;
using UnityEngine;

public class CharacterTransformation : MonoBehaviour
{
    private ThirdPersonController thirdPersonController;
    private Animator animator;

    [SerializeField] Avatar[] avatars;
    [SerializeField] GameObject[] children;
    private GameObject currentChild;
    private CharacterType currentType;

    private float transformationCooldown = 5f;
    private float nextTransformationTimes = 0f;

    private readonly Dictionary<CharacterType, int> characterIndexMap = new Dictionary<CharacterType, int>
    {
        { CharacterType.Human, 0 },
        { CharacterType.Rat, 1 },
        { CharacterType.Chameleon, 2 },
        { CharacterType.Monkey, 3 },
        { CharacterType.Kangaroo, 4 }
    };

    private readonly Dictionary<CharacterType, CharacterProperties> characterProperties = new Dictionary<CharacterType, CharacterProperties>
    {
        { CharacterType.Human,      new CharacterProperties(2f, 5.325f, 1.2f, 1f) },
        { CharacterType.Rat,        new CharacterProperties(1.0f, 2.0f, 0.2f, 0.13f) },
        { CharacterType.Chameleon,  new CharacterProperties(1.5f, 2.5f, 1.2f, 0.5f) },
        { CharacterType.Monkey,     new CharacterProperties(3.0f, 5.0f, 2.0f, .65f) },
        { CharacterType.Kangaroo,   new CharacterProperties(2.5f, 4.5f, 3.0f, 1.5f) }
    };

    private void Start()
    {
        thirdPersonController = GetComponent<ThirdPersonController>();
        animator = GetComponent<Animator>();
        //Default is Human
        currentChild = children[0];
        currentChild.SetActive(true);
    }

    private void Update()
    {
        CheckCharacterSwitchInput();
    }

    public void SwitchCharacter(CharacterType newCharacterType)
    {
        if (Time.time > nextTransformationTimes)
        {
            if (characterProperties.TryGetValue(newCharacterType, out CharacterProperties properties) &&
                characterIndexMap.TryGetValue(newCharacterType, out int index))
            {
                thirdPersonController.SetCharacterProperties(properties);
                thirdPersonController._currentCharacterType = newCharacterType;

                animator.avatar = avatars[index];

                if (currentChild != null)
                    currentChild.SetActive(false);

                currentChild = children[index];
                currentChild.SetActive(true);
                nextTransformationTimes = Time.time + transformationCooldown;
                currentType = newCharacterType;
            }
        }
        else
        {
            Debug.Log($"You can transform {nextTransformationTimes -Time.time} seconds later!");
        }
    }

    private void CheckCharacterSwitchInput()
    {
        // Map keys 0-4 to character types
        Dictionary<string, CharacterType> keyToCharacterTypeMap = new Dictionary<string, CharacterType>
        {
            { "0", CharacterType.Human },
            { "1", CharacterType.Rat },
            { "2", CharacterType.Chameleon },
            { "3", CharacterType.Monkey },
            { "4", CharacterType.Kangaroo }
        };

        if (Input.inputString.Length > 0 && keyToCharacterTypeMap.TryGetValue(Input.inputString, out CharacterType characterType))
        {
            if(characterType != currentType)
            {
                SwitchCharacter(characterType);
            }
        }
    }
}
