using System;
using UnityEngine;

public class MachineInfo
{
    private static float _fillrate;
    public static ulong ServerTime;

    public static void AutoChooseQualityLevel()
    {
        int graphicsShaderLevel = SystemInfo.graphicsShaderLevel;
        int graphicsMemorySize = SystemInfo.graphicsMemorySize;
        int processorCount = SystemInfo.processorCount;
        int num4 = SystemInfo.graphicsMemorySize;
        float graphicsPixelFillrate = -1f;
        if (graphicsPixelFillrate < 0f)
        {
            if (graphicsShaderLevel < 10)
            {
                graphicsPixelFillrate = 1000f;
            }
            else if (graphicsShaderLevel < 20)
            {
                graphicsPixelFillrate = 1300f;
            }
            else if (graphicsShaderLevel < 30)
            {
                graphicsPixelFillrate = 2000f;
            }
            else
            {
                graphicsPixelFillrate = 3000f;
            }
            if (processorCount >= 6)
            {
                graphicsPixelFillrate *= 3f;
            }
            else if (processorCount >= 3)
            {
                graphicsPixelFillrate *= 2f;
            }
            if (graphicsMemorySize < 0x3e8)
            {
                graphicsPixelFillrate *= ((float) graphicsMemorySize) / 1000f;
            }
            if (num4 > 0x400)
            {
                graphicsPixelFillrate *= 2f;
            }
            else if (num4 > 0x200)
            {
                graphicsPixelFillrate *= ((float) num4) / 512f;
            }
        }
        if (!SystemInfo.supportsShadows)
        {
            graphicsPixelFillrate = 1000f;
        }
        _fillrate = graphicsPixelFillrate;
        int width = Screen.width;
        int height = Screen.height;
        float num8 = ((width * height) + 0x1d4c0) * 3E-05f;
        float[] numArray = new float[] { 5f, 30f, 80f, 130f, 200f, 320f };
        int index = 0;
        while ((index < 5) && (graphicsPixelFillrate > (num8 * numArray[index + 1])))
        {
            index++;
        }
        QualitySettings.SetQualityLevel(index);
    }

    public static string GetDebugInfo()
    {
        Debug.Log("处理器：" + SystemInfo.operatingSystem);
        string processorType = SystemInfo.processorType;
        int processorCount = SystemInfo.processorCount;
        Debug.Log("核数" + processorCount.ToString());
        int systemMemorySize = SystemInfo.systemMemorySize;
        Debug.Log("内存" + systemMemorySize.ToString());
        int graphicsMemorySize = SystemInfo.graphicsMemorySize;
        Debug.Log("显存" + graphicsMemorySize.ToString());
        string graphicsDeviceName = SystemInfo.graphicsDeviceName;
        int graphicsShaderLevel = SystemInfo.graphicsShaderLevel;
        Debug.Log("graphicsShaderLevel" + graphicsShaderLevel.ToString());
        int graphicsPixelFillrate = SystemInfo.graphicsPixelFillrate;
        Debug.Log("graphicsPixelFillrate" + graphicsPixelFillrate.ToString());
        bool supportsShadows = SystemInfo.supportsShadows;
        bool supportsImageEffects = SystemInfo.supportsImageEffects;
        bool supportsRenderTextures = SystemInfo.supportsRenderTextures;
        string str5 = string.Empty;
        str5 = (str5 + "处理器：" + processorType + " " + processorCount.ToString() + "核\r\n") + "内存：" + systemMemorySize.ToString() + "\r\n";
        object[] objArray1 = new object[] { str5, "显卡：", graphicsDeviceName, " 显存", graphicsMemorySize, " 像素填充率", graphicsPixelFillrate.ToString(), "\r\n" };
        return (((((((string.Concat(objArray1) + "Shader Level: " + graphicsShaderLevel.ToString() + "\r\n") + "支持阴影: " + supportsShadows.ToString() + "\r\n") + "支持相机效果: " + supportsImageEffects.ToString() + "\r\n") + "支持RenderTextures: " + supportsRenderTextures.ToString() + "\r\n") + "质量: " + QualitySettings.names[QualitySettings.GetQualityLevel()] + "\r\n") + "最终填充率:" + _fillrate.ToString() + "\r\n") + "各等级需求填充率：108, 648, 1728, 2808, 4320, 6912\r\n");
    }
}

