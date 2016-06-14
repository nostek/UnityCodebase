using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class TouchInputBehaviour : MonoBehaviour
{
	public enum TouchState
	{
		Tap,

		TouchDragBegin,
		TouchDragging,
		TouchDragEnd,

		TouchDown,
		TouchUpdate,
		TouchUp,
	}

	public class TouchData
	{
		public TouchState State;
		public int TouchId;
		public float StartedTime;
		public Vector3 Start;
		public Vector3 Position;
		public Vector3 Offset;

		public float CurrentTime
		{
			get
			{
				return UnityEngine.Time.time;
			}
		}

		public float Distance
		{
			get
			{
				return (Position - Start).magnitude;
			}
		}

		public bool GoingLeft
		{
			get
			{
				return (Position.x < Start.x);
			}
		}

		public bool GoingRight
		{
			get
			{
				return (Position.x > Start.x);
			}
		}

		public bool Horizontal
		{
			get
			{
				return (Mathf.Abs(Start.x - Position.x) > Mathf.Abs(Start.y - Position.y));
			}
		}

		public bool GoingUp
		{
			get
			{
				return (Position.y < Start.y);
			}
		}

		public bool GoingDown
		{
			get
			{
				return (Position.y > Start.y);
			}
		}

		public bool Vertical
		{
			get
			{
				return (Mathf.Abs(Start.x - Position.x) < Mathf.Abs(Start.y - Position.y));
			}
		}

		//Offset for screens width & height
		public Vector3 OffsetScreen
		{
			get
			{
				return new Vector3(
					Offset.x / Screen.width,
					Offset.y / Screen.height
				);
			}
		}

		//Offset for screens Min(width, height)
		public Vector3 OffsetScreenMin
		{
			get
			{
				float min = ScreenMinimum;

				return new Vector3(
					Offset.x / min,
					Offset.y / min
				);
			}
		}

		public float ScreenMinimum
		{
			get
			{
				return Mathf.Min(Screen.width, Screen.height);
			}
		}

		public float Ratio
		{
			get
			{
				float screenWidth = Screen.width;
				float screenHeight = Screen.height;

				float ratio = 0;

				if (screenWidth > screenHeight)
				{
					ratio = Mathf.Min(screenWidth / 480, screenHeight / 320);
				}
				else
				{
					ratio = Mathf.Min(screenWidth / 320, screenHeight / 480);
				}

				return ratio;
			}
		}
	}

	class TouchIntermediate
	{
		public float Time;
		public Vector2 Start;
		public Vector2 Offset;
		public bool Started;
		public bool Down;
	}

	[SerializeField]
	float offsetStart = 5;

	[SerializeField]
	float offsetTap = 2;

	TouchData data;

	//11 should be enough for all fingers. 10 + nose :)
	TouchIntermediate[] intermediate;

	protected virtual void Start()
	{
		data = new TouchData();

		intermediate = new TouchIntermediate[11];
		for (int i = 0; i < 11; i++)
			intermediate[i] = new TouchIntermediate();

		Input.simulateMouseWithTouches = false;
	}

	protected virtual void Update()
	{
		//Reload
		if (intermediate == null)
		{
			Start();
		}

		if (Input.touchSupported)
		{
			for (int touchIndex = 0; touchIndex < Input.touchCount; touchIndex++)
			{
				Touch touch = Input.GetTouch(touchIndex);

				TouchIntermediate current = intermediate[touch.fingerId];

				if (touch.phase == TouchPhase.Began)
				{
					if (EventSystem.current.IsPointerOverGameObject(touch.fingerId))
					{
						current.Down = false;
					}
					else
					{
						current.Time = Time.time;
						current.Down = true;
						current.Started = false;
						current.Start = touch.position;
						current.Offset = touch.position;

						data.State = TouchState.TouchDown;
						data.TouchId = touch.fingerId;
						data.StartedTime = Time.time;
						data.Position = touch.position;
						data.Start = touch.position;
						data.Offset.Set(0, 0, 0);
						OnTouchDown(data);
					}
				}

				if (current.Down)
				{
					if (touch.phase == TouchPhase.Stationary || touch.phase == TouchPhase.Moved)
					{
						Vector2 diff = (touch.position - current.Offset);

						if (current.Started)
						{
							current.Offset = touch.position;

							data.State = TouchState.TouchDragging;
							data.TouchId = touch.fingerId;
							data.StartedTime = current.Time;
							data.Position = touch.position;
							data.Start = current.Start;
							data.Offset = diff;
							OnTouchDragging(data);
						}
						else
						{
							if ((touch.position - current.Offset).magnitude > offsetStart)
							{
								current.Started = true;
								current.Start = touch.position;
								current.Offset = touch.position;

								data.State = TouchState.TouchDragBegin;
								data.TouchId = touch.fingerId;
								data.StartedTime = Time.time;
								data.Position = touch.position;
								data.Start = current.Start;
								data.Offset.Set(0, 0, 0);
								OnTouchDragBegin(data);
							}
						}

						data.State = TouchState.TouchUpdate;
						data.TouchId = touch.fingerId;
						data.StartedTime = current.Time;
						data.Position = touch.position;
						data.Start = current.Start;
						data.Offset = diff;
						OnTouchUpdate(data);
					}

					if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
					{
						Vector2 diff = (touch.position - current.Offset);

						if (current.Started == false)
						{
							if (diff.magnitude <= offsetTap)
							{
								data.State = TouchState.Tap;
								data.TouchId = touch.fingerId;
								data.StartedTime = current.Time;
								data.Position = touch.position;
								data.Start = current.Start;
								data.Offset.Set(0, 0, 0);
								OnTap(data);
							}
						}
						else
						{
							data.State = TouchState.TouchDragEnd;
							data.TouchId = touch.fingerId;
							data.StartedTime = current.Time;
							data.Position = touch.position;
							data.Start = current.Start;
							data.Offset = diff;
							OnTouchDragEnd(data);
						}

						data.State = TouchState.TouchUp;
						data.TouchId = touch.fingerId;
						data.StartedTime = current.Time;
						data.Position = touch.position;
						data.Start = current.Start;
						data.Offset = diff;
						OnTouchUp(data);

						current.Down = false;
						current.Started = false;
					}
				}
			}
		}
		else
		{
			Vector2 pos = Input.mousePosition;

			for (int mouseIndex = 0; mouseIndex < 5; mouseIndex++)
			{
				TouchIntermediate current = intermediate[mouseIndex];

				if (Input.GetMouseButtonDown(mouseIndex))
				{
					if (EventSystem.current.IsPointerOverGameObject())
					{
						current.Down = false;
					}
					else
					{
						current.Time = Time.time;
						current.Down = true;
						current.Started = false;
						current.Start = pos;
						current.Offset = pos;

						data.State = TouchState.TouchDown;
						data.TouchId = mouseIndex;
						data.StartedTime = Time.time;
						data.Position = pos;
						data.Start = pos;
						data.Offset.Set(0, 0, 0);
						OnTouchDown(data);
					}
				}

				if (current.Down)
				{
					if (Input.GetMouseButton(mouseIndex))
					{
						Vector2 diff = (pos - current.Offset);

						if (current.Started)
						{
							current.Offset = pos;

							data.State = TouchState.TouchDragging;
							data.TouchId = mouseIndex;
							data.StartedTime = current.Time;
							data.Position = pos;
							data.Start = current.Start;
							data.Offset = diff;
							OnTouchDragging(data);
						}
						else
						{
							if ((pos - current.Offset).magnitude > offsetStart)
							{
								current.Started = true;
								current.Start = pos;
								current.Offset = pos;

								data.State = TouchState.TouchDragBegin;
								data.TouchId = mouseIndex;
								data.StartedTime = current.Time;
								data.Position = pos;
								data.Start = current.Start;
								data.Offset.Set(0, 0, 0);
								OnTouchDragBegin(data);
							}
						}

						data.State = TouchState.TouchUpdate;
						data.TouchId = mouseIndex;
						data.StartedTime = current.Time;
						data.Position = pos;
						data.Start = current.Start;
						data.Offset = diff;
						OnTouchUpdate(data);
					}

					if (Input.GetMouseButtonUp(mouseIndex))
					{
						Vector2 diff = (pos - current.Offset);

						if (current.Started == false)
						{
							if (diff.magnitude <= offsetTap)
							{
								data.State = TouchState.Tap;
								data.TouchId = mouseIndex;
								data.StartedTime = current.Time;
								data.Position = pos;
								data.Start = current.Start;
								data.Offset.Set(0, 0, 0);
								OnTap(data);
							}
						}
						else
						{
							data.State = TouchState.TouchDragEnd;
							data.TouchId = mouseIndex;
							data.StartedTime = current.Time;
							data.Position = pos;
							data.Start = current.Start;
							data.Offset = diff;
							OnTouchDragEnd(data);
						}

						data.State = TouchState.TouchUp;
						data.TouchId = mouseIndex;
						data.StartedTime = current.Time;
						data.Position = pos;
						data.Start = current.Start;
						data.Offset = diff;
						OnTouchUp(data);

						current.Down = false;
						current.Started = false;
					}
				}
			}
		}
	}

	public int Touches
	{
		get
		{
			int c = 0;

			for (int i = 0; i < intermediate.Length; i++)
				if (intermediate[i].Started)
					c++;

			return c;
		}
	}

	public TouchData[] ActiveTouches
	{
		get
		{
			int touches = Touches;

			TouchData[] ret = new TouchData[touches];

			int index = 0;

			for (int i = 0; i < intermediate.Length; i++)
				if (intermediate[i].Started)
					ret[index++] = new TouchData()
					{
						Start = intermediate[i].Start,
						Position = intermediate[i].Offset
					};

			return ret;
		}
	}

	//Called when Touch has gone up and not moved out of its Tap circle.
	protected virtual void OnTap(TouchData data)
	{
	}

	//Called when touch goes down.
	protected virtual void OnTouchDown(TouchData data)
	{
	}

	//Called every frame when touch is down.
	protected virtual void OnTouchUpdate(TouchData data)
	{
	}

	//Called when touch goes up.
	protected virtual void OnTouchUp(TouchData data)
	{
	}

	//Called when touch has gone down and moved outside the Tap circle.
	protected virtual void OnTouchDragBegin(TouchData data)
	{
	}

	//Called every frame after dragging has began.
	protected virtual void OnTouchDragging(TouchData data)
	{
	}

	//Called when touch has gone up after dragging has began.
	protected virtual void OnTouchDragEnd(TouchData data)
	{
	}
}
