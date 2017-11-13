/*********************************************************************
* Autor：zengruihong 
* Mail：zrh@talkmoney.cn
* CreateTime：2017/10/17 14:06:01
* Description：

*********************************************************************
* Copyright ©2017 Guangzhou Luwei Tech Co.,Ltd.ALL Rights Reserved
*********************************************************************/

using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

[AddComponentMenu("UI/Effects/TextSpacing")]
public class TextSpacing : BaseMeshEffect
{
    enum Alignment
    {
        Left,
        Middle,
        Right,
    }

    private bool setTextSpacing = true;
	public float _textSpacing = 0f;
    public bool setBold = false;
    public int runCount = 0;

    private bool boldLeftSwitch = true;
    private float boldLeft = 0.1f;
    private bool boldRightSwitch = true;
	private float boldRight = 0.1f;
	private bool boldBottomSwitch = true;
    private float boldBottom = 0.1f;
    private bool boldTopSwitch = true;
	private float boldTop = 0.1f;
    
    public enum Direction { None,Horizontal,Vertical}
    [SerializeField]
    Direction direction = Direction.None;
    public Color32 startColor = Color.red;
    public Color32 endColor = Color.yellow;

    public override void ModifyMesh(VertexHelper vh)
	{
	    boldLeft = 0.1f;
	    boldRight = 0.1f;
	    boldBottom = 0.1f;
	    boldTop = 0.1f;
        if (!IsActive() || vh.currentVertCount == 0)
		{
			return;
		}

		Text text = GetComponent<Text>();
		if (text == null)
		{
			Debug.LogError("Missing Text component");
			return;
		}

		List<UIVertex> vertexs = new List<UIVertex>();
		vh.GetUIVertexStream(vertexs);


        if(setTextSpacing)
        {
            SetTextSpacing(vertexs,text);
        }
		if (setBold)
		{
			SetBold(vertexs, text);
		}

        if (direction != Direction.None)
        {
            SetColor(vertexs,text,direction);
        }

		vh.Clear();
		vh.AddUIVertexTriangleStream(vertexs);
        vertexs.Clear();
	}

    private void SetTextSpacing(List<UIVertex> vertexs,Text text)
    {

        //Debug.Log(text);
		Alignment alignment = Alignment.Right;
        switch(text.alignment)
        {
            case TextAnchor.LowerCenter:
            case TextAnchor.MiddleCenter:
            case TextAnchor.UpperCenter:
                alignment = Alignment.Middle;
                break;
            case TextAnchor.LowerLeft:
            case TextAnchor.MiddleLeft:
            case TextAnchor.UpperLeft:
                alignment = Alignment.Left;
                break;
		}
         
		int indexCount = vertexs.Count;

		UIVertex vt;

		//用这个变量暂存前一个字符被修改前的位置，不存的话判断行会出问题
		var lastPosX0Temp = vertexs[0].position.x;
        var difSum = 0f;
        var offsetList = new List<float>();
        var start0 = 0;
        var end0 = -1;
        offsetList.Add(difSum);
		for (var i = 6; i < indexCount; i += 6)
		{
            end0 = i-6;
			var lastPosX0 = vertexs[i - 6].position.x;
			var lastPosX1 = vertexs[i - 5].position.x;
			var curPosX0 = vertexs[i].position.x;
			//var curPosX1 = vertexs[i+1].position.x;


			if (curPosX0 <= lastPosX0Temp)//换行了，curPos属于新行
			{

                SetTextSpacingOneLine(offsetList, alignment, start0, vertexs);

                lastPosX0Temp = curPosX0;
                difSum = 0f;
				offsetList.Clear();
				offsetList.Add(difSum);
                start0 = i;
				continue;
			}
			else
			{
				lastPosX0Temp = curPosX0;
			}
			var dif = lastPosX1 + _textSpacing - curPosX0;
            difSum = dif + difSum;
            offsetList.Add(difSum);
			
		}
        SetTextSpacingOneLine(offsetList, alignment, start0, vertexs);
    }

