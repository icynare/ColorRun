/*********************************************************************
* Autor：zengruihong 
* Mail：zrh@talkmoney.cn
* CreateTime：2017/10/17 14:06:01
* Description：

*********************************************************************
* Copyright ©2017 Guangzhou Luwei Tech Co.,Ltd.ALL Rights Reserved
*********************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FuzzyString
{
	public enum FuzzyStringComparisonOptions
	{
		UseHammingDistance,

		UseJaccardDistance,

		UseJaroDistance,

		UseJaroWinklerDistance,

		UseLevenshteinDistance,

		UseLongestCommonSubsequence,

		UseLongestCommonSubstring,

		UseNormalizedLevenshteinDistance,

		UseOverlapCoefficient,

		UseRatcliffObershelpSimilarity,

		UseSorensenDiceDistance,

		UseTanimotoCoefficient,

		CaseSensitive
	}
}
