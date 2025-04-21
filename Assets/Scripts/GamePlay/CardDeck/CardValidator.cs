using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class CardValidator
{

    public static bool isValidSet(ArrayList paramArrCards)
    {

        ArrayList ArrCards = new ArrayList();
        foreach (object Obj in paramArrCards)
        {
            Card CardObj = (Card)Obj;
            ArrCards.Add(CardObj.cardModel.Clone());
        }

        bool flag = true;
        SortCardModelList(ArrCards);
        if (ArrCards.Count > 2)
        {
            bool isSameRank = haveSameRank(ArrCards);
            bool isUniqueSuit = haveUniqueSuit(ArrCards);
            if (!isSameRank || !isUniqueSuit)
            {
                flag = false;
            }
        }
        else
        {
            flag = false;
        }
        return flag;
    }

    public static bool isPureValidSequence(ArrayList paramArrCards)
    {
        ArrayList ArrCards = new ArrayList();
        foreach (object Obj in paramArrCards)
        {
            Card CardObj = (Card)Obj;
            ArrCards.Add(CardObj.cardModel.Clone());
        }
        bool flag = true;
        SortCardModelList(ArrCards);
        if (ArrCards.Count > 2)
        {
            bool isUniqueRank = haveCountiousRank(ArrCards);
            bool isSameSuit = haveSameSuit(ArrCards, ignoreJoker: false);
            if (!isUniqueRank || !isSameSuit)
            {
                flag = false;
            }
        }
        else
        {
            flag = false;
        }
        return flag;
    }


    public static bool isValidSequence(ArrayList paramArrCards)
    {
        ArrayList ArrCards = new ArrayList();
        foreach (object Obj in paramArrCards)
        {
            Card CardObj = (Card)Obj;
            ArrCards.Add(CardObj.cardModel.Clone());
        }
        bool flag = true;
        SortCardModelList(ArrCards);
        if (ArrCards.Count > 2)
        {
            bool isUniqueRank = haveCountiousRankWithJoker(ArrCards);
            bool isSameSuit = haveSameSuit(ArrCards, ignoreJoker: true);
            if (!isUniqueRank || !isSameSuit)
            {
                flag = false;
            }
        }
        else
        {
            flag = false;
        }
        return flag;
    }

    public static bool isValidDublee(ArrayList paramArrCards)
    {

        ArrayList ArrCards = new ArrayList();
        foreach (object Obj in paramArrCards)
        {
            Card CardObj = (Card)Obj;
            ArrCards.Add(CardObj.cardModel.Clone());
        }

        bool flag = true;
        SortCardModelList(ArrCards);
        bool isSameRank = false;
        bool isSameSuit = false;
        if (ArrCards.Count == 2)
        {
            isSameRank = haveSameRank_For_Tunnela_Or_Dublee(ArrCards);
            isSameSuit = haveSameSuit_For_Tunnela_Or_Dublee(ArrCards);
            if (!isSameRank || !isSameSuit)
            {
                flag = false;
            }
        }
        else
        {
            flag = false;
        }
        return flag;
    }

    public static bool isValidTunnela(ArrayList paramArrCards)
    {
        ArrayList ArrCards = new ArrayList();
        foreach (object Obj in paramArrCards)
        {
            Card CardObj = (Card)Obj;
            ArrCards.Add(CardObj.cardModel.Clone());
        }

        bool flag = true;
        SortCardModelList(ArrCards);
        bool isSameRank = false;
        bool isSameSuit = false;
        if (ArrCards.Count > 2)
        {
            isSameRank = haveSameRank_For_Tunnela_Or_Dublee(ArrCards);
            isSameSuit = haveSameSuit_For_Tunnela_Or_Dublee(ArrCards);
            if (!isSameRank || !isSameSuit)
            {
                flag = false;
            }
        }
        else
        {
            flag = false;
        }
        return flag;
    }

    public static bool haveUniqueSuit(ArrayList paramArrCards)
    {

        ArrayList ArrCards = new ArrayList();
        foreach (object Obj in paramArrCards)
        {
            GameTableResponse.CardModel CardObj = (GameTableResponse.CardModel)Obj;
            ArrCards.Add(CardObj.Clone());
        }

        bool flag = true;
        if (ArrCards.Count > 0)
        {
            ArrayList ArrTemp = Remove_All_Joker(ArrCards);
            foreach (GameTableResponse.CardModel SuitCard in ArrTemp)
            {
                if (SuitCard != null)
                {
                    foreach (GameTableResponse.CardModel TempCard in ArrTemp)
                    {
                        if (!TempCard.Equals(SuitCard)
                            && TempCard.suit.Equals(SuitCard.suit))
                        {
                            flag = false;
                            break;
                        }
                    }
                }
            }

        }
        else
        {
            flag = false;
        }
        return flag;
    }

    public static bool haveSameRank(ArrayList paramArrCards)
    {

        ArrayList ArrCards = new ArrayList();
        foreach (object Obj in paramArrCards)
        {
            GameTableResponse.CardModel CardObj = (GameTableResponse.CardModel)Obj;
            ArrCards.Add(CardObj.Clone());
        }

        bool flag = true;
        if (ArrCards.Count > 0)
        {
            ArrayList ArrTemp = Remove_All_Joker(ArrCards);
            GameTableResponse.CardModel RankCard = null;
            foreach (GameTableResponse.CardModel TempCard in ArrTemp)
            {
                RankCard = TempCard;
                break;
            }
            if (RankCard != null)
            {
                foreach (GameTableResponse.CardModel TempCard in ArrTemp)
                {
                    if (TempCard.rank != RankCard.rank)
                    {
                        flag = false;
                        break;
                    }
                }
            }

        }
        else
        {
            flag = false;
        }
        return flag;
    }

    public static bool haveCountiousRank(ArrayList paramArrCards)
    {

        ArrayList ArrCards = new ArrayList();
        foreach (object Obj in paramArrCards)
        {
            GameTableResponse.CardModel CardObj = (GameTableResponse.CardModel)Obj;
            ArrCards.Add(CardObj.Clone());
        }

        SortCardModelList(ArrCards);
        bool flag = true;
        if (ArrCards.Count > 0)
        {

            for (int i = 0; i < ArrCards.Count; i++)
            {
                GameTableResponse.CardModel card1 = (GameTableResponse.CardModel)ArrCards[i];
                GameTableResponse.CardModel card2 = null;
                int nextIndex = i + 1;
                if (nextIndex < ArrCards.Count)
                {
                    card2 = (GameTableResponse.CardModel)ArrCards[nextIndex];
                }
                if (card2 != null)
                {
                    int diff = card2.rank - card1.rank;
                    if (diff > 1)
                    {
                        flag = false;
                        break;
                    }
                    else if (diff == 0)
                    {
                        flag = false;
                        break;
                    }
                }
            }

            if (!flag)
            {
                flag = true;

                ArrayList TempCardModelList = new ArrayList();
                foreach (GameTableResponse.CardModel TempCard in ArrCards)
                {
                    TempCardModelList.Add(TempCard);
                }

                foreach (GameTableResponse.CardModel cardModel in TempCardModelList)
                {
                    if (cardModel.rank == 1)
                    {
                        cardModel.rank = 14;
                    }
                }
                SortCardModelList(TempCardModelList);

                for (int i = 0; i < TempCardModelList.Count; i++)
                {
                    GameTableResponse.CardModel card1 = (GameTableResponse.CardModel)TempCardModelList[i];
                    GameTableResponse.CardModel card2 = null;
                    int nextIndex = i + 1;
                    if (nextIndex < TempCardModelList.Count)
                    {
                        card2 = (GameTableResponse.CardModel)TempCardModelList[nextIndex];
                    }
                    if (card2 != null)
                    {
                        int diff = card2.rank - card1.rank;
                        if (diff < 0)
                        {
                            diff = -diff;
                        }
                        if (diff > 1)
                        {
                            flag = false;
                            break;
                        }
                        else if (diff == 0)
                        {
                            flag = false;
                            break;
                        }
                    }
                }
            }
        }
        else
        {
            flag = false;
        }
        return flag;
    }

    public static bool haveCountiousRankWithJoker(ArrayList paramArrCards)
    {

        ArrayList ArrCards = new ArrayList();
        foreach (object Obj in paramArrCards)
        {
            GameTableResponse.CardModel CardObj = (GameTableResponse.CardModel)Obj;
            ArrCards.Add(CardObj.Clone());
        }

        bool flag = true;
        if (ArrCards.Count > 0)
        {
            ArrayList ArrTempJoker = Get_All_Joker(ArrCards);
            ArrayList ArrTemp = Remove_All_Joker(ArrCards);
            int remainingJoker = ArrTempJoker.Count;

            for (int i = 0; i < ArrTemp.Count; i++)
            {
                GameTableResponse.CardModel card1 = (GameTableResponse.CardModel)ArrTemp[i];
                GameTableResponse.CardModel card2 = null;
                int nextIndex = i + 1;
                if (nextIndex < ArrTemp.Count)
                {
                    card2 = (GameTableResponse.CardModel)ArrTemp[nextIndex];
                }
                if (card2 != null)
                {
                    int diff = card2.rank - card1.rank;
                    if (diff > 1)
                    {
                        int jokerRequired = (diff - 1);
                        if (remainingJoker >= jokerRequired)
                        {
                            remainingJoker = remainingJoker - jokerRequired;
                        }
                        else
                        {
                            flag = false;
                            break;
                        }
                    }
                    else if (diff == 0)
                    {
                        flag = false;
                        break;
                    }
                }
            }

            if (!flag)
            {
                flag = true;
                remainingJoker = ArrTempJoker.Count;

                ArrayList TempCardModelList = new ArrayList();
                foreach (GameTableResponse.CardModel TempCard in ArrTemp)
                {
                    TempCardModelList.Add(TempCard);
                }

                foreach (GameTableResponse.CardModel cardModel in TempCardModelList)
                {
                    if (cardModel.rank == 1)
                    {
                        cardModel.rank = 14;
                    }
                }
                SortCardModelList(TempCardModelList);

                for (int i = 0; i < TempCardModelList.Count; i++)
                {
                    GameTableResponse.CardModel card1 = (GameTableResponse.CardModel)TempCardModelList[i];
                    GameTableResponse.CardModel card2 = null;
                    int nextIndex = i + 1;
                    if (nextIndex < TempCardModelList.Count)
                    {
                        card2 = (GameTableResponse.CardModel)TempCardModelList[nextIndex];
                    }
                    if (card2 != null)
                    {
                        int diff = card2.rank - card1.rank;
                        if (diff < 0)
                        {
                            diff = -diff;
                        }
                        if (diff > 1)
                        {
                            int jokerRequired = (diff - 1);
                            if (remainingJoker >= jokerRequired)
                            {
                                remainingJoker = remainingJoker - jokerRequired;
                            }
                            else
                            {
                                flag = false;
                                break;
                            }
                        }
                        else if (diff == 0)
                        {
                            flag = false;
                            break;
                        }
                    }
                }
            }
        }
        else
        {
            flag = false;
        }
        return flag;
    }

    public static bool haveSameSuit(ArrayList paramArrCards, Boolean ignoreJoker)
    {

        ArrayList ArrCards = new ArrayList();
        foreach (object Obj in paramArrCards)
        {
            GameTableResponse.CardModel CardObj = (GameTableResponse.CardModel)Obj;
            ArrCards.Add(CardObj.Clone());
        }

        bool flag = true;
        if (ArrCards.Count > 0)
        {
            ArrayList ArrTemp = new ArrayList();
            if (ignoreJoker == false)
            {
                ArrTemp = ArrCards;
            }
            else
            {
                ArrTemp = Remove_All_Joker(ArrCards);
            }

            GameTableResponse.CardModel SuitCard = null;
            foreach (GameTableResponse.CardModel TempCard in ArrTemp)
            {
                SuitCard = TempCard;
                break;
            }
            if (SuitCard != null)
            {
                foreach (GameTableResponse.CardModel TempCard in ArrTemp)
                {
                    if (!TempCard.suit.Equals(SuitCard.suit))
                    {
                        flag = false;
                        break;
                    }
                }
            }
        }
        else
        {
            flag = false;
        }
        return flag;
    }

    public static bool haveSameRank_For_Tunnela_Or_Dublee(ArrayList paramArrCards)
    {

        ArrayList ArrCards = new ArrayList();
        foreach (object Obj in paramArrCards)
        {
            GameTableResponse.CardModel CardObj = (GameTableResponse.CardModel)Obj;
            ArrCards.Add(CardObj.Clone());
        }

        bool flag = true;
        if (ArrCards.Count > 0)
        {
            GameTableResponse.CardModel RankCard = null;
            foreach (GameTableResponse.CardModel TempCard in ArrCards)
            {
                RankCard = TempCard;
                break;
            }
            if (RankCard != null)
            {
                foreach (GameTableResponse.CardModel TempCard in ArrCards)
                {
                    if (TempCard.rank != RankCard.rank)
                    {
                        flag = false;
                        break;
                    }
                }
            }
        }
        else
        {
            flag = false;
        }
        return flag;
    }

    public static bool haveSameSuit_For_Tunnela_Or_Dublee(ArrayList paramArrCards)
    {

        ArrayList ArrCards = new ArrayList();
        foreach (object Obj in paramArrCards)
        {
            GameTableResponse.CardModel CardObj = (GameTableResponse.CardModel)Obj;
            ArrCards.Add(CardObj.Clone());
        }

        bool flag = true;
        if (ArrCards.Count > 0)
        {
            GameTableResponse.CardModel SuitCard = null;
            foreach (GameTableResponse.CardModel TempCard in ArrCards)
            {
                SuitCard = TempCard;
                break;
            }
            if (SuitCard != null)
            {
                foreach (GameTableResponse.CardModel TempCard in ArrCards)
                {
                    if (TempCard.suit != SuitCard.suit)
                    {
                        flag = false;
                        break;
                    }
                }
            }
        }
        else
        {
            flag = false;
        }
        return flag;
    }

    public static ArrayList Remove_All_Joker(ArrayList paramArrCards)
    {

        ArrayList ArrCards = new ArrayList();
        foreach (object Obj in paramArrCards)
        {
            GameTableResponse.CardModel CardObj = (GameTableResponse.CardModel)Obj;
            ArrCards.Add(CardObj.Clone());
        }

        ArrayList TempList = new ArrayList(ArrCards);
        ArrayList TempJoker = new ArrayList();
        foreach (GameTableResponse.CardModel TempCard in TempList)
        {
            if (TempCard.IsJoker())
            {
                TempJoker.Add(TempCard);
            }
        }
        foreach (GameTableResponse.CardModel TempCard in TempJoker)
        {
            TempList.Remove(TempCard);
        }
        return TempList;
    }

    public static ArrayList Get_All_Joker(ArrayList paramArrCards)
    {

        ArrayList ArrCards = new ArrayList();
        foreach (object Obj in paramArrCards)
        {
            GameTableResponse.CardModel CardObj = (GameTableResponse.CardModel)Obj;
            ArrCards.Add(CardObj.Clone());
        }

        ArrayList TempJoker = new ArrayList();
        foreach (GameTableResponse.CardModel TempCard in ArrCards)
        {
            if (TempCard.IsJoker())
            {
                TempJoker.Add(TempCard);
            }
        }
        return TempJoker;
    }

    public static bool IsAllJoker(ArrayList ArrCards)
    {

        //Debug.Log ("IsAllJoker :- " + ArrCards.Count);
        bool flag = true;
        if (ArrCards.Count > 0)
        {
            foreach (Card TempCard in ArrCards)
            {
                if (!TempCard.cardModel.IsJoker())
                {
                    flag = false;
                    break;
                }
            }
        }
        //Debug.Log ("IsAllJoker flag :- " + flag);

        return flag;
    }

    /*public static void SortCardsList(ArrayList ArrCards){
		ArrCards.Sort (new CardListComparer());
	}*/

    public static void SortCardModelList(ArrayList ArrCards)
    {
        ArrCards.Sort(new CardModelListComparer());
    }

    public static (List<Card>, List<Card>, int) getCardsForSystem(List<Card> deckCards)
    {
        var groupID = 0;
        List<Card> cardModels = new List<Card>();

        #region Pure Life
        {
            deckCards.Sort();
            List<Card> group = GetPureLife(deckCards, 5); // Try to get a Pure Life of 4 cards first
            if (group.Count > 0)
            {
                Debug.Log("Pure Life (4 cards) group[" + group.Count + "], deckCards[" + deckCards.Count + "]");
                AddGroupToCardModels(group, ref cardModels, ref deckCards, ref groupID);
            }
        }
        {
            deckCards.Sort();
            List<Card> group = GetPureLife(deckCards, 4); // Try to get a Pure Life of 4 cards first
            if (group.Count > 0)
            {
                Debug.Log("Pure Life (4 cards) group[" + group.Count + "], deckCards[" + deckCards.Count + "]");
                AddGroupToCardModels(group, ref cardModels, ref deckCards, ref groupID);
            }
        }
        {
            deckCards.Sort();
            List<Card> group = GetPureLife(deckCards, 4); // Try to get a Pure Life of 4 cards first
            if (group.Count > 0)
            {
                Debug.Log("Pure Life (4 cards) group[" + group.Count + "], deckCards[" + deckCards.Count + "]");
                AddGroupToCardModels(group, ref cardModels, ref deckCards, ref groupID);
            }
        }

        {
            deckCards.Sort();
            List<Card> group = GetPureLife(deckCards, 3); // Try to get a Pure Life of 3 cards next
            if (group.Count > 0)
            {
                Debug.Log("Pure Life (3 cards) group[" + group.Count + "], deckCards[" + deckCards.Count + "]");
                AddGroupToCardModels(group, ref cardModels, ref deckCards, ref groupID);
            }
        }
        #endregion

        Debug.Log("(cardModels.Count after pure life" + cardModels.Count );

        if (cardModels.Count >= 3) 
        {
            #region Partial Life
            {
                deckCards.Sort();
                List<Card> group = GetPartialLife(deckCards, 5); // Try to get a Partial Life of 4 cards
                if (group.Count > 0)
                {
                    Debug.Log("Partial Life (4 cards) group[" + group.Count + "], deckCards[" + deckCards.Count + "]");
                    AddGroupToCardModels(group, ref cardModels, ref deckCards, ref groupID);
                }
            }
            {
                deckCards.Sort();
                List<Card> group = GetPartialLife(deckCards, 4); // Try to get a Partial Life of 4 cards
                if (group.Count > 0)
                {
                    Debug.Log("Partial Life (4 cards) group[" + group.Count + "], deckCards[" + deckCards.Count + "]");
                    AddGroupToCardModels(group, ref cardModels, ref deckCards, ref groupID);
                }
            }

            {
                deckCards.Sort();
                List<Card> group = GetPartialLife(deckCards, 4); // Try to get a Partial Life of 4 cards
                if (group.Count > 0)
                {
                    Debug.Log("Partial Life (4 cards) group[" + group.Count + "], deckCards[" + deckCards.Count + "]");
                    AddGroupToCardModels(group, ref cardModels, ref deckCards, ref groupID);
                }
            }

            {
                deckCards.Sort();
                List<Card> group = GetPartialLife(deckCards, 3); // Try to get a Partial Life of 3 cards
                if (group.Count > 0)
                {
                    Debug.Log("Partial Life (3 cards) group[" + group.Count + "], deckCards[" + deckCards.Count + "]");
                    AddGroupToCardModels(group, ref cardModels, ref deckCards, ref groupID);
                }
            }
            #endregion
        }
        Debug.Log("(cardModels.Count after partial life" + cardModels.Count );


        if (cardModels.Count >= 6) 
        {
            #region Sets
            {
                deckCards.Sort();
                List<Card> group = GetSet(deckCards, 4); // Try to get a Set of 4 cards
                if (group.Count > 0)
                {
                    Debug.Log("Set (4 cards) group[" + group.Count + "], deckCards[" + deckCards.Count + "]");
                    AddGroupToCardModels(group, ref cardModels, ref deckCards, ref groupID);
                }
            }

            {
                deckCards.Sort();
                List<Card> group = GetSet(deckCards, 4); // Try to get a Set of 4 cards
                if (group.Count > 0)
                {
                    Debug.Log("Set (4 cards) group[" + group.Count + "], deckCards[" + deckCards.Count + "]");
                    AddGroupToCardModels(group, ref cardModels, ref deckCards, ref groupID);
                }
            }

            {
                deckCards.Sort();
                List<Card> group = GetSet(deckCards, 3); // Try to get a Set of 3 cards
                if (group.Count > 0)
                {
                    Debug.Log("Set (3 cards) group[" + group.Count + "], deckCards[" + deckCards.Count + "]");
                    AddGroupToCardModels(group, ref cardModels, ref deckCards, ref groupID);
                }
            }

            {
                deckCards.Sort();
                List<Card> group = GetSet(deckCards, 3); // Try to get a Set of 3 cards
                if (group.Count > 0)
                {
                    Debug.Log("Set (3 cards) group[" + group.Count + "], deckCards[" + deckCards.Count + "]");
                    AddGroupToCardModels(group, ref cardModels, ref deckCards, ref groupID);
                }
            }
            #endregion
        }

        
        while (deckCards.Count > 0)
        {
            deckCards.Sort();
            List<Card> newCardModels = GetSuitCards(deckCards[0], deckCards);
            if (newCardModels.Count > 0)
            {
                foreach (Card m in newCardModels)
                {
                    m.cardModel.groupId = groupID;
                    cardModels.Add(m);
                    deckCards.RemoveAll(model => model.cardModel.id.Equals(m.cardModel.id));
                }
                groupID++;
            }
        }

        return (cardModels, deckCards, groupID);
    }
    // Helper method to add a group of cards to cardModels and remove them from deckCards
    private static void AddGroupToCardModels(List<Card> group, ref List<Card> cardModels, ref List<Card> deckCards, ref int groupID)
    {
        foreach (Card m in group)
        {
            m.cardModel.groupId = groupID;
            cardModels.Add(m);
            deckCards.RemoveAll(model => model.cardModel.id.Equals(m.cardModel.id));
        }
        groupID++;
    }
    static List<Card> GetSuitCards(Card currentCard,List<Card> deckCards)
    {
        List<Card> cardModels = new List<Card>();
        cardModels.Add(currentCard);
        foreach (var card in deckCards)
        {
            if (currentCard.cardModel.id != card.cardModel.id && currentCard.cardModel.suit == card.cardModel.suit)
            {
                cardModels.Add(card);
            }
        }
        return cardModels;
    }
    private static List<Card> GetPureLife(List<Card> deckCards, int cardCount)
    {
        List<Card> cardModels = new List<Card>();

        // Sort the deck by suit and rank to make it easier to find sequences
        deckCards.Sort((a, b) =>
        {
            int suitComparison = string.Compare(a.cardModel.suit, b.cardModel.suit, StringComparison.OrdinalIgnoreCase);
            if (suitComparison != 0) return suitComparison;
            return a.cardModel.rank.CompareTo(b.cardModel.rank);
        });

        for (int i = 0; i < deckCards.Count; i++)
        {
            Card cardToCompare = deckCards[i];
            if (cardToCompare.cardModel.IsJoker())
            {
                continue; // Skip Jokers
            }

            cardModels.Clear();
            cardModels.Add(cardToCompare); // Start the sequence with the current card

            // Try to build a sequence of the required length
            for (int j = i + 1; j < deckCards.Count && cardModels.Count < cardCount; j++)
            {
                Card nextCard = deckCards[j];

                // Skip Jokers
                if (nextCard.cardModel.IsJoker())
                {
                    continue;
                }

                // Check if the next card has the same suit and consecutive rank
                Card lastCardInSequence = cardModels[cardModels.Count - 1];
                if (lastCardInSequence.cardModel.suit == nextCard.cardModel.suit)
                {
                    if (lastCardInSequence.cardModel.rank + 1 == nextCard.cardModel.rank)
                    {
                        cardModels.Add(nextCard);
                    }
                    // Handle the special case for J, Q, K
                    else if (lastCardInSequence.cardModel.rank == 11 && nextCard.cardModel.rank == 12 || // J -> Q
                             lastCardInSequence.cardModel.rank == 12 && nextCard.cardModel.rank == 13 || // Q -> K
                             lastCardInSequence.cardModel.rank == 13 && nextCard.cardModel.rank == 1) // K -> A
                    {
                        cardModels.Add(nextCard);
                    }
                }
            }

            // If a valid sequence is found, return it
            if (cardModels.Count == cardCount)
            {
                return cardModels;
            }
        }

        return new List<Card>(); // Return an empty list if no valid sequence is found
    }
    private static List<Card> GetPartialLife(List<Card> deckCards, int cardCount)
    {
        // Sort the deck by suit and rank to make it easier to find sequences
        deckCards.Sort((a, b) =>
        {
            int suitComparison = string.Compare(a.cardModel.suit, b.cardModel.suit, StringComparison.OrdinalIgnoreCase);
            if (suitComparison != 0) return suitComparison;
            return a.cardModel.rank.CompareTo(b.cardModel.rank);
        });

        List<Card> bestSequence = new List<Card>();
        int bestSequenceValue = 0;

        // First, try to find a sequence without Jokers
        //bestSequence = FindPartialLifeWithoutJokers(deckCards, cardCount);
        //if (bestSequence.Count == cardCount)
        //{
        //    return bestSequence; // Return the sequence if found
        //}

        // If no sequence is found without Jokers, try to find a sequence with Jokers
        bestSequence = FindPartialLifeWithJokers(deckCards, cardCount);
        return bestSequence;
    }
    //private static List<Card> FindPartialLifeWithoutJokers(List<Card> deckCards, int cardCount)
    //{
    //    List<Card> bestSequence = new List<Card>();
    //    int bestSequenceValue = 0;

    //    // Iterate through the deck to find valid sequences without Jokers
    //    for (int i = 0; i <= deckCards.Count - cardCount; i++)
    //    {
    //        List<Card> sequence = new List<Card>();
    //        sequence.Add(deckCards[i]); // Start with the current card

    //        // Try to build a sequence of the required length
    //        for (int j = i + 1; j < deckCards.Count && sequence.Count < cardCount; j++)
    //        {
    //            Card nextCard = deckCards[j];

    //            // Skip Jokers in this phase
    //            if (nextCard.cardModel.IsJoker())
    //            {
    //                continue;
    //            }

    //            // Check if the next card can be added to the sequence
    //            if (CanAddToSequence(sequence, nextCard))
    //            {
    //                sequence.Add(nextCard);
    //            }
    //        }

    //        // If a valid sequence is found, evaluate its value
    //        if (sequence.Count == cardCount)
    //        {
    //            int sequenceValue = CalculateSequenceValue(sequence);
    //            if (sequenceValue > bestSequenceValue)
    //            {
    //                bestSequence = new List<Card>(sequence);
    //                bestSequenceValue = sequenceValue;
    //            }
    //        }
    //    }

    //    return bestSequence;
    //}
    private static List<Card> FindPartialLifeWithJokers(List<Card> deckCards, int cardCount)
    {
        List<Card> bestSequence = new List<Card>();
        int bestSequenceValue = 0;

        // Iterate through the deck to find valid sequences with Jokers
        for (int i = 0; i <= deckCards.Count - cardCount; i++)
        {
            List<Card> sequence = new List<Card>();
            sequence.Add(deckCards[i]); // Start with the current card

            // Try to build a sequence of the required length
            for (int j = i + 1; j < deckCards.Count && sequence.Count < cardCount; j++)
            {
                Card nextCard = deckCards[j];

                // Check if the next card can be added to the sequence (including Jokers)
                if (CanAddToSequence(sequence, nextCard))
                {
                    sequence.Add(nextCard);
                }
            }

            // If a valid sequence is found, evaluate its value
            if (sequence.Count == cardCount)
            {
                int sequenceValue = CalculateSequenceValue(sequence);
                if (sequenceValue > bestSequenceValue)
                {
                    bestSequence = new List<Card>(sequence);
                    bestSequenceValue = sequenceValue;
                }
            }
        }

        return bestSequence;
    }
    private static bool CanAddToSequence(List<Card> sequence, Card nextCard)
    {
        // If the next card is a Joker, it can always be added
        if (nextCard.cardModel.IsJoker())
        {
            return true;
        }

        // Find the last non-Joker card in the sequence
        Card lastCard = null;
        for (int i = sequence.Count - 1; i >= 0; i--)
        {
            if (!sequence[i].cardModel.IsJoker())
            {
                lastCard = sequence[i];
                break;
            }
        }

        // If no non-Joker card is found, any card can be added
        if (lastCard == null)
        {
            return true;
        }

        // Check if the next card has the same suit and consecutive rank
        if (lastCard.cardModel.suit.Equals(nextCard.cardModel.suit, StringComparison.OrdinalIgnoreCase) &&
            (lastCard.cardModel.rank + 1 == nextCard.cardModel.rank))
        {
            return true;
        }

        return false;
    }
    private static int CalculateSequenceValue(List<Card> sequence)
    {
        int totalValue = 0;
        foreach (Card card in sequence)
        {
            totalValue += GetCardScore(card); // Use the GetCardScore method from earlier
        }
        return totalValue;
    }
    private static int GetCardScore(Card card)
    {
        if (card.cardModel.IsJoker())
        {
            return 0; // Fixed score for Jokers
        }
        else
        {
            return card.cardModel.rank; // Score based on rank
        }
    }

    private static List<Card> GetSet(List<Card> deckCards, int cardCount)
    {
        for (int i = 0; i < deckCards.Count; i++)
        {
            Card cardToCompare = deckCards[i];
            if (cardToCompare.cardModel.IsJoker())
            {
                continue;
            }

            List<Card> currentSet = new List<Card>();
            currentSet.Add(cardToCompare);

            for (int j = 0; j < deckCards.Count && currentSet.Count < cardCount; j++)
            {
                if (i == j) continue; // Skip comparing the card with itself

                Card currentCard = deckCards[j];

                if (currentSet.Count > 0)
                {
                    Card lastCardInSet = currentSet[currentSet.Count - 1];

                    bool isUniqueSuit = true;
                    foreach (Card existingCard in currentSet)
                    {
                        if (existingCard.cardModel.suit.Equals(currentCard.cardModel.suit))
                        {
                            isUniqueSuit = false;
                            break;
                        }
                    }

                    if (isUniqueSuit && lastCardInSet.cardModel.rank == currentCard.cardModel.rank)
                    {
                        currentSet.Add(currentCard);
                    }
                    else if (currentCard.cardModel.cutJoker || currentCard.cardModel.suit.Equals(CardSuit.JOKER))
                    {
                        currentSet.Add(currentCard);
                    }
                }
            }

            if (currentSet.Count == cardCount)
            {
                return currentSet;
            }
        }
        return new List<Card>(); // Return empty list if no set is found
    }

}