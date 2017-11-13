using UnityEngine;
using System.Collections;
using System.Text;

public class GameFPS : MonoBehaviour {

	private Color color = Color.white;					// The color of the GUI, depending of the FPS ( R < 10, Y < 30, G >= 30 )
	private int frames = 0;							// Frames drawn over the interval
	private StringBuilder fpsInfo = new StringBuilder(0x10);

	public float frequency = 0.5F;							// The update frequency of the fps
	public bool allowShow = true;

	private float fps;
	private float gotIntervals;
	private float timeCount;
	private float timeleft;

	void Update()
	{
		this.frames++;
		this.timeleft -= Time.deltaTime;
		this.timeCount += Time.deltaTime;
		if (this.timeleft <= 0.0)
		{
			this.fps = ((float)this.frames) / this.timeCount;
			color = (fps >= 30) ? Color.green : ((fps > 10) ? Color.yellow : Color.red);
			this.timeCount = 0f;
			this.timeleft = this.frequency;
			this.frames = 0;
			this.gotIntervals++;
			this.fpsInfo.Length = 0;
			if (this.OnFrameRateRefresh != null)
			{
				this.fpsInfo.AppendFormat("FPS:{0}", this.fps.ToString("f0"));
				this.OnFrameRateRefresh(this.fpsInfo.ToString(), Color.green);
			}
			else
			{
				this.fpsInfo.AppendFormat("FPS:{0}", this.fps.ToString("f0"));
			}
		}
	}

	public FrameRateRefreshEvent OnFrameRateRefresh;
	public delegate void FrameRateRefreshEvent(string fps, Color color);
}
