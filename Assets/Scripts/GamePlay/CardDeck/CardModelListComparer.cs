using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class CardModelListComparer : IComparer
{
	public int Compare(object l, object r)
	{
		GameTableResponse.CardModel card1 = (GameTableResponse.CardModel)l;
		GameTableResponse.CardModel card2 = (GameTableResponse.CardModel)r;
		int flag = 0;
		if (card1.rank > card2.rank)
		{
			flag = 1;
		}
		else if (card1.rank < card2.rank)
		{
			flag = -1;
		}
		return flag;
	}
}


class CardListComparer : IComparer
{
	public int Compare(object l, object r)
	{
		Card card1 = (Card)l;
		Card card2 = (Card)r;
		int flag = 0;
		if (card1.cardModel.rank > card2.cardModel.rank)
		{
			flag = 1;
		}
		else if (card1.cardModel.rank < card2.cardModel.rank)
		{
			flag = -1;
		}
		return flag;
	}
}