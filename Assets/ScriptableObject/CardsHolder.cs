using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Cards",fileName ="DeckOfCards")]
public class CardsHolder : ScriptableObject
{
    public List<Sprite> CardSprite;
}
