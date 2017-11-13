/*********************************************************************
* Autor：zhuxiaolu 
* Mail：zhuxiaolu@talkmoney.cn
* CreateTime：2017/07/30 20:10:13
* Description：
运行时界面控制台
*********************************************************************
* Copyright ©2017 Guangzhou Luwei Tech Co.,Ltd.ALL Rights Reserved
*********************************************************************/

using System.Collections;
using System.Collections.Generic;
using Msg;
using UnityEngine;

public class ConsoleGUI : MonoBehaviour
{
	struct Log
	{
		public string message;
		public string stackTrace;
		public LogType type;
	}

	#region Inspector Settings

	public KeyCode toogleKey = KeyCode.BackQuote;
	public bool isShakeToOpen = true;
	public float shakeAcceleration = 20f;
	public bool isRestrictLogCount = false;
	public int maxLogs = 1000;

	#endregion

	readonly List<Log> _logs = new List<Log>();
	private Vector2 _scrollPosition;
    public bool isVisible;
	private bool _isCollapse;

	private static readonly Dictionary<LogType, Color> logTypeColors = new Dictionary<LogType, Color>
	{
		{LogType.Assert, Color.white},
		{LogType.Error, Color.red},
		{LogType.Exception, Color.red},
		{LogType.Log, Color.white},
		{LogType.Warning, Color.yellow}
	};

	private const string WINDOW_TITLE = "Console";
	private const int MARGIN = 20;
	static readonly GUIContent _clearLabel = new GUIContent("Clear", "Clear the contents of the console");
	static readonly GUIContent _collapseLabel = new GUIContent("Collapse", "Hide repeated messages");

	readonly Rect _titleBarRect = new Rect(0, 0, 10000, 20);
	Rect _windowRect = new Rect(MARGIN, MARGIN, Screen.width - (MARGIN * 2), Screen.height - (MARGIN * 2));

	void OnEnable()
	{
		Application.logMessageReceived += HandleLog;
	}

	void OnDisable()
	{
		Application.logMessageReceived -= HandleLog;
	}

	void HandleLog(string message, string stackTrace, LogType type)
	{
		_logs.Add(new Log()
		{
			message = message,
			stackTrace = stackTrace,
			type = type,
		});
		TrimExcessLogs();
	}

	//去除相同的log
	void TrimExcessLogs()
	{
		if (!isRestrictLogCount)
		{
			return;
		}

		int amountToRemove = Mathf.Max(_logs.Count - maxLogs, 0);

		if (amountToRemove == 0)
		{
			return;
		}

		_logs.RemoveRange(0, amountToRemove);
	}

	void Update()
	{
		//按下 ~ 键打开关闭控制台
		if (Input.GetKeyDown(toogleKey))
		{
			isVisible = !isVisible;
		}
        //手机摇晃时打开控制台
        if (isShakeToOpen && Input.acceleration.sqrMagnitude > shakeAcceleration)
        {
            isVisible = true;
        }
		
	}

	void OnGUI()
	{
		if (!isVisible)
		{
			return;
		}

		_windowRect = GUILayout.Window(123456, _windowRect, DrawConsoleWindow, WINDOW_TITLE);
	}

	void DrawConsoleWindow(int windowId)
	{
		GUI.DragWindow(_titleBarRect);
		DrawLogsList();
		DrawToolBar();
	}

	void DrawLogsList()
	{
		_scrollPosition = GUILayout.BeginScrollView(_scrollPosition);

		for (int i = 0; i < _logs.Count; i++)
		{
			Log log = _logs[i];
			if (_isCollapse && i > 0)
			{
				var previousMessage = _logs[i - 1].message;
				if (log.message == previousMessage)
				{
					continue;
				}
			}

			GUI.contentColor = logTypeColors[log.type];
			GUILayout.Label(log.message);
		}

		GUILayout.EndScrollView();
	}

	void DrawToolBar()
	{
		GUILayout.BeginHorizontal();

		if (GUILayout.Button(_clearLabel))
		{
			_logs.Clear();
		}
        if(GUILayout.Button("关闭"))
        {
            isVisible = false;
        }
		_isCollapse = GUILayout.Toggle(_isCollapse, _collapseLabel, GUILayout.ExpandWidth(false));
		GUILayout.EndHorizontal();
	}
}
