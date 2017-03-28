using UnityEngine;
using System.Collections;

public class LoadingTimer : SingletonMonoBehaviour<LoadingTimer>
{
    bool _stoper = false;

	float _time = 0;
	const float WAIT_TIME = 60.0f;

	void Start () 
	{
		_time = 0;
	}

    public void IsTimerStop (bool isStop) 
    {
        _stoper = isStop;
    }

	// 非アクティブになったとき時間リセットしとく
	void OnDisable()
	{
		_time = 0;
	}

	// アクティブな間は時間数えて１０秒たったら自動的に非アクティブにする
	void Update () 
	{
        if(this.gameObject.activeSelf && _stoper == false)
		{
			_time += Time.deltaTime;
			if(_time >= WAIT_TIME)
			{
				this.gameObject.SetActive (false);
				_time = 0;
			}
		}
	}
}
