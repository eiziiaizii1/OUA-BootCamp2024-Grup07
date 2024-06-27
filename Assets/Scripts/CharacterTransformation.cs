using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterTransformation : MonoBehaviour
{
    private ThirdPersonController thirdPersonController;

    private void Start()
    {
        thirdPersonController = GetComponent<ThirdPersonController>();
    }

    private Dictionary<CharacterType, CharacterProperties> _characterProperties = new Dictionary<CharacterType, CharacterProperties>
    {
        { CharacterType.Human,      new CharacterProperties(2f, 5.325f, 1.2f, 1f) },
        { CharacterType.Rat,        new CharacterProperties(1.0f, 2.0f, 0.2f, 0.3f) },
        { CharacterType.Chameleon,  new CharacterProperties(1.5f, 2.5f, 1.2f, 0.5f) },
        { CharacterType.Monkey,     new CharacterProperties(3.0f, 5.0f, 2.0f, 0.8f) },
        { CharacterType.Kangaroo,   new CharacterProperties(2.5f, 4.5f, 3.0f, 1.5f) }
    };

    private void Update()
    {
        CheckCharacterSwitchInput();
    }

    public void SwitchCharacter(CharacterType newCharacterType)
    {
        if (_characterProperties.TryGetValue(newCharacterType, out CharacterProperties properties))
        {
            thirdPersonController.SetCharacterProperties(properties);
            thirdPersonController._currentCharacterType = newCharacterType;
        }
    }

    private void CheckCharacterSwitchInput()
    {
        switch (Input.inputString)
        {
            case "0":
                SwitchCharacter(CharacterType.Human);
                break;
            case "1":
                SwitchCharacter(CharacterType.Rat);
                break;
            case "2":
                SwitchCharacter(CharacterType.Chameleon);
                break;
            case "3":
                SwitchCharacter(CharacterType.Monkey);
                break;
            case "4":
                SwitchCharacter(CharacterType.Kangaroo);
                break;
        }
    }
}
