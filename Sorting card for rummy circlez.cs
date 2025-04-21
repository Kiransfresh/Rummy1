using System;

public class Class1
{
	public Class1()
	{
		public List<CardModel> GetPureLife(List<CardModel> deckCards, int cardCount)
{
    List<CardModel> cardModels = new List<CardModel>();
    for (int i = 0; i < deckCards.Count; i++)
    {
        CardModel cardToCompare = deckCards[i];
        if (cardToCompare.IsJoker())
        {
            continue;
        }
        cardModels.Clear();
        int j = 0;
        while (j < deckCards.Count)
        {
            if (cardModels.Count == cardCount)
            {
                return cardModels;
            }
            if (j == 0)
            {
                cardModels.Add(cardToCompare);
            }
            else
            {
                CardModel cardModel1 = cardModels[cardModels.Count - 1];
                CardModel cardModel2 = deckCards[j];
                if (cardModel1.Suit == cardModel2.Suit && (cardModel1.Rank + 1) == cardModel2.Rank)
                {
                    cardModels.Add(cardModel2);
                }
            }
            j++;
        }
    }
    return cardModels;
}

public List<CardModel> GetPartialLife(List<CardModel> deckCards, int cardCount)
{
    List<CardModel> cardModels = new List<CardModel>();
    for (int i = 0; i < deckCards.Count; i++)
    {
        CardModel cardModel1 = deckCards[i];
        cardModels.Clear();
        cardModels.Add(cardModel1);
        foreach (CardModel cardModel2 in deckCards)
        {
            if (!cardModel1.Equals(cardModel2))
            {
                List<CardModel> tempCardModels = new List<CardModel>(cardModels);
                bool cardFound = false;
                if (cardModel2.IsJoker())
                {
                    if ((cardModel2.IsCutJoker && IncludeCutJokerInAI) || (cardModel2.Suit == CardSuit.JOKER && IncludePrintedJokerInAI))
                    {
                        if (JokerIncluded < JokerCountInAI)
                        {
                            cardFound = true;
                            JokerIncluded++;
                        }
                    }
                }
                else
                {
                    cardFound = true;
                }
                if (cardFound)
                {
                    tempCardModels.Add(cardModel2);
                    Validator13Card validator13Card = new Validator13Card();
                    bool flag = validator13Card.IsValid_Partial_Sequence_WithoutCardLimit(tempCardModels);
                    if (flag)
                    {
                        cardModels.Add(cardModel2);
                    }
                }
            }
            if (cardModels.Count == cardCount)
            {
                return cardModels;
            }
        }
    }
    return cardModels;
}

public List<CardModel> GetSet(List<CardModel> deckCards, int cardCount)
{
    List<CardModel> cardModels = new List<CardModel>();
    for (int i = 0; i < deckCards.Count; i++)
    {
        CardModel cardToCompare = deckCards[i];
        if (cardToCompare.IsJoker())
        {
            continue;
        }
        cardModels.Clear();
        int j = 0;
        while (j < deckCards.Count)
        {
            if (cardModels.Count == cardCount)
            {
                return cardModels;
            }
            if (j == 0)
            {
                cardModels.Add(cardToCompare);
            }
            else
            {
                CardModel cardModel1 = cardModels[cardModels.Count - 1];
                CardModel cardModel2 = deckCards[j];
                bool cardFound = false;
                bool isUniqueSuit = true;
                foreach (CardModel cardModel in cardModels)
                {
                    if (cardModel.Suit == cardModel2.Suit)
                    {
                        isUniqueSuit = false;
                        break;
                    }
                }
                if (isUniqueSuit && cardModel1.Rank == cardModel2.Rank)
                {
                    cardFound = true;
                }
                else if ((cardModel2.IsCutJoker && IncludeCutJokerInAI) || (cardModel2.Suit == CardSuit.JOKER && IncludePrintedJokerInAI))
                {
                    if (JokerIncluded < JokerCountInAI)
                    {
                        cardFound = true;
                        JokerIncluded++;
                    }
                }
                if (cardFound)
                {
                    cardModels.Add(cardModel2);
                }
            }
            j++;
        }
    }
    return cardModels;
}

public List<CardModel> GetCardsForSystem(List<CardModel> deckCards)
{
    JokerIncluded = 0;
    JokerCountInAI = Utils.GetRandomValue(0, 2);
    IncludeCutJokerInAI = false;
    IncludePrintedJokerInAI = false;

    RandomizeJokerValuesInAI();
    List<CardModel> cardModels = new List<CardModel>();
    string meldOption = Utils.GetRandomValue(IConstants.MELD_GROUPS);
    List<CardModel> group1 = null;
    List<CardModel> group2 = null;
    List<CardModel> group3 = null;
    List<CardModel> group4 = null;

    if (meldOption.Equals(IConstants.MELD_GROUP.PURE3_PARTIAL3_SET4_SET3))
    {
        group1 = GetPureLife(deckCards, 3);
        deckCards.RemoveAll(group1);
        group2 = GetPartialLife(deckCards, 3);
        deckCards.RemoveAll(group2);
        group3 = GetSet(deckCards, 4);
        deckCards.RemoveAll(group3);
        group4 = GetSet(deckCards, 3);
        deckCards.RemoveAll(group4);
    }
    else if (meldOption.Equals(IConstants.MELD_GROUP.PURE3_PURE3_SET4_SET3))
    {
        group1 = GetPureLife(deckCards, 3);
        deckCards.RemoveAll(group1);
        group2 = GetPureLife(deckCards, 3);
        deckCards.RemoveAll(group2);
        group3 = GetSet(deckCards, 4);
        deckCards.RemoveAll(group3);
        group4 = GetSet(deckCards, 3);
        deckCards.RemoveAll(group4);
    }
    else if (meldOption.Equals(IConstants.MELD_GROUP.PURE3_PARTIAL4_SET3_SET3))
    {
        group1 = GetPureLife(deckCards, 3);
        deckCards.RemoveAll(group1);
        group2 = GetPartialLife(deckCards, 4);
        deckCards.RemoveAll(group2);
        group3 = GetSet(deckCards, 3);
        deckCards.RemoveAll(group3);
        group4 = GetSet(deckCards, 3);
        deckCards.RemoveAll(group4);
    }
    else if (meldOption.Equals(IConstants.MELD_GROUP.PURE4_PARTIAL3_SET3_SET3))
    {
        group1 = GetPureLife(deckCards, 4);
        deckCards.RemoveAll(group1);
        group2 = GetPartialLife(deckCards, 3);
        deckCards.RemoveAll(group2);
        group3 = GetSet(deckCards, 3);
        deckCards.RemoveAll(group3);
        group4 = GetSet(deckCards, 3);
        deckCards.RemoveAll(group4);
    }
    else if (meldOption.Equals(IConstants.MELD_GROUP.PURE4_PURE3_SET3_SET3))
    {
        group1 = GetPureLife(deckCards, 4);
        deckCards.RemoveAll(group1);
        group2 = GetPureLife(deckCards, 3);
        deckCards.RemoveAll(group2);
        group3 = GetSet(deckCards, 3);
        deckCards.RemoveAll(group3);
        group4 = GetSet(deckCards, 3);
        deckCards.RemoveAll(group4);
    }

    if (group1 != null)
    {
        foreach (CardModel obj in group1)
        {
            obj.GroupId = 0;
        }
        cardModels.AddRange(group1);
    }
    if (group2 != null)
    {
        foreach (CardModel obj in group2)
        {
            obj.GroupId = 1;
        }
        cardModels.AddRange(group2);
    }
    if (group3 != null)
    {
        foreach (CardModel obj in group3)
        {
            obj.GroupId = 2;
        }
        cardModels.AddRange(group3);
    }
    if (group4 != null)
    {
        foreach (CardModel obj in group4)
        {
            obj.GroupId = 3;
        }
        cardModels.AddRange(group4);
    }

    if (cardModels.Count < 13)
    {
        int cardsRequired = 13 - cardModels.Count;
        int i = 0;
        while (cardsRequired > 0 && deckCards.Count > 0)
        {
            CardModel cardModel = deckCards[i];
            int groupId = 3 - i;
            if (groupId <= 0)
            {
                groupId = 3;
            }
            cardModel.GroupId = groupId;
            deckCards.Remove(cardModel);
            cardModels.Add(cardModel);
            cardsRequired--;
            i++;
        }
    }
    return cardModels;
}
	}
}
