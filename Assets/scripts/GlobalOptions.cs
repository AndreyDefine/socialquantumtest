using UnityEngine;
using System.Collections;

/// <summary>
/// Класс глобальных настроек
/// </summary>

public class GlobalOptions {	
	private static float _difficultyLevel = 1f;
    public static float difficultyLevel {
        get {
            return _difficultyLevel;
        }
		set {
			_difficultyLevel = value;
		}
    }
	
	private static int _score = 0;
    public static int score {
        get {
            return _score;
        }
		set {
			_score = value;
		}
    }
}

