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
	public static partial class ComparisonMetrics
	{
		public static double JaroDistance(this string source, string target)
		{
			int m = source.Intersect(target).Count();

			if (m == 0) { return 0; }
			else
			{
				string sourceTargetIntersetAsString = "";
				string targetSourceIntersetAsString = "";
				IEnumerable<char> sourceIntersectTarget = source.Intersect(target);
				IEnumerable<char> targetIntersectSource = target.Intersect(source);
				foreach (char character in sourceIntersectTarget) { sourceTargetIntersetAsString += character; }
				foreach (char character in targetIntersectSource) { targetSourceIntersetAsString += character; }
				double t = sourceTargetIntersetAsString.LevenshteinDistance(targetSourceIntersetAsString) / 2;
				return ((m / source.Length) + (m / target.Length) + ((m - t) / m)) / 3;
			}
		}
	}
}