    private void SetTextSpacingOneLine(List<float> offsetList,Alignment alignment,int start0,List<UIVertex> vertexs)
    {
        if (offsetList.Count>=1)
        {
            var end0Index = start0 + (offsetList.Count - 1) * 6;
			var end1Index = start0 + (offsetList.Count - 1) * 6+1;
			var end0X = vertexs[end0Index].position.x;
			var end1X = vertexs[end1Index].position.x;
            if( end1X-end0X<2f )
            {
                offsetList.RemoveAt(offsetList.Count-1);
            }
		}
        if (offsetList.Count <= 0)
            return;
		var addOffset = 0f;
		UIVertex vt;
		if (alignment == Alignment.Right)
			addOffset = -1 * offsetList[offsetList.Count - 1];
		else if (alignment == Alignment.Middle)
			addOffset = -0.5f * offsetList[offsetList.Count - 1];
        
		for (var j = 0; j < offsetList.Count; j++)
        {
            //Debug.LogFormat("{0}:{1}+addoffset:{2} = {3} ", j, offsetList[j], addOffset, offsetList[j] + addOffset);
			offsetList[j] = offsetList[j] + addOffset;

		}
		for (var j = 0; j < offsetList.Count; j++)
		{
			var difVec = new Vector3(offsetList[j], 0, 0);
			for (int z = start0 + j * 6; z < start0 + j * 6 + 6; z++)
			{
				vt = vertexs[z];
				vt.position += difVec;
				vertexs[z] = vt;

			}
		}
    }

    private void SetBold(List<UIVertex> vertexs,Text text)
    {
        if(boldLeftSwitch)
            ApplySameColorAlloc(vertexs,  0, vertexs.Count, -boldLeft, 0f);
        if(boldRightSwitch)
            ApplySameColorAlloc(vertexs, 0, vertexs.Count, boldRight, 0f);
        if(boldTopSwitch)
            ApplySameColorAlloc(vertexs, 0, vertexs.Count, 0f, boldTop);
        if(boldBottomSwitch)
            ApplySameColorAlloc(vertexs, 0, vertexs.Count, 0f, -boldBottom);
    }

	protected void ApplySameColorAlloc(List<UIVertex> verts,int start, int end, float x, float y)
	{
		UIVertex vt;
		var neededCapacity = verts.Count + end - start;
		if (verts.Capacity < neededCapacity)
			verts.Capacity = neededCapacity;
		for (int i = start; i < end; ++i)
		{
			vt = verts[i];
			
			Vector3 v = vt.position;
			v.x += x;
			v.y += y;

			vt.position = v;
            verts.Add(vt);
			//var newColor = color;
			//vt.color = newColor;
			//verts[i] = vt;
		}
	}

    private void SetColor(List<UIVertex> vertexs, Text text,Direction direction)
    {
        if (direction == Direction.None)
        {
            return;
        }
        else if (direction == Direction.Horizontal)
        {
            float bottomX = vertexs[0].position.x;
            float topX = vertexs[0].position.x;
            for (int i = 0; i < vertexs.Count; ++i)
            {
                float y = vertexs[i].position.x;
                if (y > topX)
                {
                    topX = y;
                }
                else if (y < bottomX)
                {
                    bottomX = y;
                }
            }

            float uiElementWidht = topX - bottomX;
            for (int i = 0; i < vertexs.Count; ++i)
            {
                UIVertex uiVertex = vertexs[i];
                uiVertex.color = Color32.Lerp(endColor, startColor, (uiVertex.position.x - bottomX) / uiElementWidht);
                vertexs[i] = uiVertex;
            }
        }
        else if (direction == Direction.Vertical)
        {
            float bottomY = vertexs[0].position.y;
            float topY = vertexs[0].position.y;
            for (int i = 0; i < vertexs.Count; ++i)
            {
                float y = vertexs[i].position.y;
                if (y > topY)
                {
                    topY = y;
                }
                else if (y < bottomY)
                {
                    bottomY = y;
                }
            }

            float uiElementHeight = topY - bottomY;
            for (int i = 0; i < vertexs.Count; ++i)
            {
                UIVertex uiVertex = vertexs[i];
                uiVertex.color = Color32.Lerp(endColor, startColor, (uiVertex.position.y - bottomY) / uiElementHeight);
                vertexs[i] = uiVertex;
            }
        }
    }
}